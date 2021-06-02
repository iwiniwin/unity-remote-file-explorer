using UnityEngine;
using UnityEditor;
using System.Collections;

namespace URFS.Editor
{
    public class RFSWindow : EditorWindow
    {
        private static string m_Host = "192.168.1.6";
        private static int m_Port = 8999;

        [MenuItem("Window/RFS Window")]
        static void Init()
        {
            EditorWindow.GetWindow(typeof(RFSWindow));
        }

        private RFSServer m_Server;

        private void Awake()
        {

        }

        public void OnConnectStatusChanged(ConnectStatus status)
        {
            switch (status)
            {
                case ConnectStatus.Connected:
                    Debug.Log("服务器 已连接。。。。。。");
                    Coroutines.Start(SendRequest());
                    break;
                case ConnectStatus.Connecting:
                    Debug.Log("服务器 正在连接。。。。。。");
                    break;
                case ConnectStatus.Disconnect:
                    Debug.Log("服务器 断开连接。。。。。。");
                    break;
            }
        }

        public IEnumerator SendRequest()
        {
            QueryDirectoryInfo.Req req = new QueryDirectoryInfo.Req
            {
                Directory = "E:/UnityProject/LastBattle/Assets/Scripts/Game/Message",
            };
            Debug.Log("auto senddd ");
            SendHandle handle = RFS.Instance.Server.Send(req.Pack());
            yield return handle;
            QueryDirectoryInfo.Rsp rsp = new QueryDirectoryInfo.Rsp();
            rsp.Unpack(handle.Rsp);
            UDK.Output.Dump(rsp.Exists);
            UDK.Output.Dump(rsp.SubDirectories);
            UDK.Output.Dump(rsp.SubFiles);
        }

        private void Update()
        {
            Coroutines.Update();
            if (m_Server == null)
            {
                m_Server = RFS.Instance.Server;
                if (m_Server.Status == ConnectStatus.Disconnect)
                {
                    m_Server.OnConnectStatusChanged += OnConnectStatusChanged;
                    m_Server.Start(m_Host, m_Port);
                }
            }
        }

        private void OnDestroy()
        {
            RFS.Instance.Server.Stop();
        }
    }
}