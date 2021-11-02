using System;

namespace RemoteFileExplorer
{
    public class NewFolder
    {
        public class Req : Command
        {
            public string Path;
            public override CommandType Type { get { return CommandType.NewFolderReq; } }

            public override Octets Serialize()
            {
                Octets octets = base.Serialize();
                Packer.Bind(octets);
                Packer.WriteString(Path);
                Packer.Unbind();
                return octets;
            }

            public override int Deserialize(Octets octets)
            {
                Unpacker.Bind(octets, base.Deserialize(octets));
                this.Path = Unpacker.ReadString();
                return Unpacker.Unbind();
            }
        }

        public class Rsp : Command
        {
            public override CommandType Type { get { return CommandType.NewFolderRsp; } }
        }
    }
}