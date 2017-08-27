using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nodes.Core
{
    public static class Helpers
    {


        public static void TryInvoke(this Action a)
        {
            if (a != null)
                try
                {
                    a();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
        }

        public static void TryInvoke<T0>(this Action<T0> a, T0 arg0)
        {
            if (a != null)
                try
                {
                    a(arg0);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
        }




        static List<object> m_TempList = new List<object>();


        public static void RemoveNullOrDestroyedEntries<T>(this IList<T> inList) where T : ReferencedType
        {
            int i = 0;
            while(i < inList.Count)
            {
                if(!inList[i])
                {
                    inList.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        public static void UpdateCachedArrayWithType<TBaseType, TItem>(this IEnumerable<TBaseType> collection, ref TItem[] cache) where TItem : TBaseType
        {
            m_TempList.Clear();
            IEnumerator<TBaseType> enumerator = collection.GetEnumerator();
            while(enumerator.MoveNext())
            {
                if(enumerator.Current is TItem || enumerator.Current.GetType().IsSubclassOf(typeof(TItem)))
                {
                    m_TempList.Add(enumerator.Current);
                }
            }

            int count = m_TempList.Count;

            if (cache == null) cache = new TItem[count];
            if (cache.Length != count) Array.Resize(ref cache, count);
            int i = 0;
            while(m_TempList.Count > 0)
            {
                cache[i++] = (TItem)m_TempList[0];
                m_TempList.RemoveAt(0); 
            } 
        }


        public static void CopyNonAlloc<T>(this T[] src, ref T[] destination)
        {
            if (destination == null) destination = new T[src.Length];
            if (destination.Length != src.Length) Array.Resize(ref destination, src.Length);
            src.CopyTo(destination, 0);
        }
    }
}
