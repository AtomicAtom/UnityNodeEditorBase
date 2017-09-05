using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UNEB.Collections
{
    /// <summary>
    /// Serialized Property with type preservation
    /// </summary>
    [Serializable]
    public class Property : Internal.SerializableObject
    {

        [SerializeField, HideInInspector]
        string
            m_PropertyName,
            m_SerializedType,
            m_SerializedValue;

#region Cache 
        Type
            m_ValueType;

        object
            m_Value;
        #endregion



        #region Properties

        public string Name => m_PropertyName;

        public bool IsValid => ValueType != null && Value != null;

        public bool HasValue => Value != null;

        /// <summary>
        /// The known type of this property.
        /// </summary>
        public Type ValueType
        {
            get
            {
                // Attempt  to get type from serialized type name
                if (m_ValueType == null) m_ValueType = Type.GetType(m_SerializedType, false);
                // if still null: in case the type is a SerializedObject type, attempt to get type from known types in loaded assemblies:
                if (m_ValueType == null) m_ValueType = ReferencedTypeSerializationHelper.TryGetKnownType(m_SerializedType);
                return m_ValueType;
            }
        }
 
        /// <summary>
        /// Get/Set the value of this property.
        /// </summary>
        public object Value
        {
            get
            {
                if (m_Value == null && ValueType != null)
                    m_Value = JsonUtility.FromJson(m_SerializedValue, ValueType);
                return m_Value;
            }
            set
            {
                if (value == null)
                {
                    m_SerializedType = m_SerializedValue = null;
                    m_ValueType = null;
                    m_Value = null;
                }
                else
                {
                    m_ValueType = value.GetType();
                    m_SerializedType = value.GetType().FullName;
                    m_Value = value;
                    m_SerializedValue = JsonUtility.ToJson(value);
                }
            }
        }
      

#endregion


        /// <summary>
        /// Public constructor exists only for the sake of serialization.
        /// </summary>
        public Property()
        {

        }

        internal Property(string name)
        {
            m_PropertyName = SafeName(name);
        }


        internal static string SafeName(string n)
        {
            return n.Trim().ToLower().Replace(" ", "_");
        }

        public sealed override bool IsType(Type type)
        {
            if (ValueType == null) return false;
            return (ValueType == type || ValueType.IsSubclassOf(type));
        }

    }
}
