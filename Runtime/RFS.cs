using System.Net.Sockets;
using UnityEngine;

namespace RemoteFileExplorer
{
    public class RFS : Singleton<RFS>
    {
        
        private BaseServer m_Server;

        public BaseServer Server
        {
            get
            {
                if (m_Server == null)
                {
                    m_Server = new BaseServer();
                }
                return m_Server;
            }
        }
    }
}