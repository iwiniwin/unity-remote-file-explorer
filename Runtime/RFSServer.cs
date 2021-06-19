using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;
using System.Collections.Generic;

namespace RemoteFileExplorer
{
    public class RFSServer : Session
    {
        private bool m_KeepAlive = true;
        private bool m_IsAlive = false;
        public TcpListener m_Server;
        private TcpClient m_CurrentClient;

        private Dictionary<UInt32, SendHandle> m_HandleDict = new Dictionary<uint, SendHandle>();

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
            m_Server = new TcpListener(IPAddress.Parse(host), port);
            m_Server.ExclusiveAddressUse = true;
            m_Server.Start(1);
            StartListen();
        }

        public void StartListen()
        {
            Status = ConnectStatus.Connecting;
            m_Server.BeginAcceptTcpClient((asyncResult) => {
                m_CurrentClient = m_Server.EndAcceptTcpClient(asyncResult);
                StartTransferThreads();
            }, this);
        }

        public new SendHandle Send(Package package)
        {
            base.Send(package);
            SendHandle handle = new SendHandle();
            handle.Finished = false;
            m_HandleDict.Add(package.Head.Seq, handle);
            return handle;
        }

        public override void Receive(Package package)
        {
            // RFS服务端只处理请求包的响应包
            UInt32 ack = package.Head.Ack;
            if(m_HandleDict.ContainsKey(ack))
            {
                m_HandleDict[ack].Finished = true;
                m_HandleDict[ack].Rsp = package;
                m_HandleDict[ack].Msg = "success";
                m_HandleDict.Remove(ack);
            }
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


    public class SendHandle : ICoroutineYield
    {
        public bool Finished = false;
        public bool IsDone()
        {
            return Finished;
        }
        public string Msg;
        public Package Rsp;
    }
}