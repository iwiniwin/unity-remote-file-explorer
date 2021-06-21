using System.Collections.Generic;
using System;

namespace RemoteFileExplorer
{
    public abstract class Socket : Session
    {
        private Dictionary<UInt32, CommandHandle> m_HandleDict = new Dictionary<uint, CommandHandle>();
        public Action<Command> OnReceiveCommand;

        public CommandHandle Send(Command command)
        {
            Package package = Serializer.Serialize(command);
            base.Send(package);
            var req = package.Head.Seq;
            
            if(!m_HandleDict.ContainsKey(req))
            {
                m_HandleDict.Add(package.Head.Seq, new CommandHandle());
            }
            m_HandleDict[req].Finished = false;
            return m_HandleDict[req];
        }

        public void OnReceive(Package package)
        {
            Command command = Serializer.Deserialize(package);
            if (OnReceiveCommand != null)
            {
                OnReceiveCommand(command);
            }
            UInt32 ack = package.Head.Ack;
            if (m_HandleDict.ContainsKey(ack))
            {
                m_HandleDict[ack].Finished = true;
                m_HandleDict[ack].Command = command;
                if(command.IsFinished)
                {
                    m_HandleDict.Remove(ack);
                }
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

    public class CommandHandle : ICoroutineYield
    {
        public bool Finished = false;
        public bool IsDone()
        {
            return Finished;
        }
        public string Error;
        public Command Command;
    }
}