using System;

namespace RemoteFileExplorer
{
    public class TransferFile
    {

        public class Req : Command
        {
            public string ShortName;
            public byte[] Content;
            public override CommandType Type { get { return CommandType.TransferFileRsp; } }

            public override Octets Serialize()
            {
                Octets octets = base.Serialize();
                Packer.Bind(octets);
                Packer.WriteString(ShortName);
                Packer.WriteByteArray(Content);
                Packer.Unbind();
                return octets;
            }

            public override int Deserialize(Octets octets)
            {
                Unpacker.Bind(octets, base.Deserialize(octets));
                this.ShortName = Unpacker.ReadString();
                this.Content = Unpacker.ReadByteArray();
                return Unpacker.Unbind();
            }
        }

        public class Rsp : Command
        {
            
        }
    }
}