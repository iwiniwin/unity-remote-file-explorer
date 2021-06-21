using System;

namespace RemoteFileExplorer
{
    public class QueryPathInfo
    {
        public class Req : Command
        {
            public string Path;
            public override CommandType Type { get { return CommandType.QueryPathInfo; } }

            public override Octets Serialize()
            {
                Octets octets = new Octets();
                Packer.Bind(octets);
                Packer.WriteString(Path);
                Packer.Unbind();
                return octets;
            }

            public override void Deserialize(Octets octets)
            {
                Unpacker.Bind(octets);
                this.Path = Unpacker.ReadString();
                Unpacker.Unbind();
            }
        }

        public class Rsp : Command
        {
            public bool Exists;
            public bool IsFile;
            public string[] Directories;
            public string[] Files;
            public override CommandType Type { get { return CommandType.QueryPathInfo; } }

            private int m_ReadSize;

            public override Octets Serialize()
            {
                Octets octets = new Octets();
                Packer.Bind(octets);
                Packer.WriteBool(Exists);
                Packer.WriteBool(IsFile);
                Packer.WriteStringArray(Directories);
                Packer.WriteStringArray(Files);
                Packer.Unbind();
                return octets;
            }

            public override void Deserialize(Octets octets)
            {
                Unpacker.Bind(octets);
                this.Exists = Unpacker.ReadBool();
                this.IsFile = Unpacker.ReadBool();
                this.Directories = Unpacker.ReadStringArray();
                this.Files = Unpacker.ReadStringArray();
                m_ReadSize = Unpacker.GetPos();
                Unpacker.Unbind();
            }

            public int ReadSize()
            {
                return m_ReadSize;
            }
        }
    }
}