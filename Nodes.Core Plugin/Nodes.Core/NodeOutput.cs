using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Nodes.Core
{
    [Serializable]
    public abstract class NodeOutputBase : ReferencedType, IConnection<NodeInputBase>
    {
        Reference m_Input;


        public NodeInputBase Input
        {
            get
            {
                return m_Input.TryGetValueAsType<NodeInputBase>();
            }
        }

        public bool IsConnected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event Action<NodeInputBase> OnDisconnect;
        public event Action<NodeInputBase> OnConnect;

        public bool CanConnect(NodeInputBase target)
        {
            throw new NotImplementedException();
        }

        public bool Disconnect()
        {
            throw new NotImplementedException();
        }

        public bool TryConnect(NodeInputBase target)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue<T>(out T result)
        {
            throw new NotImplementedException();
        }
    }
}
