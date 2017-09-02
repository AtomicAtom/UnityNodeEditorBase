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
    /// <remarks>
    /// 
    /// TODO: Check for duplicate GUID (can happen onm deserialization of same object twice).
    ///       -- Output warning or throw exception if object by this GUID has been deserialized twice.
    /// </remarks>
    public abstract class Object : Internal.SerializableObject
    {
        [SerializeField]
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
        static int m_ObjectCount;





        /// <summary>
        /// Cached array of all known types that inherit from <see cref="Object"/>.
        /// </summary>
        static Type[] m_AllInheritedReferencedTypes;




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

            OnDeserializeInternal += () =>
            {
                // TODO: Check for duplicate GUID here.
                // NOTE: -- must not
            };

            OnDestroyInternal += () =>
            {
                if (m_AllInMemory.Remove(this))
                    m_AllInMemoryModified = true;
            };
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


        ///// <summary>
        ///// Safe way to copy objects without duplicate Guid:
        ///// If original object is NULL or destroyed this method will return null. 
        ///// 
        ///// NOTE: this currently only works with single objects and cannot duplicate an entire <see cref="Graph"/> yet.
        ///// </summary>
        ///// <param name="original"></param>
        ///// <returns></returns>
        //public static T Copy<T>(T original) where T : Object
        //{
        //    if (!original) return null;

        //    T obj = CreateInstance<T>();
        //    string guid = obj.GUID; 
        //    JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(obj), obj);
        //    obj.m_ID = guid; // preserve new instance GUID.
        //    return obj;
        //}


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
            for (int i = 0; i < m_ObjectCount; i++)
            {
                curr = m_AllInMemoryAsArray[i];
                if ((curr is T || curr.GetType().IsSubclassOf(typeof(T))) && curr.GUID == guid)
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
            if (m_AllInMemoryModified || m_AllInMemoryAsArray == null)
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

        #endregion










    }
}
