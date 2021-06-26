using System;

namespace RemoteFileExplorer
{
    public class CreateDirectory
    {
        public class Req : Command
        {
            public string[] Directories;
            public override CommandType Type { get { return CommandType.CreateDirectoryReq; } }

            public override Octets Serialize()
            {
                Octets octets = base.Serialize();
                Packer.Bind(octets);
                Packer.WriteStringArray(Directories);
                Packer.Unbind();
                return octets;
            }

            public override int Deserialize(Octets octets)
            {
                Unpacker.Bind(octets, base.Deserialize(octets));
                this.Directories = Unpacker.ReadStringArray();
                return Unpacker.Unbind();
            }
        }

        public class Rsp : Command
        {
            public override CommandType Type { get { return CommandType.CreateDirectoryRsp; } }
        }
    }
}