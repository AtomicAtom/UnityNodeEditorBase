using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UNEB.Core
{
    public interface IConnection<TTarget, TOwner> where TOwner : Node 
    {

        event Action<TTarget>
            OnDisconnect,
            OnConnect;

        TOwner Node { get; }

        bool IsConnected { get;}


        /// <summary>
        /// Both ends of the connection must agree they can be connected to each other.
        /// </summary> 
        /// <returns></returns>
        bool CanConnect(TTarget target);

        bool TryConnect(TTarget target);
 
        bool Disconnect();

        //bool TryGetValue<T>(out T result);


        /// <summary>
        /// Returns a <see cref="Connection"/> info for this.
        /// </summary>
        Connection GetConnection { get; }

    }


    /// <summary>
    /// Common interface for <see cref="Object"/> types which are children to a <see cref="TNodeType"/>.
    /// </summary>
    /// <typeparam name="TNodeType"></typeparam>
    public interface INodeChild<TNodeType>
    {

        TNodeType Node { get; }

        void OnInitialize(TNodeType parent);
    }
}
