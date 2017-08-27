using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nodes.Core
{
    /// <summary>
    /// Baseclass for everything in a <see cref="Graph"/>. This exposes the <see cref="Owner"/> property which enables a Node/Connection/etc to have a reference to the graph it resides in.
    /// </summary>
    [Serializable]
    public class GraphObject : ReferencedType
    {
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
                    OnGraphChanged(value);
                } 
            }
        }
 
         
    }
}
