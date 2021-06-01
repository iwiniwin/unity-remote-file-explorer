namespace URFS
{
    public abstract class Message 
    {
        public abstract Package Pack();

        public abstract void Unpack(Package package);

    }
}