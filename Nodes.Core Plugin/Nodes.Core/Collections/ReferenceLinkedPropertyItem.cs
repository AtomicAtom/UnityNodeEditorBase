using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UNEB.Collections
{
    /// <summary>
    /// Serializable reference linked property collection object.
    /// </summary>
    [Serializable]
    public sealed class ReferenceLinkedPropertyItem 
    {
        [SerializeField]
        Reference m_TargetReference;

        [SerializeField]
        PropertyCollection m_Properties = new PropertyCollection();


        Object m_CachedTarget;

        public Object Target => m_CachedTarget
            ? m_CachedTarget
            : (m_CachedTarget = m_TargetReference.Value);

        /// <summary>
        /// A reference to the <see cref="Object"/> the properties are intended for.
        /// </summary>
        public Reference Reference => m_TargetReference;

        /// <summary>
        /// Properties for the <see cref="Target"/> reference.
        /// </summary>
        public PropertyCollection Properties => !m_Properties
            ? (m_Properties = new PropertyCollection())
            : m_Properties;

        /// <summary>
        /// Public Constructor only exists for sake of serialization.
        /// </summary>
        public ReferenceLinkedPropertyItem()
        {

        }


        internal ReferenceLinkedPropertyItem(Object target)
        { 
            m_TargetReference = m_CachedTarget = target;
        }
    }
}
