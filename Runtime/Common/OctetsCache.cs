namespace RemoteFileExplorer
{
    public class OctetsCache : Singleton<OctetsPool> { }

    public class OctetsPool : Pool<Octets>
    {
        public Octets Get(int capacity)
        {
            Octets octets = base.Get();
            octets.Resize(capacity);
            return octets;
        }

        public override void Release(Octets octets)
        {
            octets.Clear();
            base.Release(octets);
        }
    }
}