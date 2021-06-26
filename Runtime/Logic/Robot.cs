using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace RemoteFileExplorer
{
    public class Robot
    {
        private static string[] emptyStringArray = new string[] { };
        private static string emptyString = "";

        public static Dictionary<string, string> PathKeyMap = new Dictionary<string, string>(){
            {"Application.dataPath", Application.dataPath},
            {"Application.persistentDataPath", Application.persistentDataPath},
            {"Application.streamingAssetsPath", Application.streamingAssetsPath},
            {"Application.consoleLogPath", Application.consoleLogPath},
            {"Application.temporaryCachePath", Application.temporaryCachePath}
        };

        private Socket m_Socket;

        public Robot(Socket socket)
        {
            m_Socket = socket;
        }

        public void Execute(Command command)
        {
            if (command is QueryPathInfo.Req || command is QueryPathKeyInfo.Req)
            {

                string path;
                if (command is QueryPathKeyInfo.Req)
                {
                    var req = command as QueryPathKeyInfo.Req;
                    path = Robot.PathKeyMap[req.PathKey];
                }
                else
                {
                    var req = command as QueryPathInfo.Req;
                    path = req.Path;
                }
                bool exists = Directory.Exists(path);
                if (exists)  // 文件夹
                {
                    path += "/";
                }
                else if (File.Exists(path))  // 文件
                {
                    exists = true;
                    path = Path.GetDirectoryName(path); // 如果是文件，返回文件所在目录的子文件夹与子文件
                }
                QueryPathInfo.Rsp rsp;
                if (command is QueryPathKeyInfo.Req)
                {
                    rsp = new QueryPathKeyInfo.Rsp()
                    {
                        Path = path,
                    };
                }
                else
                {
                    rsp = new QueryPathInfo.Rsp();
                }
                rsp.Exists = exists;
                try
                {
                    rsp.Directories = exists ? Directory.GetDirectories(path) : emptyStringArray;
                    rsp.Files = exists ? Directory.GetFiles(path) : emptyStringArray;
                    rsp.Error = emptyString;
                }
                catch (Exception e)
                {
                    rsp.Directories = emptyStringArray;
                    rsp.Files = emptyStringArray;
                    rsp.Error = e.Message;
                }
                rsp.Ack = command.Seq;
                m_Socket.Send(rsp);
            }
            else if (command is TransferFile.Req)
            {
                var req = command as TransferFile.Req;
                TransferFile.Rsp rsp = new TransferFile.Rsp()
                {
                    Ack = req.Seq,
                };
                try
                {
                    File.WriteAllBytes(req.Path, req.Content);
                }
                catch (Exception e)
                {
                    rsp.Error = e.Message;
                }
                m_Socket.Send(rsp);
            }
            else if (command is CreateDirectory.Req)
            {
                var req = command as CreateDirectory.Req;
                CreateDirectory.Rsp rsp = new CreateDirectory.Rsp()
                {
                    Ack = req.Seq,
                };
                try
                {
                    foreach (string directory in req.Directories)
                    {
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }
                    }
                }
                catch (Exception e)
                {
                    rsp.Error = e.Message;
                }
                m_Socket.Send(rsp);
            }
            else if (command is Pull.Req)
            {
                Coroutines.Start(ProcessDownload(command as Pull.Req));
            }
        }

        private IEnumerator ProcessDownload(Pull.Req downloadReq)
        {
            string path = downloadReq.Path;
            Pull.Rsp rsp = new Pull.Rsp()
            {
                Ack = downloadReq.Seq,
            };
            string[] directories = null;
            string[] files = null;
            try
            {
                if (File.Exists(path))  // 单文件下载
                {
                    files = new string[] { path };
                }
                else
                {
                    directories = FileUtil.GetAllDirectories(path);
                    files = FileUtil.GetAllFiles(path);
                }
            }
            catch (Exception e)
            {
                rsp.Error = e.Message;
                m_Socket.Send(rsp);
                yield break;
            }
            if (directories != null)
            {
                CreateDirectory.Req req = new CreateDirectory.Req()
                {
                    Ack = downloadReq.Seq,
                    Directories = directories,
                    IsFinished = false
                };
                CommandHandle handle = m_Socket.Send(req);
                yield return handle;
                if (handle.Error != null || !string.IsNullOrEmpty(handle.Command.Error))
                {
                    yield break;
                }
            }
            foreach (string file in files)
            {
                byte[] content;
                try
                {
                    content = File.ReadAllBytes(file);
                }
                catch (Exception e)
                {
                    rsp.Error = e.Message;
                    m_Socket.Send(rsp);
                    yield break;
                }
                TransferFile.Req transferFileReq = new TransferFile.Req()
                {
                    Ack = downloadReq.Seq,
                    Path = file,
                    Content = content,
                    IsFinished = false
                };
                CommandHandle transferFileHandle = m_Socket.Send(transferFileReq);
                yield return transferFileHandle;
                if (transferFileHandle.Error != null || !string.IsNullOrEmpty(transferFileHandle.Command.Error))
                {
                    yield break;
                }
            }
            m_Socket.Send(rsp);
        }
    }
}