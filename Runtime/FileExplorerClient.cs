using UnityEngine;

namespace RemoteFileExplorer
{
    public class FileExplorerClient : MonoBehaviour
    {
        private Client m_Client;
        private Robot m_Robot;
        public string host;
        public int port;
        public bool connectAutomatically;

        private void Start()
        {
            m_Client = new Client();
            m_Robot = new Robot(m_Client);
            m_Client.OnReceiveCommand += OnReceiveCommand;
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
            if(m_Client != null)
            {
                m_Client.Update();
            }
        }

        public void OnReceiveCommand(Command command)
        {
            Debug.Log("  收到。。。。。。");

            m_Robot.Execute(command);
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