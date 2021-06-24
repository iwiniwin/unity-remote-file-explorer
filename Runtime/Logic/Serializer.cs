using UnityEngine;

namespace RemoteFileExplorer
{
    public class Serializer
    {
        public static Package Serialize(Command command)
        {
            Package package = new Package();
            package.Head = new Package.Header();
            package.Head.Type = command.Type.ToUInt();
            package.Body = command.Serialize();
            package.Head.Size = (uint)(package.Body.Length + Package.Header.Length);
            return package;
        }

        public static Command Deserialize(Package package)
        {
            Command command = null;
            CommandType type = (CommandType)package.Head.Type;
            switch(type)
            {
                case CommandType.QueryPathInfoReq:
                    command = new QueryPathInfo.Req();
                    break;
                case CommandType.QueryPathInfoRsp:
                    command = new QueryPathInfo.Rsp();
                    break;
                case CommandType.QueryPathKeyInfoReq:
                    command = new QueryPathKeyInfo.Req();
                    break;
                case CommandType.QueryPathKeyInfoRsp:
                    command = new QueryPathKeyInfo.Rsp();
                    break;
                case CommandType.DownloadReq:
                    command = new Download.Req();
                    break;
                case CommandType.DownloadRsp:
                    command = new Download.Rsp();
                    break;
                case CommandType.TransferFileReq:
                    command = new TransferFile.Req();
                    break;
                case CommandType.TransferFileRsp:
                    command = new TransferFile.Rsp();
                    break;
            }
            if(command != null)
            {
                command.Deserialize(package.Body);
            }
            else
            {
                Debug.LogError("receive unknown command : " + package.Head.Type);
            }
            return command;
        }
    }
}