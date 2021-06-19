using UnityEngine;

namespace RemoteFileExplorer
{
    public class RemoteFileSystem : MonoBehaviour
    {
        private RFSClient m_Client;
        public string host;
        public int port;
        public bool connectAutomatically;

        private void Start() {
             m_Client = new RFSClient();
             m_Client.OnReceivePackage += OnClientReceivePackage;
             if(connectAutomatically)
             {
                 m_Client.StartConnect(host, port);
             }
        }

        public void StartConnect()
        {
            m_Client.StartConnect(host, port);
        }

        public void OnClientReceivePackage(Package package)
        {
            Debug.Log("  收到。。。。。。");

            Package rsp = CommandHandler.Handle(package);
            if (rsp != null)
            {
                Debug.Log("回复。。。。。。");
                m_Client.Send(rsp);
            }
        }

        private void OnDestroy() {
            if(m_Client != null)
            {
                m_Client.Close();
            }
        }
    }
}