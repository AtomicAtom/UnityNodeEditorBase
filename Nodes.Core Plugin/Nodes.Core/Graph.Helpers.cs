using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace UNEB
{
    partial class Graph
    {


		/// <summary>
        /// Get all <see cref="GraphObject"/> in the graph of the specified type or inheriting types.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
		public T[] GetObjectsOfType<T>() where T : GraphObject
        {
            UpdateCacheIfDirty();
            return (T[])m_CachedAllObjects.Where((GraphObject o) =>
            {
                return o && o.IsType<T>();
            }).ToArray();
        }

		/// <summary>
        /// Get All nodes in graph of the specified type or inheriting types.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
		public T[] GetNodesOfType<T>() where T : Node
        {
            UpdateCacheIfDirty();
            return (T[])m_CachedNodes.Where((Node n) =>
            {
                return n && n.IsType<T>();
            }).ToArray();
        }

        /// <summary>
        /// Attempt to find a <see cref="GraphObject"/> in the <see cref="Graph"/> of the specified <see cref="Type"/> or inheriting type or  by it's <see cref="GraphObject.Name"/> or by it's GUID.
        /// </summary>
        public T FindObject<T>(string nameOrGuid) where T : GraphObject
        {
            UpdateCacheIfDirty();
            return (T)(m_CachedAllObjects.FirstOrDefault((GraphObject o) =>
            {
                if (o && o.IsType<T>())
                {
                    return o.Name == nameOrGuid || o.GUID == nameOrGuid;
                }
                return false;
            }));
        }

        /// <summary>
        /// Attempt to find a <see cref="Node"/> in the <see cref="Graph"/> of the specified <see cref="Type"/> or inheriting type or by it's <see cref="GraphObject.Name"/> or by it's GUID.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nameOrGuid"></param>
        /// <returns></returns>
        public T FindNode<T>(string nameOrGuid) where T : Node
        {
            UpdateCacheIfDirty();
            return (T)(m_CachedAllObjects.FirstOrDefault((GraphObject o) =>
            {
                if (o && o.IsType<T>())
                {
                    return o.Name == nameOrGuid || o.GUID == nameOrGuid;
                }
                return false;
            }));
        }

        /// <summary>
        /// Attempt to find a <see cref="GraphObject"/> in the graph of the specified type and by user defined check.
        /// </summary> >
        public T FindObject<T>(Func<T, bool> predicate) where T : GraphObject
        {
            UpdateCacheIfDirty();
            return (T)(m_CachedAllObjects.FirstOrDefault((GraphObject o) =>
            {
                if (predicate != null && o && o.IsType<T>())
                {
                    try
                    {
                        return predicate((T)o);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
                return false;
            }));
        }

        /// <summary>
        /// Attempt to find a <see cref="Node"/> in the graph of the specified type and by user defined check.
        /// </summary> >
        public T FindNode<T>(Func<T, bool> predicate) where T : Node
        {
            UpdateCacheIfDirty();
            return (T)(m_CachedNodes.FirstOrDefault((GraphObject o) =>
            {
                if (predicate != null && o && o.IsType<T>())
                {
                    try
                    {
                        return predicate((T)o);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
                return false;
            }));
        }

    }
}
