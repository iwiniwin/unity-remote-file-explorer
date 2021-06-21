namespace RemoteFileExplorer
{
    public class Serializer
    {
        public static Package Serialize(Command command)
        {
            Package package = new Package();
            package.Head = new PackageHead();
            package.Head.Type = command.Type.ToUInt();
            package.Head.Ack = command.Ack;
            package.Body = command.Serialize();
            package.Head.Size = (uint)(package.Body.Length + PackageHead.Length);
            return package;
        }

        public static Command Deserialize(Package package)
        {
            Command command = null;
            CommandType type = (CommandType)package.Head.Type;
            if(type == CommandType.QueryPathInfo)
            {
                if(package.Head.Ack == 0)
                {
                    command = new QueryPathInfo.Req();
                }
                else
                {
                    command = new QueryPathInfo.Rsp();
                }
            }
            else if(type == CommandType.QueryPathKeyInfo)
            {
                if(package.Head.Ack == 0)
                {
                    command = new QueryPathKeyInfo.Req();
                }
                else
                {
                    command = new QueryPathKeyInfo.Rsp();
                }
            }
            if(command != null)
            {
                command.Deserialize(package.Body);
                command.Seq = package.Head.Seq;
                command.Ack = package.Head.Ack;
            }
            return command;
        }
    }
}