using System.IO;
using UDK;

namespace URFS
{
    public class CommandHandler
    {
        private static string[] emptyStringArray = new string[]{};
        public static Packer Handle(Unpacker unpacker)
        {
            MessageHeader header = Message.UnpackHeader(unpacker);
            CMD cmd = GetCMD(unpacker);
            if(header.Ack == 0)
            {
                return HandleRequest(header, cmd, unpacker);
            }
            else
            {
                HandleResponse(header, cmd, unpacker);
                return null;
            }
        }

        public static Packer HandleRequest(MessageHeader header, CMD cmd, Unpacker unpacker)
        {
            Packer packer = null;
            switch(cmd)
            {
                case CMD.QueryDirectoryInfo:
                    QueryDirectoryInfo.Req req = new QueryDirectoryInfo.Req();
                    req.Unpack(unpacker);

                    bool exists = Directory.Exists(req.Directory);
                    QueryDirectoryInfo.Rsp rsp = new QueryDirectoryInfo.Rsp{
                        Ack = header.Seq,
                        Exists = exists,
                        SubDirectories = exists ? Directory.GetDirectories(req.Directory) : emptyStringArray,
                        SubFiles = exists ? Directory.GetFiles(req.Directory) : emptyStringArray,
                    };
                    // packer = rsp.Pack(PackerPool.Instance.Get());
                    packer = rsp.Pack(new Packer());
                    DebugEx.Log(packer.Data.Length + "      len");
                    break;
            }
            unpacker.Reset();
            UnpackerPool.Instance.Release(unpacker);
            return packer;
        }

        public static void HandleResponse(MessageHeader header, CMD cmd, Unpacker unpacker)
        {
            switch(cmd)
            {
                case CMD.QueryDirectoryInfo:
                    QueryDirectoryInfo.Rsp rsp = new QueryDirectoryInfo.Rsp();
                    rsp.Unpack(unpacker);

                    Output.Dump(rsp, "ffffffffffff");
                    Output.Dump(rsp.Exists, "vvvvvvv1");
                    Output.Dump(rsp.SubDirectories, "vvvvvvv2");
                    Output.Dump(rsp.SubFiles, "vvvvvvv3");
                    break;
            }
            unpacker.Reset();
            UnpackerPool.Instance.Release(unpacker);
        }

        public static CMD GetCMD(Unpacker unpacker)
        {
            return (CMD)unpacker.ReadUInt();
        }
    }
}