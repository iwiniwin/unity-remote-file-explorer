using System;

namespace URFS
{
    public abstract class Message 
    {
        public MessageHeader Header = new MessageHeader();
        public abstract Unpacker Unpack(Unpacker unpacker);
        public abstract Packer Pack(Packer packer);

        public void BeginPack(Packer packer)
        {
             packer.Data.Push(Header.GetBytes());
        }

        public void EndPack(Packer packer)
        {
            Header.Size = (UInt32)packer.Data.Length;
            packer.Data.Overwrite(Header.GetBytes(), 0);
        }
    }
}