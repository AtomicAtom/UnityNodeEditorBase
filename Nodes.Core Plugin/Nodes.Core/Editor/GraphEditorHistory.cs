using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace UNEB.Editor
{
    /// <summary>
    /// Stores/Restores serialized history states of <see cref="Object"/> types.
    /// Usefull for runtime or 'edit mode' graph/object editors.
    /// </summary>
    [Serializable]
    public class History : History<Object>
    {
        static internal History m_DefaultInstance;

        /// <summary>
        /// Returns the default global <see cref="History"/> instance.
        /// </summary>
        public static History Default => m_DefaultInstance
            ? m_DefaultInstance
            : (m_DefaultInstance = new History());



    }

    /// <summary>
    /// Stores/Restores serialized history states of <see cref="GraphObject"/> types in a <see cref="Graph"/>.
    /// Usefull for runtime or 'edit mode' graph/object editors.
    /// </summary>
    [Serializable]
    public class GraphObjectHistory : History<GraphObject>
    {

    }




    /// <summary>
    /// Baseclass for History.
    /// </summary>
    [Serializable]
    public abstract partial class History<T> : 
        Internal.SerializableObject 
        where T : Object
    {
        /// <summary>
        /// Maximum number of serialized history values to show pwer item
        /// </summary>
        [SerializeField]
        int m_MaxHistory = 20;

        [SerializeField]
        List<HistoryItem> m_Data = new List<HistoryItem>();


        /// <summary>
        /// Get/Set the maximum stored stated per entry
        /// </summary>
        public int MaxHistory
        {
            get => m_MaxHistory;
            set => m_MaxHistory = value < 2 ? 2 : value;
        }


        /// <summary>
        /// Store 
        /// </summary>
        /// <param name="obj"></param>
        public void StoreState(T obj)
        {
            GetHistory(obj).StoreState(obj, m_MaxHistory);
        }

        /// <summary>
        /// Return the history count for the specified object.
        /// </summary> 
        public int GetHistoryCount(T obj)
        {
            return GetHistory(obj).HistoryCount;
        }

        /// <summary>
        /// Restore the object to the specified point in history.
        /// </summary> 
        public bool RestoreState(ref T obj, int index = 0)
        {
            return GetHistory(obj).TryRestoreState(index, ref obj);
        }

        /// <summary>
        /// Returns true if the object has one or more states stored in this history.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool HasHistory(T obj)
        {
            return HistoryExists(obj) && GetHistory(obj).HistoryCount > 0;
        }

        /// <summary>
        /// Retrieve (and if required, create one) a history for the specified object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        HistoryItem GetHistory(T obj)
        {
            int i = IndexOf(obj);
            if (i < 0)
            {
                i = m_Data.Count;
                HistoryItem result = new HistoryItem(obj);
                m_Data.Add(result);
                return result;
            }
            return m_Data[i];
        }

        /// <summary>
        /// Clear the history.
        /// </summary>
        public void ClearHistory()
        {
            m_Data.Clear();
        }

        /// <summary>
        /// Clear stored history states of the specified object.
        /// </summary>
        /// <param name="obj"></param>
        public void ClearHistory(T obj)
        {
            GetHistory(obj).Clear();
        }

        bool HistoryExists(T obj)
        {
            return IndexOf(obj) >= 0;
        }

        int IndexOf(T obj)
        {
            if (!obj) return -1;
            for (int i = 0; i < m_Data.Count; i++)
                if (m_Data[i].Equals(obj))
                    return i;
            return -1;
        }
    }
}
