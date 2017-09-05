using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace UNEB.Collections
{
    /// <summary>
    /// Mixed Type Serializable List which attempts to preserve original types of elements stored in the list when serialized and deserialized by unity. 
    /// Unity by cannot serialized mixed types by default (fallback to base type).
    /// </summary>
    /// <remarks>
    /// (NOTE: Ensure objects added to list are serializable).
    /// </remarks>
    [Serializable]
    public class MixedTypeSerializableList : 
        MixedTypeSerializableList<object>
    {

    }



    /// <summary>
    /// Mixed Type Serializable List which attempts to preserve original types of elements stored in the list when serialized and deserialized by unity. 
    /// Unity by cannot serialized mixed types by default (fallback to base type).
    /// </summary>
    public abstract class MixedTypeSerializableList<T> : 
        Internal.SerializableObject, 
        IList<T>
    {
        const int DEFAULT_CAPACITY = 100;
        
        T[] m_Items = new T[DEFAULT_CAPACITY];

        [SerializeField]
        int m_Count = 0;

        /// <summary>
        /// Indexes of deleted items.
        /// </summary>
        Queue<int> m_EmptyIndices = new Queue<int>();

 
        protected MixedTypeSerializableList()
        {
            OnSerializeInternal   += DoOnSerialize  ;
            OnDeserializeInternal += DoOnDeserialize;
        }

        private void DoOnDeserialize()
        {
            m_EmptyIndices.Clear();
            m_TempObjects.Clear();
            m_TempObjects.Deserialize_PerItem(m_SerializedTypes, m_SerializedValues, false);
            Array.Resize(ref m_Items, m_TempObjects.Count);
            m_TempObjects.CopyTo(m_Items);
            m_Count = m_TempObjects.Count;
        }

        private void DoOnSerialize()
        {
            Clean();
            m_TempObjects.Clear();
            m_TempObjects.AddRange(m_Items);
            m_TempObjects.Serialize_PerItem(ref m_SerializedTypes, ref m_SerializedValues);
            m_Count = m_TempObjects.Count;
        }


        int GetNextIndex()
        {
            if(m_EmptyIndices.Count > 0) 
                return m_EmptyIndices.Dequeue(); 
            if(m_Count + 1 >= m_Items.Length) 
                Array.Resize(ref m_Items, m_Items.Length + 100);
            m_Count++;
            return m_Count;
        }




        #region Rebuild
         
        bool m_IsDirty;

        /// <summary>
        /// Returns true if this collection needs to be rebuilt.
        /// </summary>
        public bool IsDirty => m_IsDirty || m_EmptyIndices.Count > 0;

        /// <summary>
        /// Rebuilds the internal array.
        /// </summary>
        void Clean()
        {
            if(IsDirty)
            {
                m_TempObjects.Clear();
                m_TempObjects.AddRange(m_Items);
                m_TempObjects.PurgeNullEntries();
                m_EmptyIndices.Clear();
                m_Count = m_TempObjects.Count;
                m_TempObjects.CopyTo(m_Items, 0);
                m_IsDirty = false;
            }
        }

        #endregion


        #region Serialization (per item mixed type)

        [SerializeField, HideInInspector]
        string[] m_SerializedTypes, m_SerializedValues;
        static List<T> m_TempObjects = new List<T>();

        #endregion


        #region Utility

        void ThrowIfOutOfRange(int index)
        {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException();
        }

        #endregion

        #region LIST IMPLEMENTATION



        public int Count => m_Count;

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get
            {
                Clean();
                ThrowIfOutOfRange(index);
                return m_Items[index];
            }
            set
            {
                Clean();
                ThrowIfOutOfRange(index);
                m_Items[index] = value;
            }
        }

        /// <summary>
        /// Returns indec of specified item. If item does not exist in list, -1 is returned instead.
        /// </summary>
        public int IndexOf(T item)
        {
            Clean();
            T current;
            for (int i = 0; i < m_Items.Length; i++)
            {
                current = m_Items[i];
                if (current == null) continue;
                if (Equals(current, item) || ReferenceEquals(current, item))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// This is not supported in this type of collection.
        /// </summary> 
        /// <exception cref="NotSupportedException" />
        public void Insert(int index, T item)
        {
            throw new NotSupportedException
            (
                string.Format("Collections of type '{0}' does not support items via 'Insert'", typeof(MixedTypeSerializableList<T>))
            );
        }
 
        public void RemoveAt(int index)
        {

            ThrowIfOutOfRange(index); 
            if (!m_EmptyIndices.Contains(index))
            {
                m_EmptyIndices.Enqueue(index);
                m_IsDirty = true;
                m_Items[index] = default(T);
            }
        }

        public void Add(T item)
        {
            int i = GetNextIndex();
            m_Items[i] = item;
            m_IsDirty = true;
        }

        public void Clear()
        {
            m_IsDirty = false;
            m_Count = 0;
            m_EmptyIndices.Clear();
            Array.Resize(ref m_Items, DEFAULT_CAPACITY);
            for (int i = 0; i < DEFAULT_CAPACITY; i++)
                m_Items[i] = default(T);
            Array.Resize(ref m_SerializedValues, 0);
            Array.Resize(ref m_SerializedTypes, 0);
        }

        public bool Contains(T item)
        {
            return m_Count > 0 && IndexOf(item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Clean();
            m_Items.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            if (Count < 1) return false;
            int i = IndexOf(item);
            if(i >= 0 && !m_EmptyIndices.Contains(i))
            {
                m_Items[i] = default(T);
                m_EmptyIndices.Enqueue(i);
                m_IsDirty = true;
                return true;
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            Clean();
            return m_Items.AsEnumerable<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
