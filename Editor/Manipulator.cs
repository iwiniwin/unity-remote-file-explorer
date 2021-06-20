using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RemoteFileExplorer.Editor.UI;

namespace RemoteFileExplorer.Editor
{
    public class Manipulator
    {
        private RemoteFileExplorerWindow m_Owner;
        public Manipulator(RemoteFileExplorerWindow owner)
        {
            m_Owner = owner;
        }

        public void GoToPath(string path)
        {
            path = "E:/UnityProject/LastBattle/Assets/Scripts/Game/Message";
            Coroutines.Start(Internal_GoToPath(path));
        }

        /// <summary>
        /// 跳转到指定路径
        /// </summary>
        private IEnumerator Internal_GoToPath(string path)
        {
            QueryDirectoryInfo.Req req = new QueryDirectoryInfo.Req
            {
                Directory = path,
            };
            Debug.Log("auto senddd ");
            SendHandle handle = m_Owner.m_Server.Send(req.Pack());
            yield return handle;
            QueryDirectoryInfo.Rsp rsp = new QueryDirectoryInfo.Rsp();
            rsp.Unpack(handle.Rsp);
            // UDK.Output.Dump(rsp.Exists);
            // UDK.Output.Dump(rsp.SubDirectories);
            // UDK.Output.Dump(rsp.SubFiles);
            // Debug.Log(rsp.Exists + "        vvvvvvv " + rsp.SubDirectories.Length);
            
            if(rsp.Exists)
            {
                List<ObjectData> list = new List<ObjectData>();
                
                foreach(var item in rsp.SubDirectories)
                {
                    list.Add(new ObjectData(ObjectType.Folder, item));
                }
                foreach(var item in rsp.SubFiles)
                {
                    list.Add(new ObjectData(ObjectType.File, item));
                }
                m_Owner.m_ObjectListArea.UpdateView(list);
            }
            
        }
    }
}