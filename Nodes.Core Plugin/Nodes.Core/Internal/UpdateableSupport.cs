using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UNEB.Core.Internal
{
    /// <summary>
    /// TODO: This is to be helper whichs will allows "Updateable" <see cref="GraphObject"/> types to exist - thus allowing an "Update" method to exis in GraphNodes (runtime only).
    /// TODO: This will also allow updatable <see cref="Object"/> objects to be able to run and await coroutines.
    /// TODO: EditorUpdatable support for <see cref="GraphObject"/> types. (Unity only allows 10 frames per second for editorUpdate) - but may be useful for some types of graph editor functionaloty
    /// </summary>
    internal static class UpdateableSupport
    {


    }
}
