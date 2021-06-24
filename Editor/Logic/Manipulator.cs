using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RemoteFileExplorer.Editor.UI;
using UnityEditor;
using System.IO;
using System;

namespace RemoteFileExplorer.Editor
{
    public class Manipulator
    {
        public string m_CurPath;

        public string curPath
        {
            get
            {
                return m_CurPath;
            }
            set
            {
                m_CurPath = value.Replace("\\", "/");
            }
        }

        private RemoteFileExplorerWindow m_Owner;
        public Manipulator(RemoteFileExplorerWindow owner)
        {
            m_Owner = owner;
        }

        public void GoTo(string path)
        {
            Coroutines.Start(Internal_GoTo(path));
        }

        public void GoTo(ObjectItem item)
        {
            var data = item.Data;
            if (data.type == ObjectType.File)
                return;
            Coroutines.Start(Internal_GoTo(data.path));
        }

        public void GoToByKey(string key)
        {
            Coroutines.Start(Internal_GoTo(key, true));
        }

        public void Select(ObjectItem item)
        {
            var data = item.Data;
            curPath = data.path;
            m_Owner.m_ObjectListArea.SetSelectItem(item);
        }

        /// <summary>
        /// 选择空
        /// </summary>
        public void Select()
        {
            ObjectItem item = m_Owner.m_ObjectListArea.GetSelectItem();
            if(item != null)
            {
                curPath = Path.GetDirectoryName(item.Data.path);
            }
            m_Owner.m_ObjectListArea.SetSelectItem(null);
        }

        public void Download(ObjectItem item)
        {
            Coroutines.Start(Internal_Download(item.Data.path));
        }

        public void Delete(ObjectItem item)
        {

        }

        public void Rename(ObjectItem item)
        {

        }

        public void Upload(string[] paths)
        {
            
        }

        /// <summary>
        /// 跳转到指定路径
        /// </summary>
        private IEnumerator Internal_GoTo(string path, bool isKey = false)
        {
            if (!CheckConnectStatus()) yield break;
            Command req;
            if (isKey)
            {
                req = new QueryPathKeyInfo.Req
                {
                    PathKey = path,
                };
            }
            else
            {
                req = new QueryPathInfo.Req
                {
                    Path = path,
                };
            }
            CommandHandle handle = m_Owner.m_Server.Send(req);
            yield return handle;
            if (!CheckHandleError(handle) || !CheckCommandError(handle.Command))
            {
                yield break;
            }
            var rsp = handle.Command as QueryPathInfo.Rsp;
            if (!rsp.Exists)
            {
                EditorUtility.DisplayDialog(Constants.WindowTitle, string.Format(Constants.PathNotExistTip, path), Constants.OkText);
                yield break;
            }
            List<ObjectData> list = new List<ObjectData>();

            foreach (var item1 in rsp.Directories)
            {
                list.Add(new ObjectData(ObjectType.Folder, item1));
            }
            foreach (var item1 in rsp.Files)
            {
                list.Add(new ObjectData(ObjectType.File, item1));
            }
            m_Owner.m_ObjectListArea.UpdateView(list);
            if (rsp is QueryPathKeyInfo.Rsp)
            {
                curPath = (rsp as QueryPathKeyInfo.Rsp).Path;
            }
            else
            {
                curPath = path;
            }
        }

        /// <summary>
        /// 下载
        /// </summary>
        private IEnumerator Internal_Download(string path)
        {
            if (!CheckConnectStatus()) yield break;
            var req = new Download.Req
            {
                Path = path,
            };
            CommandHandle handle = m_Owner.m_Server.Send(req);
            yield return handle;
            while (CheckHandleError(handle) && CheckCommandError(handle.Command))
            {
                if(handle.Command is TransferFile.Req)
                {
                    var transferFileReq = handle.Command as TransferFile.Req;
                    TransferFile.Rsp rsp = new TransferFile.Rsp(){
                        Ack = transferFileReq.Seq,
                    };
                    try
                    {
                        // File.WriteAllBytes(Path.Combine(path, transferFileReq.Path), transferFileReq.Content);
                        Debug.Log(transferFileReq.Path + "       ff " + transferFileReq.Content.Length);
                    }
                    catch(Exception e)
                    {
                        rsp.Error = e.Message;
                    }
                    m_Owner.m_Server.Send(rsp);
                    if(!CheckCommandError(rsp))
                    {
                        yield break;
                    }
                    handle.Finished = false;
                    yield return handle;
                }
                else if(handle.Command is Download.Rsp)
                {
                    EditorUtility.DisplayDialog(Constants.WindowTitle, "文件下载成功", Constants.OkText);
                    yield break;
                }
                else
                {
                    EditorUtility.DisplayDialog(Constants.WindowTitle, Constants.UnknownError, Constants.OkText);
                    yield break;
                }
            }
        }

        public bool CheckConnectStatus()
        {
            if (m_Owner.m_Server.Status == ConnectStatus.Connected)
            {
                return true;
            }
            EditorUtility.DisplayDialog(Constants.WindowTitle, Constants.NotConnectedTip, Constants.OkText);
            return false;
        }

        public bool CheckHandleError(CommandHandle handle)
        {
            if (handle.Error != null)
            {
                EditorUtility.DisplayDialog(Constants.WindowTitle, "handle.Error", Constants.OkText);
                return false;
            }
            return true;
        }

        public bool CheckCommandError(Command command)
        {
            if(!string.IsNullOrEmpty(command.Error))
            {
                EditorUtility.DisplayDialog(Constants.WindowTitle, command.Error, Constants.OkText);
                return false;
            }
            return true;
        }
    }
}