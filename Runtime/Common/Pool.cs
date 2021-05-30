using System.Collections.Generic;

namespace URFS
{
    public class Pool<T> where T : new()
    {
        private Stack<T> m_Stack = new Stack<T>();
        
        public int CountAll
        {
            get;
            private set;
        }

        public int CountActive
        {
            get
            {
                return CountAll - m_Stack.Count;
            }
        }

        public int CountInactive
        {
            get 
            {
                return m_Stack.Count;
            }
        }

        public T Get()
        {
            T element;
            if(m_Stack.Count == 0)
            {
                element = new T();
            }
            else
            {
                element = m_Stack.Pop();
            }
            return element;
        }

        public void Release(T element)
        {
            // 避免重复回收相同元素
            if(m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                return;
            m_Stack.Push(element);
        }
    }
}