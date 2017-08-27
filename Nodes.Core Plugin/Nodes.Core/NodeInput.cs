using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Nodes.Core
{
    [Serializable]
    public abstract class NodeInputBase : ReferencedType, IConnection<NodeOutputBase>
    {
        Reference m_Output;

        public NodeOutputBase Output
        {
            get
            {
                return m_Output.TryGetValueAsType<NodeOutputBase>();
            }
        }

        public bool IsConnected
        {
            get
            {
                return Output && Output.IsConnected;
            }
        }

        bool IConnection<NodeOutputBase>.IsConnected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event Action<NodeOutputBase> OnDisconnect;
        public event Action<NodeOutputBase> OnConnect;

        public bool CanConnect(NodeOutputBase target)
        {
            throw new NotImplementedException();
        }

        public bool Disconnect()
        {
            throw new NotImplementedException();
        }

        public bool TryConnect(NodeOutputBase target)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue<T>(out T result)
        {
            throw new NotImplementedException();
        }
    }
}
