using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace UNEB.Editor
{
    /// <summary>
    /// PlaceHolder
    /// TODO: This will be a class for storing states of <see cref="Object"/> objects in a graph , or even an entire graph - to allow restoring states of objects later. (EG: undo).
    /// TODO: the history could be serialized to disk as well even...
    /// </summary>
    [System.Serializable]
    public abstract class GraphEditorHistory<T> : 
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
        /// Store 
        /// </summary>
        /// <param name="obj"></param>
        public void StoreState(T obj)
        {
            GetHistory(obj).StoreState(obj, m_MaxHistory);
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
        /// Retrieve (and if required, create one) a history for the specified <see cref="T"/>
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
        /// <param name="o"></param>
        public void ClearHistory(T o)
        {
            
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


        [Serializable]
        public class HistoryItem : 
            Internal.SerializableObject,
            IEquatable<T>
        {
            [SerializeField]
            String m_ReferenceID;

            [SerializeField]
            string m_TypeName;

            Type m_TypeValue;

            /// <summary>
            /// History of JSON serialized stated.
            /// </summary>
            List<string> m_History = new List<string>();



            public string ReferenceID   => m_ReferenceID;
            public string TypeName      => m_TypeName; 
            public int HistoryCount     => m_History.Count;

            public Type ValueType
            {
                get
                {
                    if (m_TypeValue == null && string.IsNullOrEmpty(m_TypeName))
                        return null;
                    if (m_TypeName == null)
                        m_TypeValue = ReferencedTypeSerializationHelper.TryGetKnownType(m_TypeName);
                    return null;
                }
            }

            public bool IsKnownType => ValueType != null;


            /// <summary>
            /// DO NOT USE: this is used by <see cref="GraphEditorHistory"/>.
            /// 
            /// Attempt overwrite the target object with the specified state stored in history.
            /// Lowest index is most recent in the object's history. Any history newer than the desired state is removed.
            /// as well as the restored state. Index must be between zero and <see cref="HistoryCount"/>.
            /// </summary> 
            internal bool TryRestoreState(int index, ref Object result)
            {
                try
                {
                    if (!result)
                    {
                        // TODO: find existing object of same GUID and TYPE in OBJECT.AllObjects
                        // before creating a new instance

                        if (ValueType == null) throw new Exception("Unknwon Type.");
                        result = (Object)System.Activator.CreateInstance(ValueType); 
                    }
                   
                    if (this.Equals(result) && m_History.Count > 0)
                    { 
                        // if out of range an exception will occur here:
                        JsonUtility.FromJsonOverwrite(m_History[index], result);

                        RemoveLatestStates(index);
                        return true;
                    }
                }
                catch(Exception ex)
                {
                    Debug.LogException(ex);
                }
                return false;
            }

            /// <summary>
            /// DO NOT USE: this is used by <see cref="GraphEditorHistory"/>.
            /// </summary>
            internal bool StoreState(T value, int maxHistory = 20)
            {
                if (!value || !this.Equals(value))
                    return false;
                m_History.Insert(0, JsonUtility.ToJson(value));
                while (m_History.Count > maxHistory)
                    m_History.RemoveAt(m_History.Count - 1);
                return true;
            }

            string GetLatestState()
            {
                if(m_History.Count > 0)
                {
                    return m_History[0];
                }
                return null;
            }

            void RemoveLatestState()
            {
                if (m_History.Count > 0)
                    m_History.RemoveAt(0);
            }

            void RemoveLatestStates(int newestState)
            {
                int i = 0;
                while(i++ < newestState && m_History.Count > 0)
                {
                    m_History.RemoveAt(0);
                }
            }

            public HistoryItem()
            {

            }

            public HistoryItem(Object o)
            {
                m_ReferenceID = o.GUID;

            }

            public override bool Equals(object obj)
            {
                if (obj is Object) return Equals((Object)obj);
                return false;
            }

            bool IEquatable<Object>.Equals(Object other)
            {
                return other && other.GUID == m_ReferenceID && other.GetType().FullName == m_TypeName;
            }
        }




    }
}
