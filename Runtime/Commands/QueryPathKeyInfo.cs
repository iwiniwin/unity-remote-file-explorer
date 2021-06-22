using System;

namespace RemoteFileExplorer
{
    public class QueryPathKeyInfo 
    {
        public class Req : Command
        {
            public string PathKey;
            public override CommandType Type { get { return CommandType.QueryPathKeyInfoReq; } }

            public override Octets Serialize()
            {
                Octets octets = base.Serialize();
                Packer.Bind(octets);
                Packer.WriteString(PathKey);
                Packer.Unbind();
                return octets;
            }

            public override int Deserialize(Octets octets)
            {
                Unpacker.Bind(octets, base.Deserialize(octets));
                this.PathKey = Unpacker.ReadString();
                return Unpacker.Unbind();
            }
        }

        public class Rsp : QueryPathInfo.Rsp
        {
            public string Path;
            public override CommandType Type { get { return CommandType.QueryPathKeyInfoRsp; } }

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
    }
}