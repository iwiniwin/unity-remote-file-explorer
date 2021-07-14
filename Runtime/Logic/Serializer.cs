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
                case CommandType.PullReq:
                    command = new Pull.Req();
                    break;
                case CommandType.PullRsp:
                    command = new Pull.Rsp();
                    break;
                case CommandType.TransferFileReq:
                    command = new TransferFile.Req();
                    break;
                case CommandType.TransferFileRsp:
                    command = new TransferFile.Rsp();
                    break;
                case CommandType.CreateDirectoryReq:
                    command = new CreateDirectory.Req();
                    break;
                case CommandType.CreateDirectoryRsp:
                    command = new CreateDirectory.Rsp();
                    break;
                case CommandType.DeleteReq:
                    command = new Delete.Req();
                    break;
                case CommandType.DeleteRsp:
                    command = new Delete.Rsp();
                    break;
                case CommandType.RenameReq:
                    command = new Rename.Req();
                    break;
                case CommandType.RenameRsp:
                    command = new Rename.Rsp();
                    break;
                case CommandType.QueryDeviceInfoReq:
                    command = new QueryDeviceInfo.Req();
                    break;
                case CommandType.QueryDeviceInfoRsp:
                    command = new QueryDeviceInfo.Rsp();
                    break;
            }
            if(command != null)
            {
                command.Deserialize(package.Body);
                OctetsCache.Instance.Release(package.Body);
            }
            else
            {
                Log.Error("receive unknown command : " + package.Head.Type);
            }
            return command;
        }
    }
}