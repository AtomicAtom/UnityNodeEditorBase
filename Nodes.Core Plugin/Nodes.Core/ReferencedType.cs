using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace UNEB
{
    /// <summary>
    /// Baseclass for serializable UNEB objects which supported preserveing references via JSON serializer.
    /// </summary>
    public abstract class Object : 
        System.IDisposable , 
        ISerializationCallbackReceiver
    {

        string m_ID = System.Guid.NewGuid().ToString();

        /// <summary>
        /// All <see cref="Object"/> objects in memory. This includes all graphs, nodes, connections - etc. Used only to help resolve references
        /// after a Graph is serialized.
        /// </summary>
        static List<Object> m_AllInMemory = new List<Object>();

        /// <summary>
        /// Memory items are cast to  array for  faster iteration when needed and are cached;
        /// </summary>
        static Object[] m_AllInMemoryAsArray;
        static bool m_AllInMemoryModified;
        static int  m_ObjectCount;


        bool
            m_IsDestroyed,
            m_IsDestroying;


        /// <summary>
        /// Cached array of all known types that inherit from <see cref="Object"/>.
        /// </summary>
        static Type[] m_AllInheritedReferencedTypes;

#region Events

        protected event Action 
            OnDestroy,
            
            // Events which can be used when custom serialization logic is required
            OnBeforeSerialization,
            OnAfterDeserialization;

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

     
        /// <summary>
        /// Returns an array of all known types which inherit from <see cref="Object"/>, not including abstract types.
        /// This includes types in all loaded .NET/Mono assemblies, including those defined by scripts in the unity assets folder.
        /// </summary>
        public static Type[] AllInheritingTypes
        {
            get
            {
                if (m_AllInheritedReferencedTypes == null)
                    m_AllInheritedReferencedTypes = ReferencedTypeSerializationHelper.GetAllInheritedTypes<Object>();

                // we return a copy so the original cached array cannot be modified by reference
                return (Type[])m_AllInheritedReferencedTypes.Clone();
            }
        }


        #endregion


        protected Object()
        {
            ReferencedTypeSerializationHelper.CheckSerializationInitialized();
            ReferencedTypeSerializationHelper.MarkKnownType(this);

            m_AllInMemory.Add(this);
            m_AllInMemoryModified = true;

            OnCreated();
        }
        /// <summary>
        /// This is added so the object is actually removed from lists.
        /// </summary>
        ~Object()
        {
            if (!IsDestroyed)
                Destroy();
        }

#region Utility




        /// <summary>
        /// Create a new referenced type of the specified type.  
        /// <see cref="CreateInstance{T}"/> is preferred over the constructor of this class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateInstance<T>() where T : Object
        {
            T result = System.Activator.CreateInstance<T>(); 
            return result;
        }

        /// <summary>
        /// Used to resolve a reference to a <see cref="Object"/> object via a GUID.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        internal static bool ResolveReference<T>(out T result, string guid) where T : Object
        {
            UpdateMemoryArrayIfModified();
            Object curr;
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
                    m_AllInMemoryAsArray = new Object[m_ObjectCount];
                if (m_AllInMemoryAsArray.Length < m_ObjectCount)
                    Array.Resize(ref m_AllInMemoryAsArray, m_ObjectCount);
                m_AllInMemory.CopyTo(m_AllInMemoryAsArray);
                m_AllInMemoryModified = false;
            }
        }



       

#endregion

#region Overridables

        protected virtual void OnCreated()
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

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            OnBeforeSerialization.TryInvoke();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            OnAfterDeserialization.TryInvoke();
        }






        #region Operators

        /// <summary>
        /// Allows these objects to be cast as bool like unity objects.
        /// </summary>
        /// <param name="type"></param>
        public static implicit operator bool(Object type)
        {
            return type != null && !type.IsDestroyed;
        }

        #endregion
    }


}
