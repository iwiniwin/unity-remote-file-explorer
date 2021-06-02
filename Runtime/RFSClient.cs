using System;
using System.Net.Sockets;

namespace URFS
{
    public class RFSClient : Session
    {
        private TcpClient m_Client;

        public event Action<Package> OnReceivePackage;

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
            m_Client.BeginConnect(host, prot, (asyncResult) =>
            {
                if (m_Client.Connected)
                {
                    StartTransferThreads();
                }
                else
                {
                    Status = ConnectStatus.Disconnect;
                }
                m_Client.EndConnect(asyncResult);
            }, this);
        }

        public override void Receive(Package package)
        {
            if(OnReceivePackage != null)
            {
                OnReceivePackage(package);
            }
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