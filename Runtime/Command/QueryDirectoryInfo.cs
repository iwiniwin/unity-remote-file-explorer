using System;

namespace URFS
{
    public class QueryDirectoryInfo 
    {
        public class Req : Message
        {
            public string Directory;

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

            public override Packer Pack(Packer packer)
            {
                BeginPack(packer);
                this.Header.Ack = this.Ack;
                packer.WriteUInt(CMD.QueryDirectoryInfo.ToUInt());
                packer.WriteBool(Exists);
                packer.WriteStringArray(SubDirectories);
                packer.WriteStringArray(SubFiles);
                EndPack(packer);
                return packer;
            }

            public override Unpacker Unpack(Unpacker unpacker)
            {
                this.Exists = unpacker.ReadBool();
                this.SubDirectories = unpacker.ReadStringArray();
                this.SubFiles = unpacker.ReadStringArray();
                return unpacker;
            }
        }
    }
}