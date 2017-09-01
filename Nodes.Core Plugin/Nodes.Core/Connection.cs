using System;
using System.Collections.Generic;
using UnityEngine;


namespace UNEB
{
    /// <summary>
    /// Represents a connection between a <see cref="NodeOutputBase"/> and a <see cref="NodeInputBase"/> in a <see cref="Graph{TNodeType, TInputType, TOutputType}"/>
    /// </summary> 
    [Serializable]
    public struct Connection 
    { 
        /// <summary>
        /// Represents a connection between a <see cref="NodeInputBase{TParentType}"/> and a <see cref="NodeOutputBase{TParentType}"/>
        /// </summary>
        public static Connection Empty => new Connection(null, null);

        [SerializeField]
        Reference 
            m_NodeOutput, 
            m_NodeInput;


        public Node InputNode  => m_NodeInput .HasValue ? m_NodeInput .TryGetValueAsType<Node>() : null;
        public Node OutputNode => m_NodeOutput.HasValue ? m_NodeOutput.TryGetValueAsType<Node>() : null;

        /// <summary>
        /// Returns reference to a <see cref="Graph"/> is referenced instance is loaded in memory and exists.
        /// </summary>
        public Graph Graph => InputNode && InputNode.Owner
            ? InputNode.Owner
            : OutputNode && OutputNode.Owner
                ? OutputNode.Owner
                : null;


        /// <summary>
        /// Retrieves a node input if the input is of the specified type or inheriting type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T TryGetInput<T>() where T : NodeInputBase<Node>
        {
            return m_NodeInput.TryGetValueAsType<T>();
        }

        /// <summary>
        /// Retrieves a node output if the output is of the specified type or inheriting type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T TryGetOutput<T>() where T : NodeInputBase<Node>
        {
            return m_NodeOutput.TryGetValueAsType<T>();
        }


        internal Connection(Reference input, Reference output)
        {
            m_NodeOutput = output;
            m_NodeInput  = input;
        }



        public bool IsValid
        {
            get
            {
                return 
                    m_NodeInput .HasValue && 
                    m_NodeOutput.HasValue && 
                    m_NodeInput .Value == m_NodeOutput.Value && 
                    m_NodeOutput.Value == m_NodeInput .Value;
            }
        }


        public static implicit operator Connection(NodeInputBase<Node> input)
        {
            return input.IsConnected
                ? new Connection(input, input.Target)
                : Connection.Empty;
        }

        public static implicit operator Connection(NodeOutputBase<Node> output)
        {
            return output.IsConnected
                ? new Connection(output.Target, output)
                : Connection.Empty;
        }
    }
}
