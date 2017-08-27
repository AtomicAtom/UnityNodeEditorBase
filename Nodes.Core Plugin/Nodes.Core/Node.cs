using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
namespace Nodes.Core
{
    /// <summary>
    /// Baseclass for all Node Types. Inheriting Types must implement the <see cref="System.SerializableAttribute"/>.
    /// </summary>
    [Serializable]
    public abstract class Node : GraphObject
    {

        /// <summary>
        /// All connection references from this node.
        /// </summary>
        [SerializeField]
        List<Reference> m_Connections = new List<Reference>();

        [SerializeField]
        string m_Name = "Node";

#region Events

        protected event Action<Graph>
            OnAddedToGraph,
            OnRemovedFromGraph;



        #endregion


 #region Properties
         
        /// <summary>
        /// Get/Set the name of this node.
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }



        public Connection GetConnection(int index)
        {
            return m_Connections[index].TryGetValueAsType<Connection>();
        }


#endregion



    }
}
