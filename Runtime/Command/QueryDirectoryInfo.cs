using System;

namespace URFS
{
    public class QueryDirectoryInfo 
    {
        public class Req : Message
        {
            public string Directory;
            public CMD CMD;

            public Req()
            {
            }

            public Req(string directory)
            {
                Directory = directory;
            }

            public override Packer Pack(Packer packer)
            {
                BeginPack(packer);
                packer.WriteUInt(CMD.QueryDirectoryInfo.ToUInt());
                packer.WriteString(Directory);
                EndPack(packer);
                return packer;
            }

            public override Unpacker Unpack(Unpacker unpacker)
            {
                unpacker.ReadHeader();
                this.CMD = (CMD)unpacker.ReadUInt();
                this.Directory = unpacker.ReadString();
                return unpacker;
            }
        }

        public class Rsp : Message
        {
            public UInt32 Ack;
            public bool Exists;
            public string[] SubDirectories;
            public string[] SubFiles;
            public Rsp(UInt32 ack, bool exists, string[] subDirectories, string[] subFiles)
            {
                Header.Ack = ack; 
                this.Exists = exists;
                this.SubDirectories = subDirectories;
                this.SubFiles = subFiles;
            }
            public override Packer Pack(Packer packer)
            {
                BeginPack(packer);
                packer.WriteUInt(CMD.QueryDirectoryInfo.ToUInt());
                packer.WriteBool(Exists);
                packer.WriteStringArray(SubDirectories);
                packer.WriteStringArray(SubFiles);
                EndPack(packer);
                return packer;
            }

            public override Unpacker Unpack(Unpacker unpacker)
            {
                MessageHeader header = unpacker.ReadHeader();
                this.Ack = header.Ack;
                this.Exists = unpacker.ReadBool();
                this.SubDirectories = unpacker.ReadStringArray();
                this.SubFiles = unpacker.ReadStringArray();
                return unpacker;
            }
        }
    }
}