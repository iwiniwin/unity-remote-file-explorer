using UnityEngine;
using UnityEditor;

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

        private void Awake()
        {
            Debug.Log("awake.........");
            var server = RFS.Instance.Server;
            if (server.Status == ConnectStatus.Disconnect)
            {
                server.OnConnectStatusChanged += OnConnectStatusChanged;
                server.Start(m_Host, m_Port);
            }
        }

        public void OnConnectStatusChanged(ConnectStatus status)
        {
            switch (status)
            {
                case ConnectStatus.Connected:
                    Debug.Log("服务器 已连接。。。。。。");
                    break;
                case ConnectStatus.Connecting:
                    Debug.Log("服务器 正在连接。。。。。。");
                    break;
                case ConnectStatus.Disconnect:
                    Debug.Log("服务器 断开连接。。。。。。");
                    break;
            }
        }

        private void Update() 
        {
            RFS.Instance.Update(0);
        }

        private void OnDestroy() {
            Debug.Log("on destroy.............");
            RFS.Instance.Server.Stop();
        }
    }
}