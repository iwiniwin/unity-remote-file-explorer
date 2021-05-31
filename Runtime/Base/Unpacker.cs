using System;
using System.Text;

namespace URFS
{
    public class Unpacker
    {
        public Octets m_Data = new Octets();

        private int m_Pos = 0;

        public Octets Data
        {
            get
            {
                return m_Data;
            }
        }

        public byte ReadByte()
        {
            ReadDataTypeFlag();
            return InternalReadByte();
        }

        public bool ReadBool()
        {
            ReadDataTypeFlag();
            return InternalReadBool();
        }

        public Int32 ReadInt()
        {
            ReadDataTypeFlag();
            return InternalReadInt();
        }

        public UInt32 ReadUInt()
        {
            ReadDataTypeFlag();
            return InternalReadUInt();
        }

        public string ReadString()
        {
            ReadDataTypeFlag();
            return InternalReadString();
        }

        public byte[] ReadByteArray()
        {
            ReadDataTypeFlag();
            ReadDataTypeFlag();
            Int32 length = InternalReadInt();
            byte[] data = new byte[length];
            Array.Copy(m_Data.Buffer, m_Pos, data, 0, length);
            m_Pos += length;
            return data;
        }

        public Int32[] ReadIntArray()
        {
            ReadDataTypeFlag();
            ReadDataTypeFlag();
            Int32 length = InternalReadInt();
            Int32[] data = new Int32[length];
            for (int i = 0; i < length; i++)
            {
                data[i] = InternalReadInt();
            }
            return data;
        }

        public UInt32[] ReadUIntArray()
        {
            ReadDataTypeFlag();
            ReadDataTypeFlag();
            Int32 length = InternalReadInt();
            UInt32[] data = new UInt32[length];
            for (int i = 0; i < length; i++)
            {
                data[i] = InternalReadUInt();
            }
            return data;
        }

        public string[] ReadStringArray()
        {
            ReadDataTypeFlag();
            ReadDataTypeFlag();
            Int32 length = InternalReadInt();
            string[] data = new string[length];
            for (int i = 0; i < length; i++)
            {
                data[i] = InternalReadString();
            }
            return data;
        }

        public byte InternalReadByte()
        {
            return m_Data.Buffer[m_Pos++];
        }

        public bool InternalReadBool()
        {
            bool b = BitConverter.ToBoolean(m_Data.Buffer, m_Pos);
            m_Pos += sizeof(bool);
            return b;
        }

        public Int32 InternalReadInt()
        {
            Int32 i = BitConverter.ToInt32(m_Data.Buffer, m_Pos);
            m_Pos += sizeof(Int32);
            return i;
        }

        public UInt32 InternalReadUInt()
        {
            UInt32 i = BitConverter.ToUInt32(m_Data.Buffer, m_Pos);
            m_Pos += sizeof(UInt32);
            return i;
        }

        public string InternalReadString()
        {
            Int32 length = InternalReadInt();
            string str = Encoding.UTF8.GetString(m_Data.Buffer, m_Pos, length);
            m_Pos += length;
            return str;
        }

        public void ReadDataTypeFlag()
        {
            ++m_Pos;
        }

        public void Reset() {
            m_Pos = 0;
            m_Data.Clear();
        }
    }
}