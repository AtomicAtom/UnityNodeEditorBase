using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nodes.Core
{
    public interface IConnection<TTarget> 
    {

        event Action<TTarget>
            OnDisconnect,
            OnConnect;

        bool IsConnected { get; set; }


        /// <summary>
        /// Both ends of the connection must agree they can be connected to each other.
        /// </summary> 
        /// <returns></returns>
        bool CanConnect(TTarget target);

        bool TryConnect(TTarget target);
 
        bool Disconnect();

        bool TryGetValue<T>(out T result);

    }
}
