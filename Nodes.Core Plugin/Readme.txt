Author: CodieMorgan (AtomicAtom)

This is my rewritten Node Core api.  It is in a VS2017 Solution and intends to target unity as drop in plugins (instead of dropping a crap-ton of scripts all over the assets heirarchy) 
and removes the ScriptableObject dependency for serializing nodes as a rewritten-from-scratch api.

Feature/Nodes.Core is the main development branch for the core functionality of the API. Editor things will go into Nodes.Core.Editor.

Compiled version of this are plugin DLL assemblies (so unity doesnt have to compile aextra scripts).

Nodes.Libnoise will be extension (also as plugin) which will extend this API with Libnoise port.



The Dependency Folder contains target reference Unity assemblies for reference paths that dont break across multiple PCs. Current Version 2017.1.


-- additional notes here --