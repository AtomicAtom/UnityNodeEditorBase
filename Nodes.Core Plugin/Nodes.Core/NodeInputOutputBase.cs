using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UNEB
{
    /// <summary>
    /// Baseclass for <see cref="NodeInput"/> and <see cref="NodeOutput"/> ports on a <see cref="UNEB.Node"/>
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    [Serializable]
    public abstract class NodePort<TNode, TTarget, TSelf> : 
        Object,
        IConnection<TTarget, TNode>
        where TNode   : Node
        where TSelf   : NodePort<TNode, TTarget, TSelf>
        where TTarget : NodePort<TNode, TSelf, TTarget>
    {

        [SerializeField]
        Reference
            m_Node,
            m_Target;

        /// <summary>
        /// Event triggered when this port is disonnected from anoher port.
        /// </summary>
        public event Action<TTarget> OnDisconnect;

        /// <summary>
        /// Event triggered when this port is connected to a new port.
        /// </summary>
        public event Action<TTarget> OnConnect;

        /// <summary>
        /// Returns the parent <see cref="Node"/> this <see cref="NodePort{TNode, TTarget, TSelf}"/> belongs to.
        /// </summary>
        public TNode Node => m_Node.TryGetValueAsType<TNode>();

        /// <summary>
        /// Returns the known graph of this port. 
        /// Is this is not part of <see cref="Node"/> then this may likely return Null.
        /// </summary>
        public Graph Graph => Node ? Node.Graph : null;

        /// <summary>
        /// Returns true if Node is valid and connection is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return Node && IsConnected;
            }
        }

        /// <summary>
        /// Returns true if Target is valid and target connects back to this.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return Target && Target.Target == this;
            }
        }

        /// <summary>
        /// Returns the <see cref="Connection"/> for this <see cref="NodePort{TParent, TTarget, TSelf}"/>. 
        /// If not <see cref="IsConnected"/>, this will return <see cref="Connection.Empty"/>
        /// </summary>
        public abstract Connection GetConnection { get; }


        /// <summary>
        /// The port this port is connected to.
        /// </summary>
        public TTarget Target => (TTarget)m_Target; 


        public virtual bool CanConnect(TTarget target)
        {
            return true;
        }



        protected NodePort()
        {
            OnDestroy +=()=> { Disconnect(); };
        }


        public bool Disconnect()
        {
            if (IsConnected && Target)
            {
                TTarget tmp = Target;
                m_Target = null;
                OnDisconnect.TryInvoke(tmp);
                tmp.Disconnect(); 
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempt to connect this port to the target port.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool TryConnect(TTarget target)
        {
            if (IsDestroyed || !target || target == this)
                return false;

            if (Target == target)
                return true; // is already connected to the spcified target

    
            if(target && CanConnect(target))
            {
                // break any existing connection:
                Disconnect();
                target.Disconnect();

                if(DoConnectIfNotConnected(target) && target.DoConnectIfNotConnected((TSelf)this))
                {
                    return true;
                }
                else
                {
                    Destroy();
                }
            }
            return false;
        }


        bool DoConnectIfNotConnected(TTarget target)
        {
            if(!IsConnected && target && CanConnect(target))
            {
                m_Target = target;
                OnConnect.TryInvoke(target);
                return true;
            }
            return false;
        }

        //public abstract bool TryGetValue<T>(out T result);


        internal void DoInitialize(TNode node)
        {
            m_Node = node;
            OnInitialize(node);
        }

        protected virtual void OnInitialize(TNode node)
        {

        }
    }
}
