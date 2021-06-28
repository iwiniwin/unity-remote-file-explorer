using System;

namespace RemoteFileExplorer
{
    public class Package : ISerialize
    {
        public Header Head;
        public Octets Body;

        public Octets Serialize()
        {
            Octets octets = Head.Serialize();
            octets.Push(Body);
            return octets;
        }

        public int Deserialize(Octets octets)
        {
            Head = new Header();
            int size = Head.Deserialize(octets);
            octets.Erase(0, size);
            Body = octets;
            return 0;
        }

        public class Header : ISerialize
        {
            public UInt32 Size;
            public UInt32 Type;

            public static int Length 
            {
                get 
                {
                    return sizeof(UInt32) * 2;
                }
            }

            public Octets Serialize()
            {
                Octets octets = new Octets();
                Packer.Bind(octets);
                Packer.WriteUInt(Size);
                Packer.WriteUInt(Type);
                Packer.Unbind();
                return octets;
            }

            public int Deserialize(Octets octets)
            {
                Unpacker.Bind(octets);
                this.Size = Unpacker.ReadUInt();
                this.Type = Unpacker.ReadUInt();
                return Unpacker.Unbind();
            }
        }
    }
}