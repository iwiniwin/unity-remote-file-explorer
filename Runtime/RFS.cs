using System.Net.Sockets;

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
                if(m_Client == null)
                {
                    m_Client = new RFSClient();
                }
                return m_Client;
            }
        }
        public RFSServer Server 
        {
            get
            {
                if(m_Server == null)
                {
                    m_Server = new RFSServer();
                }
                return m_Server;
            }
        }

        public void Update(float dt)
        {
            if(m_Client != null)
            {
                m_Client.Update(dt);
            }
            if(m_Server != null)
            {
                m_Server.Update(dt);
            }
        }
    }
}