using UnityEngine;

namespace RemoteFileExplorer
{
    public class FileExplorerClient : MonoBehaviour
    {
        private BaseClient m_Client;
        public string host;
        public int port;
        public bool connectAutomatically;

        private void Start()
        {
            m_Client = new BaseClient();
            m_Client.OnReceivePackage += OnReceivePackage;
            if (connectAutomatically)
            {
                m_Client.StartConnect(host, port);
            }
        }

        public void StartConnect()
        {
            m_Client.StartConnect(host, port);
        }

        private void Update() {
            m_Client.Update();
        }

        public void OnReceivePackage(Package package)
        {
            Debug.Log("  收到。。。。。。");

            Package rsp = CommandHandler.Handle(package);
            if (rsp != null)
            {
                Debug.Log("回复。。。。。。");
                m_Client.Send(rsp);
            }
        }

        private void OnDestroy()
        {
            if (m_Client != null)
            {
                m_Client.Close();
            }
        }
    }
}