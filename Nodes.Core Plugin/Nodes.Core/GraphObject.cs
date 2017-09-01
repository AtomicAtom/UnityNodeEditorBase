using System;
using System.Collections.Generic;
using UnityEngine;

namespace UNEB.Core
{
    /// <summary>
    /// Baseclass for everything in a <see cref="Graph"/>. This exposes the <see cref="Owner"/> property which enables a Node/Connection/etc to have a reference to the graph it resides in.
    /// </summary>
    [Serializable]
    public class GraphObject : Object
    {
        [SerializeField]
        string m_Name;

        /// <summary>
        /// Serializable Reference to the graph which owns this node.
        /// </summary>
        [SerializeField]
        Reference m_Owner;

        /// <summary>
        /// Event triggered when this Graph Object is added to a graph or moved to a new graph.
        /// </summary>
        protected event Action<Graph>
            OnGraphChanged;



        /// <summary>
        /// The Graph this node belongs to
        /// </summary>
        public Graph Owner
        {
            get
            {
                return m_Owner.TryGetValueAsType<Graph>();
            }
            internal set
            {
                if(m_Owner != (Reference)value)
                {
                    m_Owner = value;
                    OnGraphChanged.TryInvoke(value);
                } 
            }
        }
 


        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(m_Name))
                    m_Name = GetDefaultName();
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }

        protected GraphObject()
        {
            m_Name = GetDefaultName();
        }

        string GetDefaultName()
        {
            return GetType().Name + " " + GUID.Substring(GUID.Length - 7, 6);
        }


    }
}
