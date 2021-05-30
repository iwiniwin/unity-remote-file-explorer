namespace URFS
{
    public abstract class Message 
    {
        public abstract void Unpack(MessageUnpacker unpacker);
        public abstract void Pack(MessagePacker packer);
    }
}