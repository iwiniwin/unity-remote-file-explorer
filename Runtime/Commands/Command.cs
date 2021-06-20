using System;

namespace RemoteFileExplorer
{
    public abstract class Command 
    {
        public abstract Octets Serialize();

        public abstract void Deserialize(Octets octets);

        public bool IsFinished;

        public UInt32 Seq = 0;
        public UInt32 Ack = 0;

    }
}