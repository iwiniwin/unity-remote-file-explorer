using System;

namespace RemoteFileExplorer
{
    public class QueryPathInfo 
    {
        public class Req : Command
        {
            public string Path;

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
            public string[] SubDirectories;
            public string[] SubFiles;

            public override Octets Serialize()
            {
                Octets octets = new Octets();
                Packer.Bind(octets);
                Packer.WriteBool(Exists);
                Packer.WriteStringArray(SubDirectories);
                Packer.WriteStringArray(SubFiles);
                Packer.Unbind();
                return octets;
            }

            public override void Deserialize(Octets octets)
            {
                Unpacker.Bind(octets);
                this.Exists = Unpacker.ReadBool();
                this.SubDirectories = Unpacker.ReadStringArray();
                this.SubFiles = Unpacker.ReadStringArray();
                Unpacker.Unbind();
            }
        }
    }
}