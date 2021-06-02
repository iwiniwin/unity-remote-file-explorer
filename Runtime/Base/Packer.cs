using System;
using System.Text;

namespace URFS
{
    public class Packer
    {
        private static Octets m_Data;

        private Packer() {}

        public static void Bind(Octets octets)
        {
            m_Data = octets;
        }

        public static void Unbind()
        {
            m_Data = null;
        }

        public static byte[] GetBuffer()
        {
            if(m_Data != null)
            {
                return m_Data.Buffer;
            }
            return null;
        }

        public static void WriteByte(byte b)
        {
            m_Data.Push(b);
        }

        public static void WriteBool(bool b) 
        {
            m_Data.Push(BitConverter.GetBytes(b));
        }

        public static void WriteInt(Int32 i)
        {
            m_Data.Push(BitConverter.GetBytes(i));
        }

        public static void WriteUInt(UInt32 i)
        {
            m_Data.Push(BitConverter.GetBytes(i));
        }

        public static void WriteString(string str)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            m_Data.Push(BitConverter.GetBytes(data.Length));
            m_Data.Push(data);
        }

        public static void WriteByteArray(byte[] data)
        {
            m_Data.Push(BitConverter.GetBytes(data.Length));
            m_Data.Push(data);
        }

        public static void WriteIntArray(Int32[] data)
        {
            m_Data.Push(BitConverter.GetBytes(data.Length));
            for (int i = 0; i < data.Length; i++)
            {
                m_Data.Push(BitConverter.GetBytes(data[i]));
            }
        }

        public static void WriteUIntArray(UInt32[] data)
        {
            m_Data.Push(BitConverter.GetBytes(data.Length));
            for (int i = 0; i < data.Length; i++)
            {
                m_Data.Push(BitConverter.GetBytes(data[i]));
            }
        }

        public static void WriteStringArray(string[] data)
        {
            m_Data.Push(BitConverter.GetBytes(data.Length));
            for (int i = 0; i < data.Length; i++)
            {
                m_Data.Push(BitConverter.GetBytes(data[i].Length));
                m_Data.Push(Encoding.UTF8.GetBytes(data[i]));
            }
        }
    }
}