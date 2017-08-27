using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Nodes.Core.Example_Usage
{
    /// <summary>
    /// Example that shows Graphs can be stored inside nodes even...
    /// ... Which means a node or nodes in <see cref="Graph"/> might even be able to connect to nodes inside of a graph stored inside of a node.
    /// </summary>
    [Serializable]
    public class GraphNode : Node
    {
        public Graph ChildGraphA;
        public Graph ChildGraphB;
    }
}
