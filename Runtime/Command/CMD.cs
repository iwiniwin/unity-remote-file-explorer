namespace URFS
{
    public enum CMD : uint
    {
        QueryDirectoryInfo,
    }

    public static class CMDExtend
    {
        public static uint ToUInt(this CMD t)
        {
            return (uint)t.GetHashCode();
        }
    }
}