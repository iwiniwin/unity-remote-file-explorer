using System.Net.Sockets;
using UnityEngine;

namespace URFS
{
    public class RFS : Singleton<RFS>
    {
        private RFSClient m_Client;
        private RFSServer m_Server;

        public RFSClient Client
        {
            get
            {
                if (m_Client == null)
                {
                    m_Client = new RFSClient();
                    m_Client.OnReceivePackage += OnClientReceivePackage;
                }
                return m_Client;
            }
        }
        public RFSServer Server
        {
            get
            {
                if (m_Server == null)
                {
                    m_Server = new RFSServer();
                }
                return m_Server;
            }
        }

        public void OnClientReceivePackage(Package package)
        {
            Debug.Log("  收到。。。。。。");

            Package rsp = URFS.CommandHandler.Handle(package);
            if (rsp != null)
            {
                Debug.Log("回复。。。。。。");
                RFS.Instance.Client.Send(rsp);
            }
        }
    }
}