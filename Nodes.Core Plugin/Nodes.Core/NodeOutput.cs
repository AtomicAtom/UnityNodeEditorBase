using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UNEB
{
    /// <summary>
    /// Baseclass for Node Output types.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    [Serializable]
    public abstract class NodeOutputBase<TNode> :
        NodePort<TNode, NodeInputBase<TNode>, NodeOutputBase<TNode>>
        where TNode : Node
    {
       
    }


    /// <summary>
    /// Basic Node Output 
    /// </summary>
    [Serializable]
    public class NodeOutput : NodeOutputBase<Node>
    {
        /// <summary>
        /// Returns the <see cref="Connection"/> for this <see cref="NodeOutput"/>. 
        /// If not <see cref="NodePort{TNode, TTarget, TSelf}.IsConnected"/>, this will return <see cref="Connection.Empty"/>
        /// </summary>
        public override Connection GetConnection => this;

        public override bool CanConnect(NodeInputBase<Node> target)
        {
            return base.CanConnect(target);
        }
    }
}
