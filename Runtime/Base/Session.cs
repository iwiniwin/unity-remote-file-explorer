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

    public class PackerPool : Singleton<Pool<MessagePacker>> { }

    public class UnpackerPool : Singleton<Pool<MessageUnpacker>> { }

    public abstract class Session
    {
        private string m_Host;
        private int m_Port;
        private ConnectStatus m_Status = ConnectStatus.Disconnect;

        public event Action<ConnectStatus> OnConnectStatusChanged;

        public ConnectStatus Status
        {
            get
            {
                return m_Status;
            }
            protected set
            {
                if (m_Status != value)
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

        private ConcurrentQueue<MessagePacker> m_PackerQueue;
        private Thread m_PackThread;
        private AutoResetEvent m_PackResetEvent;

        public void Send(MessagePacker packer)
        {
            if (m_PackerQueue == null)
            {
                return;
            }
            m_PackerQueue.Enqueue(packer);
            m_PackResetEvent.Set();
        }

        public virtual void Update(float dt)
        {
            if(CurrentClient != null)
            {
            }
        }

        public void StartSendThread()
        {
            m_SendOctets = new Octets(1024 * 1024);
            m_SendBuffer = new byte[CurrentClient.SendBufferSize];
            m_SendResetEvent = new AutoResetEvent(false);
            m_SendThread = new Thread(SendThreadFunction);
            m_SendThread.Start();

            m_PackerQueue = new ConcurrentQueue<MessagePacker>();
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
                MessagePacker packer;
                while (m_PackerQueue.TryDequeue(out packer))
                {
                    lock (m_SendOctets)
                    {
                        m_SendOctets.Push(packer.Data.Buffer);
                        m_SendResetEvent.Set();
                    }
                    packer.Reset();
                    PackerPool.Instance.Release(packer);
                }
            }
        }

        private Octets m_ReceiveOctets;
        private byte[] m_ReceiveBuffer;
        private Thread m_ReceiveThread;

        private ConcurrentQueue<MessageUnpacker> m_UnpackerQueue;
        private Thread m_UnpackThread;
        private AutoResetEvent m_UnpackResetEvent;

        public void StartReceiveThread()
        {
            m_ReceiveOctets = new Octets(1024 * 1024);
            m_ReceiveBuffer = new byte[CurrentClient.ReceiveBufferSize];
            m_ReceiveThread = new Thread(ReceiveThreadFunction);
            m_ReceiveThread.Start();

            m_UnpackerQueue = new ConcurrentQueue<MessageUnpacker>();
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
                    if (m_ReceiveOctets.Length < MessageHeader.Length)
                    {
                        break;
                    }
                    int messageLength = (int)BitConverter.ToUInt32(m_ReceiveOctets.Buffer, 0);
                    if (m_ReceiveOctets.Length < messageLength)
                    {
                        break;
                    }
                    byte[] receiveBytes;
                    lock (m_ReceiveOctets)
                    {
                        m_ReceiveOctets.Erase(0, messageLength, out receiveBytes);
                    }
                    MessageUnpacker unpacker = UnpackerPool.Instance.Get();
                    unpacker.Data.Push(receiveBytes);
                    m_UnpackerQueue.Enqueue(unpacker);
                }
            }
        }

        public virtual void Close()
        {
            Status = ConnectStatus.Disconnect;

            if (m_PackerQueue != null)
            {
                MessagePacker packer;
                while (m_PackerQueue.TryDequeue(out packer))
                {
                    packer.Reset();
                    PackerPool.Instance.Release(packer);
                }
            }

            if (m_UnpackerQueue != null)
            {
                MessageUnpacker unpacker;
                while (m_UnpackerQueue.TryDequeue(out unpacker))
                {
                    unpacker.Reset();
                    UnpackerPool.Instance.Release(unpacker);
                }
            }
        }
    }
}
