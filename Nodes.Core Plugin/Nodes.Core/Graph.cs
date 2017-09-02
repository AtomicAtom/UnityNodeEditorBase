using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

namespace UNEB
{
    /// <summary>
    /// A Serializable Graph Object
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    [Serializable]
    public abstract class Graph: 
        Object, 
        IEnumerable<GraphObject>  
    {
 
        /// <summary>
        /// ALL <see cref="GraphObject"/>s in the Graph. This includes <see cref="Node"/> types types.
        /// </summary>
        List<GraphObject> m_Objects = new List<GraphObject>();

        protected event Action<GraphObject>
            OnAddObject,
            OnRemoveObject;




        protected Graph()
        {
            OnSerializeInternal   += Graph_OnBeforeSerialize;
            OnDeserializeInternal += Graph_OnAfterDeserialize;
        }



        #region Cached Values

        bool m_IsModified = true;

        /// <summary>
        /// Event raised when cached arrays in this graph are updated.
        /// </summary>
        public event Action 
            OnUpdateCache;

        /// <summary>
        /// Event raised after objects have been deserialized. 
        /// (this will trigger in both Play and Edit modes in Unity via Unity's internal serializers).
        /// </summary>
        public event Action
            OnDeserialized;

        /// <summary>
        /// Chached nodes in our graph
        /// </summary>
        Node[] m_CachedNodes;



        ///// <summary>
        ///// Cached connection in our graph;
        ///// </summary>
        Connection[] m_CachedConnections;

        /// <summary>
        /// Cached array of all objects (Arrays are faster to iterate than lists).
        /// </summary>
        GraphObject[] m_CachedAllObjects;








        void UpdateCacheIfDirty()
        {
            if(m_IsModified)
            {
                // Ensure reference ot Owner graph is assigned in all objects: 
                SetGraphParents();


                // cleanup any lingering objects that are null or destroyed.
                m_Objects.RemoveNullOrDestroyedEntries();
                // Update Nodes:
                m_Objects.UpdateCachedArrayWithType(ref m_CachedNodes);
                //// Update Connections:
                //m_Objects.UpdateCachedArrayWithType(ref m_CachedConnections);

                // acessable cached array of all objects:
                m_Objects.UpdateCachedArrayWithType(ref m_CachedAllObjects);

                m_IsModified = false;

                UpdateConnectionCache(ref m_CachedConnections);

                OnUpdateCache.TryInvoke();
            }
        }


        /// <summary>
        /// Temporary list for <see cref="GetAllConnections"/>.
        /// </summary>
        static List<Connection> tmp_Connections = new List<Connection>();

        /// <summary>
        /// [NOT IMPLEMENTED]  Update Cached Connection infor between nodes.
        /// </summary>
        /// <param name="result"></param>
        void UpdateConnectionCache(ref Connection[] result)
        {
            //throw new NotImplementedException("Node Inputs/Outputs must be implemented before Connection info array can be created");
            //tmp_Connections.Clear();
            //foreach(Node n in m_CachedNodes)
            //{
                
            //}
        }

        /// <summary>
        /// Forces the cache to be updated the next time cached arrays need to be accessed.
        /// </summary>
        public void SetDirty()
        {
            m_IsModified = true;
        }

        /// <summary>
        /// Get all the nodes in this graph.
        /// </summary> 
        public void GetNodesNonAlloc(ref Node[] result)
        {
            UpdateCacheIfDirty();
            m_CachedNodes.CopyNonAlloc(ref result);
        }

        ///// <summary>
        ///// Get all the connections in this graph.
        ///// </summary> 
        //public void GetConnectionsNonAlloc(ref Connection[] result)
        //{
        //    UpdateCacheIfDirty();
        //    m_CachedConnections.CopyNonAlloc(ref result);
        //}

        /// <summary>
        /// Get All Objects in this graph.
        /// </summary>
        /// <param name="result"></param>
        public void GetAllObjectsNonAlloc(ref GraphObject[] result)
        {
            UpdateCacheIfDirty();
            m_CachedAllObjects.CopyNonAlloc(ref result);
        }

        #endregion

        /// <summary>
        /// Returns true if cached arrays in this graph needs to be updated.
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return m_IsModified;
            }
        }




        bool RegisterGraphObject(GraphObject obj)
        {
            ThrowIfDestroyed();
            if(obj && !m_Objects.Contains(obj))
            {
                m_Objects.Add(obj);
                m_IsModified = true;
                OnAddObject.TryInvoke(obj);
                return true;
            }
            return false;
        }

        void UnRegisterGraphObject(GraphObject obj)
        {
            ThrowIfDestroyed();
            if (obj && m_Objects.Remove(obj))
            {
                OnRemoveObject.TryInvoke(obj);
                m_IsModified = true;
            }
        }
 

        public bool Contains(GraphObject obj)
        {
            if (!obj) return false;
            return m_Objects.Contains(obj);
        }

        /// <summary>
        /// Retrieve the index of the specified graph object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Index of the <see cref="GraphObject"/>. -1 if object is null or does not exist in this graph.</returns>
        public int IndexOf(GraphObject obj)
        {
            return obj && m_Objects.Contains(obj)
                ? m_Objects.IndexOf(obj)
                : -1; 
        }
 

        /// <summary>
        /// Ads an object to the graph.
        /// </summary> 
        bool AddObjectToGraph(GraphObject obj)
        {
            if (!obj) return false;
            if (obj.Graph && obj.Graph == this) // no change, object is already in this graph
                return false;

            if (obj.Graph) // remove object from it's original graph
                obj.Graph.RemoveObjectFromGraph(obj);

            if(RegisterGraphObject(obj))
            {
                obj.Graph = this;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds an object to the <see cref="Graph"/>. 
        /// This can be any <see cref="GraphObject"/> type, not just <see cref="Node"/> types.
        /// </summary> 
        T AddObjectToGraph<T>() where T : GraphObject
        {
            T result = CreateInstance<T>();
            AddObjectToGraph(result);
            return result;
        }


        bool RemoveObjectFromGraph(GraphObject obj)
        {
            if (!obj) return false;
            if(m_Objects.Remove(obj))
            {
                m_IsModified = true;
                OnRemoveObject(obj);

                // If a node is removed the connections to or from this node will be broken, so we need to destroy 
                // only the connections To or From this node:
                if (obj is Node)
                    DestroyConnectionsFromNode((Node)obj);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a supported <see cref="Node"/> type.
        /// If type is not marked as supported via <see cref="SupportNode{T}"/> or an inheriting type
        /// this method will fail and return Null;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T AddNode<T>() where T : Node
        {
            if (IsNodeSupported<T>())
                return AddObjectToGraph<T>();
            return null;
        }
        

        /// <summary>
        /// If the specified <see cref="Node"/> is contained within this graph, it is removed and destroyed.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool DestroyNode(Node node)
        {
            if(node && Contains(node))
            {
                if(RemoveObjectFromGraph(node))
                {
                    node.Destroy();
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Alias of <see cref="AddObjectToGraph(GraphObject)"/>
        /// </summary>
        /// <param name="obj"></param>
        void AddObject(GraphObject obj)
        {
            AddObjectToGraph(obj);
        }

        /// <summary>
        /// Alias of <see cref="AddObjectToGraph{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T AddObject<T>() where T : GraphObject
        {
            T result = CreateInstance<T>();
            AddObjectToGraph(result);
            return result;
        }

        /// <summary>
        /// Removes <see cref="GraphObject"/> item from graph at the specified index.
        /// </summary> 
        void RemoveObject(int index)
        { 
            RemoveObjectFromGraph(m_Objects[index]);
        }

        /// <summary>
        /// Alias of <see cref="RemoveObjectFromGraph(GraphObject)"/>
        /// </summary>
        /// <param name="obj"></param>
        void RemoveObject(GraphObject obj)
        {
            RemoveObjectFromGraph(obj);
        }

        /// <summary>
        /// Removes and Destroys a <see cref="GraphObject"/> in this graph.
        /// This method will only succeed if the <see cref="GraphObject"/> is not destroyed 
        /// and is an actual member of this <see cref="Graph"/> instance.
        /// </summary>
        public bool DestroyObject(GraphObject obj)
        {
            if (!obj) return false;
            if(RemoveObjectFromGraph(obj))
            {
                obj.Destroy();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Destroy only the <see cref="Connection"/> types in this graph.
        /// </summary>
        public void DestroyAllConnections()
        {
            //UpdateCacheIfDirty();

            //for(int i = 0; i < m_CachedConnections.Length; i++)
            //{
            //    DestroyObject(m_CachedConnections[i]);
            //}
        }


        /// <summary>
        /// Remove any connections that are connected to this node
        /// </summary>
        /// <param name="node"></param>
        public void DestroyConnectionsFromNode(Node node)
        {
            // NOT IMPLEMENTED
        }

 

 





        #region Utility

        /// <summary>
        /// Temp array for some operations within a graph- to avoid allocating new temporary arrays.
        /// </summary>
        static GraphObject[] tmp_GraphObjects = new GraphObject[0];


        /// <summary>
        /// Resolve a reference to a <see cref="GraphObject"/> only in the specified specified <see cref="Graph"/>
        /// </summary>
        /// <remarks>
        /// // This is Unused - but may be useful in the future.
        /// </remarks>
        internal static bool ResolveReference<T>(out T result, string guid, Graph inGraph) where T : GraphObject
        {
            result = null;
            if (!inGraph) return false;
            inGraph.GetAllObjectsNonAlloc(ref tmp_GraphObjects);
            GraphObject curr;
            for (int i = 0; i < tmp_GraphObjects.Length; i++)
            {
                curr = tmp_GraphObjects[i];
                if ((curr is T || curr.GetType().IsSubclassOf(typeof(T))) && curr.GUID == guid)
                {
                    result = (T)curr;
                    return true;
                }
            }
            return false;
        }

        #endregion



        #region Serialization Trickery

        /*
         * 
         * Unity JSON serializer does not recognize each type in an array as a seperate type from the base type they inherit from.
         * We manually serialize each object one by one to json and store the JSON strings as a list.
         * 
         */

        [SerializeField, HideInInspector]
        string[] m_GraphObjectSerializedData = new string[0]; 

        [SerializeField, HideInInspector]
        string[] m_GraphObjectSerializedTypes = new string[0];

        void Graph_OnBeforeSerialize()
        {
            UpdateCacheIfDirty();
            m_GraphObjectSerializedData = new string[m_Objects.Count];
            m_GraphObjectSerializedTypes = new string[m_Objects.Count];
            for (int i = 0; i < m_Objects.Count; i++)
            {
                m_GraphObjectSerializedData[i] = JsonUtility.ToJson(m_Objects[i]);
                Type T = m_Objects[i].GetType();
                m_GraphObjectSerializedTypes[i] = T.FullName;
            }
        }


        void Graph_OnAfterDeserialize()
        {
            m_Objects.Clear(); 
            for(int i = 0; i < m_GraphObjectSerializedData.Length && i < m_GraphObjectSerializedTypes.Length; i++)
            {
                Type t = ReferencedTypeSerializationHelper.TryGetKnownType(m_GraphObjectSerializedTypes[i]);
                if(t != null)
                {
                    m_Objects.Add((GraphObject)JsonUtility.FromJson(m_GraphObjectSerializedData[i], t)); 
                }
                else
                {
                    Debug.LogWarning
                    (
                        string.Format("Unable to deserialize Graph Object of type '{0}' because it's type could not be found.", m_GraphObjectSerializedTypes[i])
                    );
                }
            }

            SetGraphParents();

            m_IsModified = true;
            if (m_Objects.Count > 0)
                OnDeserialized.TryInvoke();

        }

        /// <summary>
        /// Ensures <see cref="GraphObject.Graph"/> is assigned to <see cref="GraphObject"/>.
        /// </summary>
        void SetGraphParents()
        {
            foreach (GraphObject obj in m_Objects)
                obj.Graph = this;
        }




        #endregion





        #region Node Type Restriction



        /// <summary>
        /// List of supported node types defined manually.
        /// </summary>
        List<Type> m_SupportedNodeTypes = new List<Type>();


        /// <summary>
        /// Support all node types inheriting from <see cref="Node"/>. 
        /// This is the equivelant of passing <see cref="Node"/> as the <see cref="Type"/> Parameter to <see cref="SupportNode{T}"/>.
        /// </summary>
        protected void SupportAllNodeTypes()
        {
            SupportNode<Node>();
        }


        /// <summary>
        /// Returns true if specified <see cref="Node"/> Type is marked as supported by this graph.
        /// </summary>
        public bool IsNodeSupported<T>() where T : Node
        {
            Type type = typeof(T);
            if (m_SupportedNodeTypes.Contains(type))
                return true;
            foreach (Type t in m_SupportedNodeTypes)
                if (t == type || t.IsSubclassOf(type))
                    return true;
            return false; 
        }

        /// <summary>
        /// Registers a <see cref="TNodeType"/> and inheriting types to be supported by this node.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        protected void SupportNode<T>() where T : Node
        {
            Type t = typeof(T);
            if (!IsNodeSupported<T>())
                m_SupportedNodeTypes.Add(t);
        }


        public IEnumerator<GraphObject> GetEnumerator()
        {
            return m_Objects.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }




        /// <summary>
        /// Array of node types supported by this graph. (Inheriting types of nodes/baseclasses supported by this graph are supported as well).
        /// </summary>
        public Type[] SupportedNodeTypes => m_SupportedNodeTypes.ToArray();


 

        #endregion
    }









}
