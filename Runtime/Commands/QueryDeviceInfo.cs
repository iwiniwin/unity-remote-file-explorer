using System;

namespace RemoteFileExplorer
{
    public class QueryDeviceInfo
    {
        public class Req : Command
        {
            public override CommandType Type { get { return CommandType.QueryDeviceInfoReq; } }
        }

        public class Rsp : Command
        {
            public string Name;
            public string Model;
            public string System;
            public override CommandType Type { get { return CommandType.QueryDeviceInfoRsp; } }

            public override Octets Serialize()
            {
                Octets octets = base.Serialize();
                Packer.Bind(octets);
                Packer.WriteString(Name);
                Packer.WriteString(Model);
                Packer.WriteString(System);
                Packer.Unbind();
                return octets;
            }

            public override int Deserialize(Octets octets)
            {
                Unpacker.Bind(octets, base.Deserialize(octets));
                this.Name = Unpacker.ReadString();
                this.Model = Unpacker.ReadString();
                this.System = Unpacker.ReadString();
                return Unpacker.Unbind();
            }
        }
    }
}