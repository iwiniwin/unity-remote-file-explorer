using System;

namespace RemoteFileExplorer
{
    public abstract class Command : ISerialize
    {
        public bool IsFinished = true;

        public virtual CommandType Type {get; protected set;}

        public UInt32 Seq = 0;
        public UInt32 Ack = 0;
        public string Error = "";

        public virtual Octets Serialize()
        {
            Octets octets = OctetsCache.Instance.Get();
            Packer.Bind(octets);
            Seq = UniqueSeq.Get();
            Packer.WriteUInt(Seq);
            Packer.WriteUInt(Ack);
            Packer.WriteBool(IsFinished);
            Packer.WriteString(Error);
            Packer.Unbind();
            return octets;
        }

        public virtual int Deserialize(Octets octets)
        {
            Unpacker.Bind(octets);
            this.Seq = Unpacker.ReadUInt();
            this.Ack = Unpacker.ReadUInt();
            this.IsFinished = Unpacker.ReadBool();
            this.Error = Unpacker.ReadString();
            return Unpacker.Unbind();
        }

    }

    internal class UniqueSeq 
    {
        private static UInt32 m_Seq = 0;

        public static UInt32 Get()
        {
            return ++ m_Seq;
        }
    }
}