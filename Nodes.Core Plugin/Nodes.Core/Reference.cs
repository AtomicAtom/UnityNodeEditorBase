using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Nodes.Core
{

    

    /// <summary>
    /// Serializable Reference for <see cref="ReferencedType"/> objects. All inheriting types should use <see cref="System.SerializableAttribute"/>.
    /// 
    /// 
    /// This is to behave similar to unity's new ExposedReference struct (which does not accommodate Non Unity objects),
    /// but we use our own for compatibility with older versions of Unity instead of using their type.
    /// </summary> 
    /// <remarks>
    /// Originally I intended to use a baseclass and have inheriting types for seperate class type references - but this would imply
    /// creating seperate operators for every type. So instead we can just use one reference type for ALL <see cref="ReferencedType"/> objects instead. 
    /// </remarks>
    [Serializable]
    public struct Reference : IEquatable<Reference> , IEquatable<ReferencedType>
    {
        /// <summary>
        /// An empty reference. Casts to (Null).
        /// </summary>
        public static Reference Empty
        {
            get
            {
                return new Reference() { m_ReferenceGuid = null } ;
            }
        }

 

        ReferencedType m_Value;
 
        /// <summary>
        /// Serialized guid needed to resolved reference after deserialization;
        /// </summary>
        [SerializeField]
        string m_ReferenceGuid;

        internal string ReferenceGuid
        {
            get
            {
                return m_ReferenceGuid;
            }
        }

        public ReferencedType Value
        {
            get
            {
                return Get();
            }
        }

        public bool HasValue
        {
            get
            {
                return Value != null && !Value.IsDestroyed
                    ? true
                    : false;
            }
        }

        public Type ValueType
        {
            get
            {
                return HasValue
                    ? Value.GetType()
                    : typeof(ReferencedType);
            }
        }

        internal Reference(ReferencedType value) : this()
        {
            Set(value);
        }


        ReferencedType Get()
        {
            if (string.IsNullOrEmpty(m_ReferenceGuid))
                return null;

            if (m_Value == null || m_Value.GUID != m_ReferenceGuid)
            {
                ReferencedType.ResolveReference(out m_Value, m_ReferenceGuid);
            }

            return m_Value;
        }

        /// <summary>
        /// Returns true if this reference currently references a <see cref="GraphObject"/> type.
        /// </summary>
        /// <returns></returns>
        public bool IsGraphObjectReference()
        {
            if (!HasValue) return false;
            return (ValueType == typeof(GraphObject) || ValueType.IsSubclassOf(typeof(GraphObject)));
        }

        ///// <summary>
        ///// Retrieve value - and if required - resolve/return ONLY a reference which exists in the specified graph.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="graph"></param>
        ///// <returns></returns>
        //internal GraphObject GetOrResolveReferenceInGraph<T>(Graph graph)
        //{
        //    if (!graph) return null;
        //    if(!IsGraphObjectReference())
        //    {
        //        if (Graph.ResolveReference(out m_Value, m_ReferenceGuid))
        //        {
        //            return (GraphObject)m_Value;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        return (GraphObject)Value; 
        //    }
        //}

        internal void Set(ReferencedType value)
        {
            if (value == null)
            {
                m_Value = null;
                m_ReferenceGuid = null;
                return;
            }
            m_ReferenceGuid = value.GUID;
            m_Value = value;
        }



        public T TryGetValueAsType<T>() where T : ReferencedType
        {
            return Value is T
                ? (T)Value
                : null;
        }
 
        public override int GetHashCode()
        {
            if (m_ReferenceGuid == null)
                return string.Empty.GetHashCode();
            return m_ReferenceGuid.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Reference     ) return Equals((Reference)obj);
            if (obj is ReferencedType) return Equals((ReferencedType)obj);
            return false;
        }

        public bool Equals(Reference other)
        {
            return other.m_ReferenceGuid == m_ReferenceGuid;
        }

        public bool Equals(ReferencedType other)
        {
            if (!HasValue) return false;
            if (other == null || other.IsDestroyed) return false;
            return ReferenceEquals(Value, other);
        }


        #region Operators

        public static implicit operator ReferencedType(Reference reference)
        {
            if (!reference.HasValue || reference == Empty)
                return null;
            return reference.Value;
        }
 
        public static implicit operator Reference(ReferencedType value)
        {
            if (value == null || value.IsDestroyed)
                return Empty;

            return new Reference(value);
        }
 
        public static bool operator ==(Reference a, Reference b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(Reference a, Reference b)
        {
            return !a.Equals(b);
        }
 
        public static bool operator ==(Reference a, ReferencedType b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(Reference a, ReferencedType b)
        {
            return !a.Equals(b);
        }

        public static bool operator ==(ReferencedType a, Reference b)
        { 
            return b.Equals(a);
        }
        public static bool operator !=(ReferencedType a, Reference b)
        {
            return !b.Equals(a);
        }

        #endregion
    }






}
