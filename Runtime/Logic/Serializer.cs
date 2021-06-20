namespace RemoteFileExplorer
{
    public class Serializer
    {
        public static Package Serialize(Command command)
        {
            Package package = new Package();
            package.Head = new PackageHead();
            package.Head.Type = CommandType.QueryPathInfo.ToUInt();
            package.Head.Ack = command.Ack;
            package.Body = command.Serialize();
            package.Head.Size = (uint)(package.Body.Length + PackageHead.Length);
            return package;
        }

        public static Command Deserialize(Package package)
        {
            Command command = null;
            
            if(package.Head.Type == CommandType.QueryPathInfo.ToUInt())
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