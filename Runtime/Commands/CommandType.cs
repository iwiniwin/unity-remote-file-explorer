namespace RemoteFileExplorer
{
    public enum CommandType : uint
    {
        QueryPathInfo = 1,
        QueryPathKeyInfo,
    }

    public static class CommandTypeExtend
    {
        public static uint ToUInt(this CommandType t)
        {
            return (uint)t.GetHashCode();
        }
    }
}