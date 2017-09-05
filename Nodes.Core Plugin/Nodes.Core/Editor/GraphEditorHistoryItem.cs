using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UNEB.Editor
{
    /*
     * TODO: Histroy Item should probably have a "Note" field.
     */
    public abstract partial class History<T> where T : Object
    {
        [Serializable]
        public sealed class HistoryItem :
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



            public string ReferenceID => m_ReferenceID;
            public string TypeName => m_TypeName;
            public int HistoryCount => m_History.Count;

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
            /// DO NOT USE: this is used by <see cref="History{T}"/>.
            /// 
            /// Attempt overwrite the target object with the specified state stored in history.
            /// Lowest index is most recent in the object's history. Any history newer than the desired state is removed.
            /// as well as the restored state. Index must be between zero and <see cref="HistoryCount"/>.
            /// </summary> 
            internal bool TryRestoreState(int index, ref T result)
            {
                try
                {
                    if (!result)
                    {
                        // TODO: find existing object of same GUID and TYPE in OBJECT.AllObjects
                        // before creating a new instance

                        if (ValueType == null) throw new Exception("Unknwon Type.");
                        result = (T)System.Activator.CreateInstance(ValueType);
                    }

                    if (this.Equals(result) && m_History.Count > 0)
                    {
                        // if out of range an exception will occur here:
                        JsonUtility.FromJsonOverwrite(m_History[index], result);

                        RemoveLatestStates(index);
                        return true;
                    }
                }
                catch (Exception ex)
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
                if (m_History.Count > 0)
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
                while (i++ < newestState && m_History.Count > 0)
                {
                    m_History.RemoveAt(0);
                }
            }

            public HistoryItem()
            {

            }

            public HistoryItem(T o)
            {
                m_ReferenceID = o.GUID;

            }

            public void Clear()
            {
                m_History.Clear();
            }

            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                if (obj is T || obj.GetType().IsSubclassOf(typeof(T))) 
                    return Equals((T)obj);
                return false;
            }

         

            public bool Equals(T other)
            {
                return other.GUID == m_ReferenceID;
            }
        } 
    } 
}
