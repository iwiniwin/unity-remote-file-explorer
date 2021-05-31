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
                    QueryDirectoryInfo.Req req = new QueryDirectoryInfo.Req
                    {
                        Directory = "E:/UnityProject/LastBattle/Assets/Scripts/Game/Message",
                    };
                    Debug.Log("auto senddd ");
                    RFS.Instance.Server.Send(req.Pack(PackerPool.Instance.Get()));
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
            if (m_Server == null)
            {
                m_Server = RFS.Instance.Server;
                if (m_Server.Status == ConnectStatus.Disconnect)
                {
                    m_Server.OnConnectStatusChanged += OnConnectStatusChanged;
                    m_Server.OnReceiveMessage += OnReceiveMessage;
                    m_Server.Start(m_Host, m_Port);
                }
            }
            RFS.Instance.Server.Update(0);
        }

        public void OnReceiveMessage(Unpacker unpacker)
        {

            URFS.CommandHandler.Handle(unpacker);
            // if (rspPacker != null)
            // {
            //     RFS.Instance.Client.Send(rspPacker);
            // }
        }

        private void OnDestroy()
        {
            RFS.Instance.Server.Stop();
        }
    }
}