using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;
using System.Collections.Generic;

namespace RemoteFileExplorer
{
    public class Server : Socket
    {
        private bool m_KeepAlive = true;
        private bool m_IsAlive = false;
        public Listener m_Server;
        private TcpClient m_CurrentClient;

        

        public override TcpClient CurrentClient
        {
            get
            {
                return m_CurrentClient;
            }
        }

        public void Start(string host, int port)
        {
            if (m_Server != null)
            {
                return;
            }
            m_Server = new Listener(IPAddress.Parse(host), port);
            m_Server.ExclusiveAddressUse = true;
            StartListen();
        }

        public void StartListen()
        {
            if(!m_Server.IsListening)
            {
                m_Server.Start(1);
            }
            Status = ConnectStatus.Connecting;
            m_Server.BeginAcceptTcpClient((asyncResult) => {
                m_CurrentClient = m_Server.EndAcceptTcpClient(asyncResult);
                m_CurrentClient.SendBufferSize = defaultBufferSize;
                m_CurrentClient.ReceiveBufferSize = defaultBufferSize;
                StartTransferThreads();
                m_Server.Stop();
            }, this);
        }
        
        public override void Close()
        {
            base.Close();
            if (m_CurrentClient != null)
            {
                m_CurrentClient.Close();
            }
            m_CurrentClient = null;
            if(m_Server != null)
            {
                StartListen();
            }
        }

        public void Stop()
        {
            if (m_Server != null)
            {
                m_Server.Stop();
            }
            m_Server = null;
            Close();
        }
    }

    public class Listener : TcpListener
    {
        public Listener(IPAddress localaddr, int port) : base(localaddr, port) {}

        public bool IsListening
        {
            get 
            {
                return this.Active;
            }
        }
    }
    
}