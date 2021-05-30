using System;

namespace URFS
{
    public class QueryDirectoryInfo 
    {
        public class Req : Message
        {
            public string Directory;
            public CMD CMD;
            public Req(string directory)
            {
                Directory = directory;
            }

            public override void Pack(MessagePacker packer)
            {
                packer.Begin();
                packer.WriteUInt(CMD.QueryDirectoryInfo.ToUInt());
                packer.WriteString(Directory);
                packer.End();
            }

            public override void Unpack(MessageUnpacker unpacker)
            {
                unpacker.ReadHeader();
                this.CMD = (CMD)unpacker.ReadUInt();
                this.Directory = unpacker.ReadString();
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
                this.Ack = ack; 
                this.Exists = exists;
                this.SubDirectories = subDirectories;
                this.SubFiles = subFiles;
            }
            public override void Pack(MessagePacker packer)
            {
                packer.Begin();
                packer.Header.Ack = this.Ack;
                packer.WriteBool(Exists);
                packer.WriteStringArray(SubDirectories);
                packer.WriteStringArray(SubFiles);
                packer.End();
            }

            public override void Unpack(MessageUnpacker unpacker)
            {
                MessageHeader header = unpacker.ReadHeader();
                this.Ack = header.Ack;
                this.Exists = unpacker.ReadBool();
                this.SubDirectories = unpacker.ReadStringArray();
                this.SubFiles = unpacker.ReadStringArray();
            }
        }
    }
}