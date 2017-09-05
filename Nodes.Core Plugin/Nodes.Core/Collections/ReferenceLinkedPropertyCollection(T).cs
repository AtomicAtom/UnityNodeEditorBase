using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace UNEB.Collections
{
    /// <summary>
    /// Collection of <see cref="PropertyCollection"/>s resolved to <see cref="Object"/>s via <see cref="Reference"/>. 
    /// This allows storing external properties for serializable objects in a seperate location or file.
    /// Usefull for runtime or 'edit mode' editors which can store editor only values for objects in a graph.
    /// </summary> 
    [Serializable]
    public sealed class ReferenceLinkedPropertyCollection : ReferenceLinkedPropertyCollection<Object>
    {
        static internal ReferenceLinkedPropertyCollection m_DefaultInstance;

        /// <summary>
        /// Returns the global default <see cref="ReferenceLinkedPropertyItem"/> instance.
        /// </summary>
        public static ReferenceLinkedPropertyCollection Default => m_DefaultInstance
            ? m_DefaultInstance
            : (m_DefaultInstance = new ReferenceLinkedPropertyCollection());



    }



    /// <summary>
    /// Baseclass for <see cref="PropertyCollection"/>s linked to <see cref="Object"/>s via <see cref="Reference" />. 
    /// This allows storing external properties for serializable objects in a seperate location or file.
    /// </summary> 
    public abstract class ReferenceLinkedPropertyCollection<T> : 
        Internal.SerializableObject 
        where T : Object
    {
        [SerializeField]
        List<ReferenceLinkedPropertyItem> m_Items = new List<ReferenceLinkedPropertyItem>();



        /// <summary>
        /// Set a <see cref="Reference"/> linked property value for the specified object.
        /// </summary> 
        public void SetProperty<TPropertyType>(T forObject, string propertyName, TPropertyType value)
        {
            if (!forObject) throw new ArgumentNullException("forObject");
            GetProperties(forObject).SetProperty<TPropertyType>(propertyName, value);
        }

        /// <summary>
        /// Attempt to retrieve a <see cref="Reference"/> linked property value for the specified <see cref="T"/> by name if it exists.
        /// </summary> 
        public TPropertyType GetProperty<TPropertyType>(T forObject, string propertyName, TPropertyType defaultValue = default(TPropertyType))
        {
            if (!forObject) throw new ArgumentNullException("forObject");
            return GetProperties(forObject).GetProperty<TPropertyType>(propertyName, defaultValue);
        }

        /// <summary>
        /// Returns and/or creates a <see cref="PropertyCollection"/> linked to the specified <see cref="Object"/> resolved via <see cref="Reference"/>.
        /// </summary>
        public PropertyCollection GetProperties(T obj)
        {
            if (!obj) throw new ArgumentNullException("obj");
            ReferenceLinkedPropertyItem result = (m_Items.FirstOrDefault((ReferenceLinkedPropertyItem o) =>
            {
                return o.Target && o.Target == obj;
            }));
            if (result == null)
            {
                result = new ReferenceLinkedPropertyItem(obj);
                m_Items.Add(result);
            }
            return result.Properties;
        }

  
    }
}
