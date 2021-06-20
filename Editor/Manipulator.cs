using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RemoteFileExplorer.Editor.UI;

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
            if(data.type == ObjectType.File)
                return;
            Coroutines.Start(Internal_GoTo(item.Data.path));
        }

        public void Select(ObjectItem item)
        {
            var data = item.Data;
            curPath = data.path;
        }

        public void Download(ObjectItem item)
        {

        }

        public void Delete(ObjectItem item)
        {

        }

        public void Rename(ObjectItem item)
        {

        }

        public void Upload()
        {

        }

        /// <summary>
        /// 跳转到指定路径
        /// </summary>
        private IEnumerator Internal_GoTo(string path)
        {
            if(!path.EndsWith("/"))
            {
                path += "/";
            }
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
                
                foreach(var item1 in rsp.SubDirectories)
                {
                    list.Add(new ObjectData(ObjectType.Folder, item1));
                }
                foreach(var item1 in rsp.SubFiles)
                {
                    list.Add(new ObjectData(ObjectType.File, item1));
                }
                m_Owner.m_ObjectListArea.UpdateView(list);
                curPath = path;
            }
            
        }
    }
}