using System;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Concurrent;
using UnityEngine;

namespace URFS
{
    public enum ConnectStatus
    {
        Connected,
        Connecting,
        Disconnect,
    }

    public abstract class Session
    {
        private string m_Host;
        private int m_Port;
        private ConnectStatus m_Status = ConnectStatus.Disconnect;

        public event Action<ConnectStatus> OnConnectStatusChanged;
        public event Action<Package> OnReceivePackage;

        public ConnectStatus Status
        {
            get
            {
                return m_Status;
            }
            protected set
            {
                if (m_Status != value && OnConnectStatusChanged != null)
                {
                    OnConnectStatusChanged(value);
                }
                m_Status = value;
            }
        }

        public abstract TcpClient CurrentClient
        {
            get;
        }

        private Octets m_SendOctets;
        private byte[] m_SendBuffer;
        private Thread m_SendThread;
        private AutoResetEvent m_SendResetEvent;

        private ConcurrentQueue<Package> m_SendQueue;
        private Thread m_PackThread;
        private AutoResetEvent m_PackResetEvent;

        public void Send(Package package)
        {
            if (m_SendQueue == null)
            {
                return;
            }
            m_SendQueue.Enqueue(package);
            m_PackResetEvent.Set();
        }

        public void Update(float dt)
        {
            if(m_ReceiveQueue != null && m_ReceiveQueue.Count > 0)
            {
                Package package;
                m_ReceiveQueue.TryDequeue(out package);
                if(OnReceivePackage != null)
                {
                    OnReceivePackage(package);
                }
            }
        }

        public void StartTransferThreads()
        {
            bool changed = m_Status != ConnectStatus.Connected;
            m_Status = ConnectStatus.Connected;
            StartSendThread();
            StartReceiveThread();
            if(changed && OnConnectStatusChanged != null)
            {
                OnConnectStatusChanged(m_Status);
            }
        }

        public void StartSendThread()
        {
            m_SendOctets = new Octets(1024 * 1024);
            m_SendBuffer = new byte[CurrentClient.SendBufferSize];
            m_SendResetEvent = new AutoResetEvent(false);
            m_SendThread = new Thread(SendThreadFunction);
            m_SendThread.Start();

            m_SendQueue = new ConcurrentQueue<Package>();
            m_PackResetEvent = new AutoResetEvent(false);
            m_PackThread = new Thread(PackThreadFunction);
            m_PackThread.Start();
        }

        public void SendThreadFunction()
        {
            while (Status == ConnectStatus.Connected)
            {
                m_SendResetEvent.WaitOne();
                while (m_SendOctets.Length > 0)
                {
                    byte[] sendBytes;
                    lock (m_SendOctets)
                    {
                        int length = Math.Min(m_SendOctets.Length, m_SendBuffer.Length);
                        m_SendOctets.Erase(0, length, out sendBytes);
                    }
                    try
                    {
                        CurrentClient.GetStream().Write(sendBytes, 0, sendBytes.Length);
                    }
                    catch (Exception e)
                    {
                        if (Status != ConnectStatus.Disconnect)
                        {
                            Close();
                        }
                    }
                }
            }
        }

        public void PackThreadFunction()
        {
            while (Status == ConnectStatus.Connected)
            {
                m_PackResetEvent.WaitOne();
                Package package;
                while (m_SendQueue.TryDequeue(out package))
                {
                    lock (m_SendOctets)
                    {
                        Packer.Bind();
                        Packer.WriteUInt(package.Head.Seq);
                        Packer.WriteUInt(package.Head.Seq);
                        Packer.WriteUInt(package.Head.Seq);
                        Packer.WriteUInt(package.Head.Seq);
                        m_SendOctets.Push(Packer.GetBuffer(), PackageHead.Length);
                        Packer.Unbind();
                        m_SendResetEvent.Set();
                    }
                    PackageManager.Instance.Release(package);
                }
            }
        }

        private Octets m_ReceiveOctets;
        private byte[] m_ReceiveBuffer;
        private Thread m_ReceiveThread;

        private ConcurrentQueue<Package> m_ReceiveQueue;
        private Thread m_UnpackThread;
        private AutoResetEvent m_UnpackResetEvent;

        public void StartReceiveThread()
        {
            m_ReceiveOctets = new Octets(1024 * 1024);
            m_ReceiveBuffer = new byte[CurrentClient.ReceiveBufferSize];
            m_ReceiveThread = new Thread(ReceiveThreadFunction);
            m_ReceiveThread.Start();

            m_ReceiveQueue = new ConcurrentQueue<Package>();
            m_UnpackResetEvent = new AutoResetEvent(false);
            m_UnpackThread = new Thread(UnpackThreadFunction);
            m_UnpackThread.Start();
        }

        public void ReceiveThreadFunction()
        {
            while (Status == ConnectStatus.Connected)
            {
                int readLength = 0;
                try
                {
                    readLength = CurrentClient.GetStream().Read(m_ReceiveBuffer, 0, m_ReceiveBuffer.Length);
                }
                catch (Exception e)
                {
                    if (Status != ConnectStatus.Disconnect)
                    {
                        Close();
                    }
                    return;
                }
                Debug.Log(readLength + " read length");
                if(readLength == 0)  // 主动断开
                {
                    Close();
                }
                else if (readLength > 0)
                {
                    lock (m_ReceiveOctets)
                    {
                        m_ReceiveOctets.Push(m_ReceiveBuffer, readLength);
                        m_UnpackResetEvent.Set();
                    }
                }
            }
        }

        public void UnpackThreadFunction()
        {
            while (Status == ConnectStatus.Connected)
            {
                m_UnpackResetEvent.WaitOne();
                while (true)
                {
                    if (m_ReceiveOctets.Length < PackageHead.Length)
                    {
                        break;
                    }
                    int packageLength = (int)BitConverter.ToUInt32(m_ReceiveOctets.Buffer, 0);
                    if (m_ReceiveOctets.Length < packageLength)
                    {
                        break;
                    }
                    byte[] receiveBytes;
                    lock (m_ReceiveOctets)
                    {
                        m_ReceiveOctets.Erase(0, packageLength, out receiveBytes);
                    }
                    Package package = PackageManager.Instance.Get();
                    Unpacker.Bind(receiveBytes);
                    package.Head = PackageHead.Create(Unpacker.ReadUInt(), Unpacker.ReadUInt(), Unpacker.ReadUInt(), Unpacker.ReadUInt());
                    Unpacker.Unbind();
                    package.Body.Push(receiveBytes, PackageHead.Length, packageLength - PackageHead.Length);
                    m_ReceiveQueue.Enqueue(package);
                }
            }
        }

        public virtual void Close()
        {
            Status = ConnectStatus.Disconnect;

            if (m_SendQueue != null)
            {
                Package package;
                while (m_SendQueue.TryDequeue(out package))
                {
                    PackageManager.Instance.Release(package);
                }
            }

            if (m_ReceiveQueue != null)
            {
                Package package;
                while (m_ReceiveQueue.TryDequeue(out package))
                {
                    PackageManager.Instance.Release(package);
                }
            }
        }
    }
}
