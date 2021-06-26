using System;

namespace RemoteFileExplorer
{
    public class Rename
    {
        public class Req : Command
        {
            public string Path;
            public string NewPath;
            public override CommandType Type { get { return CommandType.RenameReq; } }

            public override Octets Serialize()
            {
                Octets octets = base.Serialize();
                Packer.Bind(octets);
                Packer.WriteString(Path);
                Packer.WriteString(NewPath);
                Packer.Unbind();
                return octets;
            }

            public override int Deserialize(Octets octets)
            {
                Unpacker.Bind(octets, base.Deserialize(octets));
                this.Path = Unpacker.ReadString();
                this.NewPath = Unpacker.ReadString();
                return Unpacker.Unbind();
            }
        }

        public class Rsp : Command
        {
            public override CommandType Type { get { return CommandType.RenameRsp; } }
        }
    }
}