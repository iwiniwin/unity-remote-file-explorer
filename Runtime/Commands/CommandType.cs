namespace RemoteFileExplorer
{
    public enum CommandType : uint
    {
        QueryPathInfoReq = 1,
        QueryPathInfoRsp,
        QueryPathKeyInfoReq,
        QueryPathKeyInfoRsp,
        DownloadReq,
        DownloadRsp,
        TransferFileReq,
        TransferFileRsp,
    }

    public static class CommandTypeExtend
    {
        public static uint ToUInt(this CommandType t)
        {
            return (uint)t.GetHashCode();
        }
    }
}