using System;
using System.Collections.Generic;

namespace MHW_Save_Editor
{
    //Taken from https://stackoverflow.com/questions/5756692/arraysegment-returning-the-actual-segment-c-sharp/30337982
    //credit goes to digEmAll and JeppeStigNielsen
    public class ArraySegmentWrapper<T> : IList<T>
    {
        private readonly ArraySegment<T> segment;
    
        public ArraySegmentWrapper(ArraySegment<T> segment)
        {
            this.segment = segment;
        }
    
        public ArraySegmentWrapper(T[] array, int offset, int count)
            : this(new ArraySegment<T>(array, offset, count))
        {
        }
    
        public int IndexOf(T item)
        {
            for (int i = segment.Offset; i < segment.Offset + segment.Count; i++)
                if (Equals(segment.Array[i], item))
                    return i;
            return -1;
        }
    
        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }
    
        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }
    
        public T this[int index]
        {
            get
            {
                if (index >= this.Count)
                    throw new IndexOutOfRangeException();
                return this.segment.Array[index + this.segment.Offset];
            }
            set
            {
                if (index >= this.Count)
                    throw new IndexOutOfRangeException();
                this.segment.Array[index + this.segment.Offset] = value;
            }
        }
    
        public void Add(T item)
        {
            throw new NotSupportedException();
        }
    
        public void Clear()
        {
            throw new NotSupportedException();
        }
    
        public bool Contains(T item)
        {
            return this.IndexOf(item) != -1;
        }
    
        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = segment.Offset; i < segment.Offset + segment.Count; i++)
            {
                array[arrayIndex] = segment.Array[i];
                arrayIndex++;
            }
        }

        public void CopyFrom(T[] array, int offset)
        {
            for (int i = segment.Offset+offset; i < segment.Offset + offset + array.Length; i++)
            {
                segment.Array[i] = array[i-(segment.Offset+offset)];
            }            
        }
    
        public int Count
        {
            get { return this.segment.Count; }
        }
    
        public bool IsReadOnly
        {
            get { return false; }
        }
    
        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }
    
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = segment.Offset; i < segment.Offset + segment.Count; i++)
                yield return segment.Array[i];
        }
    
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T[] Slice(int start, int end)
        {
            T[] result = new T[end-start];
            for(int i = start; i<end; i++)result[i]=this[i];
            return result;
        }
    }
}