using System.Net.Sockets;

namespace URFS
{
    public class RFSClient : Session
    {
        private TcpClient m_Client;

        public override TcpClient CurrentClient
        {
            get
            {
                return m_Client;
            }
        }

        public void StartConnect(string host, int prot)
        {
            Status = ConnectStatus.Connecting;
            m_Client = new TcpClient(AddressFamily.InterNetwork);
            UnityEngine.Debug.Log(host + " " + prot + "       kehuduan ");
            m_Client.BeginConnect(host, prot, (asyncResult) =>
            {
                UnityEngine.Debug.Log(m_Client.Connected + "    gggg");
                if (m_Client.Connected)
                {
                    Status = ConnectStatus.Connected;
                    StartSendThread();
                    StartReceiveThread();
                }
                else
                {
                    Status = ConnectStatus.Disconnect;
                }
                m_Client.EndConnect(asyncResult);
            }, this);
        }

        public override void Close()
        {
            base.Close();
            if(m_Client != null)
            {
                m_Client.Close();
            }
            m_Client = null;
        }
    }

    
}