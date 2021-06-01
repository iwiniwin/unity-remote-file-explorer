using System;

namespace URFS
{
    public class Package
    {
        public PackageHead Head;
        public Octets Body = new Octets();

        public void Reset() 
        {
            if(Head != null)
            {
                Head.Reset();
            }
            Body.Clear();
        }
    }

    public class PackageHead
    {
        private static UInt32 uniqueSeq = 0;
        public UInt32 Size;
        private UInt32 m_Seq;
        public UInt32 Ack;
        public UInt32 Type;

        public UInt32 Seq
        {
            get
            {
                return m_Seq;
            }
        }

        public static int Length
        {
            get
            {
                return sizeof(UInt32) * 4;
            }
        }

        public static PackageHead Create(UInt32 size, UInt32 seq, UInt32 ack, UInt32 type)
        {
            return new PackageHead(size, seq, ack, type);
        }

        private PackageHead(UInt32 size, UInt32 seq, UInt32 ack, UInt32 type)
        {
            this.Size = size;
            this.m_Seq = seq;
            this.Ack = ack;
            this.Type = type;
        }

        public PackageHead() : this(0) { }

        public PackageHead(UInt32 size) : this(size, 0) { }

        public PackageHead(UInt32 size, UInt32 ack)
        {
            this.Size = size;
            this.m_Seq = ++uniqueSeq;
            this.Ack = ack;
        }

        public void Reset()
        {
            this.Size = 0;
            this.m_Seq = ++ uniqueSeq;
            this.Ack = 0;
            this.Type = 0;
        }
    }

    public class PackagePool : Pool<Package>
    {
        public override Package Get()
        {
            Package package = base.Get();
            package.Reset();
            return package;
        }
    }

    public class PackageManager : Singleton<PackagePool> { }
}