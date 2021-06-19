namespace RemoteFileExplorer
{

    public enum DataType : byte
    {
        Byte,
        Bool,
        Int,
        UInt,
        String,
        Array,
    }

    public static class ExtendDataType
    {
        public static byte ToByte(this DataType t)
        {
            return (byte)t.GetHashCode();
        }
    }
}