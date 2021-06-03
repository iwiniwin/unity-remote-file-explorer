using System.IO;
using UDK;

namespace URFS
{
    public class CommandHandler
    {
        private static string[] emptyStringArray = new string[]{};

        public static Package Handle(Package package)
        {
            if(package.Head.Ack == 0)
            {
                return HandleRequest(package);
            }
            else
            {
                HandleResponse(package);
            }
            return null;
        }

        public static Package HandleRequest(Package package)
        {
            Package response = null;
            CommandType cmd = (CommandType)package.Head.Type;
            switch(cmd)
            {
                case CommandType.QueryDirectoryInfo:
                    QueryDirectoryInfo.Req req = new QueryDirectoryInfo.Req();
                    req.Unpack(package);

                    bool exists = Directory.Exists(req.Directory);
                    QueryDirectoryInfo.Rsp rsp = new QueryDirectoryInfo.Rsp{
                        Ack = package.Head.Seq,
                        Exists = exists,
                        SubDirectories = exists ? Directory.GetDirectories(req.Directory) : emptyStringArray,
                        SubFiles = exists ? Directory.GetFiles(req.Directory) : emptyStringArray,
                    };
                    response = rsp.Pack();
                    break;
            }
            return response;
        }

        public static void HandleResponse(Package package)
        {
            CommandType cmd = (CommandType)package.Head.Type;
            switch(cmd)
            {
                case CommandType.QueryDirectoryInfo:
                    QueryDirectoryInfo.Rsp rsp = new QueryDirectoryInfo.Rsp();
                    rsp.Unpack(package);
                    Output.Dump(rsp.Exists);
                    Output.Dump(rsp.SubDirectories);
                    Output.Dump(rsp.SubFiles);
                    break;
            }
        }
    }
}