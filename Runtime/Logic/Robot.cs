using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace RemoteFileExplorer
{
    public class Robot
    {
        private static string[] emptyStringArray = new string[]{};
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
            if(command is QueryPathInfo.Req || command is QueryPathKeyInfo.Req)
            {
                
                string path;
                if(command is QueryPathKeyInfo.Req)
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
                if(exists)  // 文件夹
                {
                    path += "/";
                }
                else if(File.Exists(path))  // 文件
                {
                    exists = true;
                    path = Path.GetDirectoryName(path); // 如果是文件，返回文件所在目录的子文件夹与子文件
                }
                QueryPathInfo.Rsp rsp;
                if(command is QueryPathKeyInfo.Req)
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
                catch(Exception e)
                {
                    rsp.Directories = emptyStringArray;
                    rsp.Files = emptyStringArray;
                    rsp.Error = e.Message;
                }
                rsp.Ack = command.Seq;
                m_Socket.Send(rsp);
            }
            else if(command is Download.Req)
            {
                Coroutines.Start(ProcessDownload(command as Download.Req));
            }
        }

        private IEnumerator ProcessDownload(Download.Req downloadReq)
        {
            string path = downloadReq.Path;
            Download.Rsp rsp = new Download.Rsp(){
                Ack = downloadReq.Seq,
            };
            string[] files = null;
            try
            {
                if(File.Exists(path))  // 单文件下载
                {
                    files = new string[]{path};
                }
                else
                {
                    files = Directory.GetFiles(path);
                }
            }
            catch(Exception e)
            {
                Debug.Log(e.ToString());
                Debug.Log(e.Message + "   vvv");
                rsp.Error = e.Message;
                m_Socket.Send(rsp);
                yield break;
            }
            foreach(string file in files)
            {
                byte[] content;
                try
                {
                    content = File.ReadAllBytes(file);
                }
                catch(Exception e) 
                {
                    rsp.Error = e.Message;
                    m_Socket.Send(rsp);
                    yield break;
                }
                TransferFile.Req req = new TransferFile.Req(){
                    Ack = downloadReq.Seq,
                    Path = file, 
                    Content = content,
                    IsFinished = false
                };
                CommandHandle handle = m_Socket.Send(req);
                yield return handle;
                if(handle.Error != null || !string.IsNullOrEmpty(handle.Command.Error))
                {
                    yield break;
                }
            }
            m_Socket.Send(rsp);
        }
    }
}