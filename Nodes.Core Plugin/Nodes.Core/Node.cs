using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
namespace UNEB
{
    /// <summary>
    /// Baseclass for all Node Types. Inheriting Types must implement the <see cref="System.SerializableAttribute"/>.
    /// TODO: NodeInput/NodeOuput implementation here.
    /// </summary>
    [Serializable]
    public abstract class Node : GraphObject
    {

        [SerializeField]
        List<NodeInputBase<Node>>  m_Inputs = new List<NodeInputBase<Node>>();

        [SerializeField]
        List<NodeOutputBase<Node>> m_Outputs = new List<NodeOutputBase<Node>>();

 

#region Events

        protected event Action<Graph>
            OnAddedToGraph,
            OnRemovedFromGraph;



        #endregion


#region Properties

        /// <summary>
        /// Cached array of all known types which inherit from <see cref="Node"/>, not including Abstract types.
        /// This includes types in all loaded .NET/Mono assemblies, including those defined by scripts in the unity assets folder.
        /// </summary>
        static Type[] m_AllKnownInheritingNodeTypes;

        /// <summary>
        /// Returns an array of all known types which inherit from <see cref="Node"/>, not including abstract types.
        /// This includes types in all loaded .NET/Mono assemblies, including those defined by scripts in the unity assets folder.
        /// </summary>
        public static Type[] AllKnownInheritingNodeTypes
        {
            get
            {
                if (m_AllKnownInheritingNodeTypes == null)
                    m_AllKnownInheritingNodeTypes = ReferencedTypeSerializationHelper.GetAllInheritedTypes<Node>();

                // return a copy so the cached array cannot be modified by reference
                return (Type[])m_AllKnownInheritingNodeTypes.Clone();
            }
        }

#endregion



    }
}
