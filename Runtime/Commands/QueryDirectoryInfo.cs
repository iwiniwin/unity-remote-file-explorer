using System;

namespace RemoteFileExplorer
{
    public class QueryDirectoryInfo 
    {
        public class Req : Message
        {
            public string Directory;

            public override Package Pack()
            {
                Package package = new Package();
                package.Head = new PackageHead();
                package.Head.Type = CommandType.QueryDirectoryInfo.ToUInt();
                package.Body = new Octets();
                Packer.Bind(package.Body);
                Packer.WriteString(Directory);
                package.Head.Size = (uint)(package.Body.Length + PackageHead.Length);
                Packer.Unbind();
                return package;
            }

            public override void Unpack(Package package)
            {
                Unpacker.Bind(package.Body);
                this.Directory = Unpacker.ReadString();
                Unpacker.Unbind();
            }
        }

        public class Rsp : Message
        {
            public UInt32 Ack;
            public bool Exists;
            public string[] SubDirectories;
            public string[] SubFiles;

            public override Package Pack()
            {
                Package package = new Package();
                package.Head = new PackageHead();
                package.Head.Ack = this.Ack;
                package.Head.Type = CommandType.QueryDirectoryInfo.ToUInt();
                package.Body = new Octets();
                Packer.Bind(package.Body);
                Packer.WriteBool(Exists);
                Packer.WriteStringArray(SubDirectories);
                Packer.WriteStringArray(SubFiles);
                package.Head.Size = (uint)(package.Body.Length + PackageHead.Length);
                Packer.Unbind();
                return package;
            }

            public override void Unpack(Package package)
            {
                Unpacker.Bind(package.Body);
                this.Exists = Unpacker.ReadBool();
                this.SubDirectories = Unpacker.ReadStringArray();
                this.SubFiles = Unpacker.ReadStringArray();
                Unpacker.Unbind();
            }
        }
    }
}