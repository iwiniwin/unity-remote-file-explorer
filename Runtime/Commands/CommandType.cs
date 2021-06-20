namespace RemoteFileExplorer
{
    public enum CommandType : uint
    {
        QueryPathInfo = 1,
    }

    public static class CommandTypeExtend
    {
        public static uint ToUInt(this CommandType t)
        {
            return (uint)t.GetHashCode();
        }
    }
}