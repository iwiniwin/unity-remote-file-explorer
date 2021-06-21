using System;
using System.Net.Sockets;

namespace RemoteFileExplorer
{
    public class Client : Socket
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

        public override void Close()
        {
            base.Close();
            if (m_Client != null)
            {
                m_Client.Close();
            }
            m_Client = null;
        }
    }


}