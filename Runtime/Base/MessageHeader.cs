using System;

namespace URFS
{
    public class MessageHeader
    {
        private static UInt32 uniqueSeq = 0;
        public UInt32 Size;
        private UInt32 m_Seq;
        public UInt32 Ack;

        public UInt32 Seq
        {
            get 
            {
                return m_Seq;
            }
        }

        public static int Length 
        {
            get 
            {
                return sizeof(UInt32) * 3;
            }
        }
        
        public static MessageHeader Create(UInt32 size, UInt32 seq, UInt32 ack)
        {
            return new MessageHeader(size, seq, ack);
        }

        private MessageHeader(UInt32 size, UInt32 seq, UInt32 ack)
        {
            this.Size = size;
            this.m_Seq = seq;
            this.Ack = ack;
        }

        public MessageHeader() : this(0){}

        public MessageHeader(UInt32 size) : this(size, 0) {}

        public MessageHeader(UInt32 size, UInt32 ack)
        {
            this.Size = size;
            this.m_Seq = ++ uniqueSeq;
            this.Ack = ack;
        }

        public byte[] GetBytes()
        {
            byte[] header = new byte[Length];
            var sizeBytes = BitConverter.GetBytes(this.Size);
            Array.Copy(sizeBytes, 0, header, 0, sizeBytes.Length);
            var seqBytes = BitConverter.GetBytes(this.Seq);
            Array.Copy(seqBytes, 0, header, sizeof(UInt32), seqBytes.Length);
            var ackBytes = BitConverter.GetBytes(this.Ack);
            Array.Copy(ackBytes, 0, header, sizeof(UInt32) * 2, ackBytes.Length);
            return header;
        }

        public void Reset()
        {
            this.Size = 0;
            this.m_Seq = ++ uniqueSeq;
            this.Ack = 0;
        }
    }
}