using System;
using System.Text;

namespace URFS
{
    public class MessagePacker
    {
        public Octets m_Data = new Octets();
        private MessageHeader m_Header = new MessageHeader();

        public Octets Data
        {
            get
            {
                return m_Data;
            }
        }

        public MessageHeader Header 
        {
            get
            {
                return m_Header;
            }
        }

        public void Begin()
        {
            m_Data.Push(m_Header.GetBytes());
        }

        public void End() 
        {
            m_Header.Size = (UInt32)m_Data.Length;
            m_Data.Overwrite(m_Header.GetBytes(), 0);
        }

        public void WriteByte(byte b)
        {
            m_Data.Push(DataType.Byte.ToByte());
            m_Data.Push(b);
        }

        public void WriteBool(bool b) 
        {
            m_Data.Push(DataType.Bool.ToByte());
            m_Data.Push(BitConverter.GetBytes(b));
        }

        public void WriteInt(Int32 i)
        {
            m_Data.Push(DataType.Int.ToByte());
            m_Data.Push(BitConverter.GetBytes(i));
        }

        public void WriteUInt(UInt32 i)
        {
            m_Data.Push(DataType.UInt.ToByte());
            m_Data.Push(BitConverter.GetBytes(i));
        }

        public void WriteString(string str)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            m_Data.Push(DataType.String.ToByte());
            m_Data.Push(BitConverter.GetBytes(data.Length));
            m_Data.Push(data);
        }

        public void WriteByteArray(byte[] data)
        {
            m_Data.Push(DataType.Array.ToByte());
            m_Data.Push(DataType.Byte.ToByte());
            m_Data.Push(BitConverter.GetBytes(data.Length));
            m_Data.Push(data);
        }

        public void WriteIntArray(Int32[] data)
        {
            m_Data.Push(DataType.Array.ToByte());
            m_Data.Push(DataType.Int.ToByte());
            m_Data.Push(BitConverter.GetBytes(data.Length));
            for (int i = 0; i < data.Length; i++)
            {
                m_Data.Push(BitConverter.GetBytes(data[i]));
            }
        }

        public void WriteUIntArray(UInt32[] data)
        {
            m_Data.Push(DataType.Array.ToByte());
            m_Data.Push(DataType.UInt.ToByte());
            m_Data.Push(BitConverter.GetBytes(data.Length));
            for (int i = 0; i < data.Length; i++)
            {
                m_Data.Push(BitConverter.GetBytes(data[i]));
            }
        }

        public void WriteStringArray(string[] data)
        {
            m_Data.Push(DataType.Array.ToByte());
            m_Data.Push(DataType.String.ToByte());
            m_Data.Push(BitConverter.GetBytes(data.Length));
            for (int i = 0; i < data.Length; i++)
            {
                m_Data.Push(Encoding.UTF8.GetBytes(data[i]));
            }
        }

        public void Reset() {
            m_Data.Clear();
            m_Header.Reset();
        }
    }
}