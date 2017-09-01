using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace UNEB.Core
{
    [Serializable]
    public abstract class NodeInputBase<TNode> :
        NodePort<TNode, NodeOutputBase<TNode>, NodeInputBase<TNode>>
        where TNode : Node
    {
 
    }


    /// <summary>
    /// Basic Node Input
    /// </summary>
    [Serializable]
    public class NodeInput : NodeInputBase<Node>
    {
        /// <summary>
        /// Returns the <see cref="Connection"/> for this <see cref="NodeInput"/>. 
        /// If not <see cref="NodePort{TNode, TTarget, TSelf}.IsConnected"/>, this will return <see cref="Connection.Empty"/>
        /// </summary>
        public override Connection GetConnection => this;
    }
}
