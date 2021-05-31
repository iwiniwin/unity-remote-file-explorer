using System;

namespace URFS
{
    public abstract class Message
    {
        public MessageHeader Header;
        public abstract Unpacker Unpack(Unpacker unpacker);
        public abstract Packer Pack(Packer packer);

        public void BeginPack(Packer packer)
        {
            Header = new MessageHeader();
            packer.Data.Push(Header.GetBytes());
        }

        public void EndPack(Packer packer)
        {
            Header.Size = (UInt32)packer.Data.Length;
            packer.Data.Overwrite(Header.GetBytes(), 0);
        }

        public static MessageHeader UnpackHeader(Unpacker unpacker)
        {
            UInt32 size = unpacker.InternalReadUInt();
            UInt32 seq = unpacker.InternalReadUInt();
            UInt32 ack = unpacker.InternalReadUInt();
            return MessageHeader.Create(size, seq, ack);
        }

    }
}