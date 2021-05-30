namespace URFS
{
    public abstract class Message 
    {
        public abstract MessageUnpacker Unpack(MessageUnpacker unpacker);
        public abstract MessagePacker Pack(MessagePacker packer);
    }
}