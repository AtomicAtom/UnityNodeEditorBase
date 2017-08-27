using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Nodes.Core
{
    /// <summary>
    /// baseclass for anything that can be referenced and have references to it restored via a <see cref="Reference"/> type, including nodes/connections/etc;
    /// </summary>
    public abstract class ReferencedType : System.IDisposable
    {

        string m_ID = System.Guid.NewGuid().ToString();

        /// <summary>
        /// All <see cref="ReferencedType"/> objects in memory. This includes all graphs, nodes, connections - etc. Used only to help resolve references
        /// after a Graph is serialized.
        /// </summary>
        static List<ReferencedType> m_AllInMemory = new List<ReferencedType>();

        /// <summary>
        /// Memory items are cast to  array for  faster iteration when needed;
        /// </summary>
        static ReferencedType[] m_AllInMemoryAsArray;
        static bool m_AllInMemoryModified;
        static int  m_ObjectCount;


        bool
            m_IsDestroyed,
            m_IsDestroying;


#region Events

        protected event Action
            OnCreate,
            OnDestroy;

#endregion


#region Properties

        /// <summary>
        /// The Globally unique ID of this object. This is used to resolved references after deserialization;
        /// </summary>
        public string GUID
        {
            get
            { 
                return m_ID;
            }
        }

 
        public bool IsDestroyed
        {
            get
            {
                return m_IsDestroyed || m_IsDestroying;
            }
        }

        #endregion

#region Utility

        internal static T CreateInstance<T>() where T : ReferencedType
        {
            T result = System.Activator.CreateInstance<T>();
            result.Start();
            if (result.OnCreate != null)
                result.OnCreate.TryInvoke();
            m_AllInMemory.Add(result);
            m_AllInMemoryModified = true;
            return result;
        }

        /// <summary>
        /// Used to resolve a reference to a <see cref="ReferencedType"/> object via a GUID.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        internal static bool ResolveReference<T>(out T result, string guid) where T : ReferencedType
        {
            UpdateMemoryArrayIfModified();
            ReferencedType curr;
            result = null;
            for(int i = 0; i < m_ObjectCount; i++)
            {
                curr = m_AllInMemoryAsArray[i];
                if((curr is T || curr.GetType().IsSubclassOf(typeof(T))) && curr.GUID == guid)
                {
                    result = (T)curr;
                    return true;
                }
            }
            return false;
        }




        /// <summary>
        /// Updates a memory array of all objects.
        /// </summary>
        static void UpdateMemoryArrayIfModified()
        {
            // Resize a single static array: 
            // - So we dont allocate a new array every time
            // - Because iterating arrays is faster than iterating lists - especially if we have an elefant load of objects.
            if(m_AllInMemoryModified || m_AllInMemoryAsArray == null)
            {
                m_ObjectCount = m_AllInMemory.Count;
                if (m_AllInMemoryAsArray == null)
                    m_AllInMemoryAsArray = new ReferencedType[m_ObjectCount];
                if (m_AllInMemoryAsArray.Length < m_ObjectCount)
                    Array.Resize(ref m_AllInMemoryAsArray, m_ObjectCount);
                m_AllInMemory.CopyTo(m_AllInMemoryAsArray);
                m_AllInMemoryModified = false;
            }
        }

#endregion

#region Overridables

        protected virtual void Start()
        {

        }

#endregion


#region Methods
        public void Destroy()
        {
            ((IDisposable)this).Dispose();
        }
#endregion


        void IDisposable.Dispose()
        {
            if (IsDestroyed) return;
            m_IsDestroying = true;
            OnDestroy.TryInvoke();
            m_IsDestroyed = true;
            m_AllInMemory.Remove(this);
            m_AllInMemoryModified = true;
            GC.SuppressFinalize(this);
        }






        #region Operators

        /// <summary>
        /// Allows these objects to be cast as bool like unity objects.
        /// </summary>
        /// <param name="type"></param>
        public static implicit operator bool(ReferencedType type)
        {
            return type != null && !type.IsDestroyed;
        }

        #endregion
    }


}
