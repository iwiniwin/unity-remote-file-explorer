using System;
using System.Text;

namespace URFS
{
    public class Unpacker
    {
        private static int m_Pos = 0;

        private static byte[] m_Data;

        private Unpacker() {}

        public static void Bind(Octets octets)
        {
            Bind(octets.Buffer, 0);
        }

        public static void Bind(byte[] data)
        {
            m_Data = data;
            m_Pos = 0;
        }

        public static void Bind(Octets octets, int pos)
        {
            Bind(octets.Buffer, pos);
        }

        public static void Bind(byte[] data, int pos)
        {
            m_Data = data;
            m_Pos = pos;
        }

        public static void Unbind()
        {
            m_Data = null;
            m_Pos = 0;
        }

        public static byte ReadByte()
        {
            return m_Data[m_Pos++];
        }

        public static bool ReadBool()
        {
            bool b = BitConverter.ToBoolean(m_Data, m_Pos);
            m_Pos += sizeof(bool);
            return b;
        }

        public static Int32 ReadInt()
        {
            Int32 i = BitConverter.ToInt32(m_Data, m_Pos);
            m_Pos += sizeof(Int32);
            return i;
        }

        public static UInt32 ReadUInt()
        {
            UInt32 i = BitConverter.ToUInt32(m_Data, m_Pos);
            m_Pos += sizeof(UInt32);
            return i;
        }

        public static string ReadString()
        {
            Int32 length = ReadInt();
            string str = Encoding.UTF8.GetString(m_Data, m_Pos, length);
            m_Pos += length;
            return str;
        }

        public static byte[] ReadByteArray()
        {
            Int32 length = ReadInt();
            byte[] data = new byte[length];
            Array.Copy(m_Data, m_Pos, data, 0, length);
            m_Pos += length;
            return data;
        }

        public static Int32[] ReadIntArray()
        {
            Int32 length = ReadInt();
            Int32[] data = new Int32[length];
            for (int i = 0; i < length; i++)
            {
                data[i] = ReadInt();
            }
            return data;
        }

        public static UInt32[] ReadUIntArray()
        {
            Int32 length = ReadInt();
            UInt32[] data = new UInt32[length];
            for (int i = 0; i < length; i++)
            {
                data[i] = ReadUInt();
            }
            return data;
        }

        public static string[] ReadStringArray()
        {
            Int32 length = ReadInt();
            string[] data = new string[length];
            for (int i = 0; i < length; i++)
            {
                data[i] = ReadString();
            }
            return data;
        }
    }
}