using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Nodes.Core
{
    /// <summary>
    /// A Serializable Graph Object
    /// </summary>
    /// <remarks>
    /// Since I am implementing "Graph" as a <see cref="ReferencedType"/> - then this means it will be possible 
    /// for one graph (or nodes in it) - to implement connections between  multiple graphs.
    /// 
    /// </remarks>
    [Serializable]
    public class Graph : ReferencedType, ISerializationCallbackReceiver
    {




 

        /// <summary>
        /// ALL <see cref="GraphObject"/>s in the Graph. This includes <see cref="Node"/> types and <see cref="Connection"/> types.
        /// </summary>
        //[SerializeField]
        List<GraphObject> m_Objects = new List<GraphObject>();



        protected event Action<GraphObject>
            OnAddObject,
            OnRemoveObject;

 


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

        /// <summary>
        /// Cached connection in our graph;
        /// </summary>
        Connection[] m_CachedConnections;

        /// <summary>
        /// Cached array of all objects (Arrays are faster to iterate than lists).
        /// </summary>
        GraphObject[] m_CachedAllObjects;

        void UpdateCacheIfDirty()
        {
            if(m_IsModified)
            {
                // cleanup any lingering objects that are null or destroyed.
                m_Objects.RemoveNullOrDestroyedEntries();
                // Update Nodes:
                m_Objects.UpdateCachedArrayWithType(ref m_CachedNodes);
                // Update Connections:
                m_Objects.UpdateCachedArrayWithType(ref m_CachedConnections);

                // acessable cached array of all objects:
                m_Objects.UpdateCachedArrayWithType(ref m_CachedAllObjects);

                m_IsModified = false;

                OnUpdateCache.TryInvoke();
            }
        }

        /// <summary>
        /// Forces the cache to be updated the next time cached objects need to be accessed.
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

        /// <summary>
        /// Get all the connections in this graph.
        /// </summary> 
        public void GetConnectionsNonAlloc(ref Connection[] result)
        {
            UpdateCacheIfDirty();
            m_CachedConnections.CopyNonAlloc(ref result);
        }

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
        /// Returns true if cached arrays in this graph needs ot be updated.
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
 


        bool AddObjectToGraph(GraphObject obj)
        {
            if (!obj) return false;
            if (obj.Owner && obj.Owner == this) // no change
                return false;

            if (obj.Owner)
                obj.Owner.RemoveObjectFromGraph(obj);

            if(RegisterGraphObject(obj))
            {
                obj.Owner = this;
                return true;
            }
            return false;
        }

        T AddObjectToGraph<T>() where T : GraphObject
        {
            T result = ReferencedType.CreateInstance<T>();
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
        /// Add an object to this graph;
        /// </summary>
        /// <param name="obj"></param>
        public void AddObject(GraphObject obj)
        {
            AddObjectToGraph(obj);
        }

        public T AddObject<T>() where T : GraphObject
        {
            T result = CreateInstance<T>();
            AddObjectToGraph(result);
            return result;
        }


        public void RemoveObject(int index)
        { 
            RemoveObjectFromGraph(m_Objects[index]);
        }

        public void RemoveObject(GraphObject obj)
        {
            RemoveObjectFromGraph(obj);
        }

        /// <summary>
        /// Remove and Destroy a <see cref="GraphObject"/> in this graph.
        /// </summary>
        /// <param name="obj"></param>
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
            UpdateCacheIfDirty();

            for(int i = 0; i < m_CachedConnections.Length; i++)
            {
                DestroyObject(m_CachedConnections[i]);
            }
        }


        /// <summary>
        /// Remove any connections that are connected to this node
        /// </summary>
        /// <param name="node"></param>
        public void DestroyConnectionsFromNode(Node node)
        {
            // NOT IMPLEMENTED
        }

 


        internal IEnumerator<GraphObject> GetEnumerator()
        {
            return m_Objects.AsEnumerable().GetEnumerator();
        }





        #region Utility

        /// <summary>
        /// Temp array for some operations within a graph- to avoid allocating new temporary arrays.
        /// </summary>
        static GraphObject[] tmp_GraphObjects = new GraphObject[0];


        /// <summary>
        /// Resolve a reference to a <see cref="GraphObject"/> only in the specified specified <see cref="Graph"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <param name="guid"></param>
        /// <param name="inGraph"></param>
        /// <returns>True if graph contains matching <see cref="ReferencedType"/> of the specified type</returns>
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
         * We manually serialize each object one by one to json and store the JSON strings as an array
         * 
         */

        [SerializeField]
        string[] m_GraphObjectSerializedData = new string[0]; 
        [SerializeField]
        string[] m_GraphObjectSerializedTypes = new string[0];

        void ISerializationCallbackReceiver.OnBeforeSerialize()
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


        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            m_Objects.Clear(); 
            for(int i = 0; i < m_GraphObjectSerializedData.Length && i < m_GraphObjectSerializedTypes.Length; i++)
            {
                Type t = ReferencedTypeSerializationHelper.TryGetKnownType(m_GraphObjectSerializedTypes[i]);
     
                if(t != null)
                m_Objects.Add((GraphObject)JsonUtility.FromJson(m_GraphObjectSerializedData[i], t));
            }
            m_IsModified = true;
            if (m_Objects.Count > 0)
                OnDeserialized.TryInvoke();
        }




        ///// <summary>
        ///// Force Register a <see cref="GraphObject"/> type that is not known by the Nodes.Core namespace.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        //public static void ManuallyRegisterType<T>() where T: GraphObject
        //{
        //    ReferencedTypeSerializationHelper.MarkKnownType(typeof(T));
        //}

        #endregion
    }
}
