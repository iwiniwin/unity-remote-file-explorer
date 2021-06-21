using System;

namespace RemoteFileExplorer
{
    public class QueryPathKeyInfo 
    {
        public class Req : Command
        {
            public string PathKey;
            public override CommandType Type { get { return CommandType.QueryPathKeyInfo; } }

            public override Octets Serialize()
            {
                Octets octets = new Octets();
                Packer.Bind(octets);
                Packer.WriteString(PathKey);
                Packer.Unbind();
                return octets;
            }

            public override void Deserialize(Octets octets)
            {
                Unpacker.Bind(octets);
                this.PathKey = Unpacker.ReadString();
                Unpacker.Unbind();
            }
        }

        public class Rsp : QueryPathInfo.Rsp
        {
            public string Path;
            public override CommandType Type { get { return CommandType.QueryPathKeyInfo; } }

            public override Octets Serialize()
            {
                Octets octets = base.Serialize();
                Packer.Bind(octets);
                Packer.WriteString(Path);
                Packer.Unbind();
                return octets;
            }

            public override void Deserialize(Octets octets)
            {
                base.Deserialize(octets);
                Unpacker.Bind(octets, base.ReadSize());
                this.Path = Unpacker.ReadString();
                Unpacker.Unbind();
            }
        }
    }
}