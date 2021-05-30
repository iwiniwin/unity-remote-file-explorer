using System;

namespace URFS
{
    public class Octets
    {
        private byte[] m_Buffer;
        private int m_Length;
        private int m_Capacity;

        public byte[] Buffer
        {
            get
            {
                return m_Buffer;
            }
        }

        public int Length
        {
            get
            {
                return m_Length;
            }
        }

        public int Capacity
        {
            get
            {
                return m_Capacity;
            }
        }

        public Octets()
        {
            m_Length = 0;
            m_Capacity = 0;
            m_Buffer = null;
        }

        public Octets(int capacity)
        {
            m_Length = 0;
            m_Capacity = capacity;
            m_Buffer = new byte[capacity];
        }

        public Octets(byte[] data, int length)
        {
            m_Length = length;
            m_Capacity = length;
            m_Buffer = new byte[length];
            Array.Copy(data, m_Buffer, length);
        }

        public Octets(byte[] data, int pos, int length)
        {
            m_Length = length;
            m_Capacity = length;
            m_Buffer = new byte[length];
            Array.Copy(data, pos, m_Buffer, 0, length);
        }

        public Octets(Octets o)
        {
            m_Length = o.m_Length;
            m_Capacity = o.m_Capacity;
            m_Buffer = o.m_Buffer;
        }

        public void Push(byte data)
        {
            Resize(m_Length + 1);
            m_Buffer[m_Length++] = data;
        }

        public void Push(byte[] data)
        {
            Push(data, 0, data.Length);
        }

        public void Push(byte[] data, int length)
        {
            Push(data, 0, length);
        }

        public void Push(byte[] data, int pos, int length)
        {
            if (data == null) return;
            Resize(m_Length + length);
            Array.Copy(data, pos, m_Buffer, m_Length, length);
            m_Length += length;
        }

        public void Overwrite(byte data, int pos)
        {
            if (pos < m_Length)
            {
                m_Buffer[pos] = data;
            }
        }

        public void Overwrite(byte[] data, int start)
        {
            Overwrite(data, 0, data.Length, start);
        }

        public void Overwrite(byte[] data, int pos, int length, int start)
        {
            if (data != null && length > 0 && length + start < m_Length)
            {
                Array.Copy(data, pos, m_Buffer, start, length);
            }
        }

        public void Resize(int size)
        {
            if (m_Capacity < size)
            {
                if (m_Buffer == null)
                {
                    m_Buffer = new byte[size];
                }
                else
                {
                    Array.Resize(ref m_Buffer, size);
                }
                m_Capacity = size;
            }
        }

        public void Erase(int pos, int length)
        {
            Array.Copy(m_Buffer, pos + length, m_Buffer, pos, m_Length - length - pos);
            m_Length -= length;
        }

        public void Erase(int pos, int length, out byte[] eraseBytes)
        {
            eraseBytes = new byte[length];
            Array.Copy(m_Buffer, pos, eraseBytes, 0, length);
            Array.Copy(m_Buffer, pos + length, m_Buffer, pos, m_Length - length - pos);
            m_Length -= length;
        }

        public void Clear()
        {
            m_Length = 0;
        }
    }

}