using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;

namespace URFS
{
    public class RFSServer : Session
    {
        public TcpListener m_Server;
        private TcpClient m_CurrentClient;

        // private AutoResetEvent  m_AcceptResetEvent;
        private bool m_Stopped = true;

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
            m_Server.Start(1);
            // m_AcceptResetEvent = new AutoResetEvent(false);
            StartAcceptThread();
        }

        private Thread m_AcceptThread;

        public void StartAcceptThread()
        {
            m_Stopped = false;
            m_AcceptThread = new Thread(AcceptThreadFunction);
            m_AcceptThread.Start();
        }

        public void AcceptThreadFunction()
        {
            try
            {
                while (!m_Stopped)
                {
                    // if(Status == ConnectStatus.Disconnect)
                    // {

                    //     Status = ConnectStatus.Connecting;
                    //     try
                    //     {
                    //         m_CurrentClient = m_Server.AcceptTcpClient();
                    //         Status = ConnectStatus.Connected;
                    //         StartSendThread();
                    //         StartReceiveThread();
                    //     }
                    //     catch(ThreadAbortException e)
                    //     {

                    //     }
                    //     catch(Exception e) 
                    //     {
                    //         UnityEngine.Debug.Log(e.ToString());
                    //         Status = ConnectStatus.Disconnect;
                    //         UnityEngine.Debug.Log("1111111111111111");
                    //     }
                    //     UnityEngine.Debug.Log("zzzzzzzzzzzzzzzz");
                    // }
                    // UnityEngine.Debug.Log("ttttttttttttt");
                    Thread.Sleep(1000);
                    UnityEngine.Debug.Log("ggggggggg");
                }
            }
            catch(ThreadAbortException e)
            {
                UnityEngine.Debug.LogError("111     " + e.ToString());
            }
            catch(Exception e)
            {
                UnityEngine.Debug.LogError("22     " + e.ToString());
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

        }

        public void Stop()
        {
            m_Stopped = true;
            Close();
            if (m_Server != null)
            {
                m_Server.Stop();
            }
            m_Server = null;
        }
    }
}