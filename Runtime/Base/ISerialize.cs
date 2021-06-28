namespace RemoteFileExplorer
{
    public interface ISerialize
    {
        Octets Serialize();
        int Deserialize(Octets octets);
    }
}
