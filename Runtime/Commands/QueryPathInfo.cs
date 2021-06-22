using System;

namespace RemoteFileExplorer
{
    public class QueryPathInfo
    {
        public class Req : Command
        {
            public string Path;
            public override CommandType Type { get { return CommandType.QueryPathInfoReq; } }

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
            public bool Exists;
            public bool IsFile;
            public string[] Directories;
            public string[] Files;
            public string Error;
            public override CommandType Type { get { return CommandType.QueryPathInfoRsp; } }

            private int m_ReadSize;

            public override Octets Serialize()
            {
                Octets octets = base.Serialize();
                Packer.Bind(octets);
                Packer.WriteBool(Exists);
                Packer.WriteBool(IsFile);
                Packer.WriteStringArray(Directories);
                Packer.WriteStringArray(Files);
                Packer.WriteString(Error);
                Packer.Unbind();
                return octets;
            }

            public override int Deserialize(Octets octets)
            {
                Unpacker.Bind(octets, base.Deserialize(octets));
                this.Exists = Unpacker.ReadBool();
                this.IsFile = Unpacker.ReadBool();
                this.Directories = Unpacker.ReadStringArray();
                this.Files = Unpacker.ReadStringArray();
                this.Error = Unpacker.ReadString();
                return Unpacker.Unbind();
            }
        }
    }
}