using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UNEB
{
    /// <summary>
    /// Basic Graph implementation which supports all <see cref="Node"/> types. This class cannot be inherited. Inherit from <see cref="Graph"/> instead.
    /// </summary>
    [Serializable]
    public sealed class SimpleGraph : Graph
    {
        public SimpleGraph()
        {
            SupportAllNodeTypes();
        }
    }
}
