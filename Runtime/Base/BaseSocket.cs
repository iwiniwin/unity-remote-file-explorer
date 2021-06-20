using System.Collections.Generic;
using System;

namespace RemoteFileExplorer
{
    public abstract class BaseSocket : Session
    {
        private Dictionary<UInt32, SendHandle> m_HandleDict = new Dictionary<uint, SendHandle>();
        public Action<Package> OnReceivePackage;

        public new SendHandle Send(Package package)
        {
            base.Send(package);
            SendHandle handle = new SendHandle();
            handle.Finished = false;
            m_HandleDict.Add(package.Head.Seq, handle);
            return handle;
        }

        public void OnReceive(Package package)
        {
            if (OnReceivePackage != null)
            {
                OnReceivePackage(package);
            }
            // RFS服务端只处理请求包的响应包
            UInt32 ack = package.Head.Ack;
            if (m_HandleDict.ContainsKey(ack))
            {
                m_HandleDict[ack].Finished = true;
                m_HandleDict[ack].Rsp = package;
                m_HandleDict[ack].Msg = "success";
                m_HandleDict.Remove(ack);
            }
        }

        public void Update()
        {
            if (m_ReceiveQueue != null && m_ReceiveQueue.Count > 0)
            {
                Package package;
                if(m_ReceiveQueue.TryDequeue(out package))
                {
                    OnReceive(package);
                }
            }
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