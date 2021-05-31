namespace URFS
{
    public enum CMD : uint
    {
        QueryDirectoryInfo = 1,
    }

    public static class CMDExtend
    {
        public static uint ToUInt(this CMD t)
        {
            return (uint)t.GetHashCode();
        }
    }
}