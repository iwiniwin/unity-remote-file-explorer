using System.Net.Sockets;
using UnityEngine;

namespace RemoteFileExplorer
{
    public class RFS : Singleton<RFS>
    {
        
        private RFSServer m_Server;

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
    }
}