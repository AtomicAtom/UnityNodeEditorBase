using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UNEB
{
    /// <summary>
    /// General Purpose Helpers.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public static class Helpers
    {


#region TryInvoke Extensions

        public static bool TryInvoke(this Action a)
        {
            if (a != null)
                try
                {
                    a();
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            return false; 
        }

        public static bool TryInvoke<T0>(this Action<T0> a, T0 arg0)
        {
            if (a != null)
                try
                {
                    a(arg0);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            return false;
        }


#endregion

        static List<object> m_TempList = new List<object>();

        /// <summary>
        /// Helper to cleanup a list of <see cref="Object"/> objects that are either null or marked as destroyed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inList"></param>
        public static void RemoveNullOrDestroyedEntries<T>(this IList<T> inList) where T : Object
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

        /// <summary>
        /// Helper for filling an array of <typeparamref name="TItem"/> instances or matching inherting instanced of the <typeparamref name="TItem"/> 
        /// in a collection of the <typeparamref name="TBaseType"/> type they inherit from. 
        /// </summary>
        /// <remarks>
        /// CodieMorgan: - my own comments confuse me sometimes when I can't think of a better way to describe it.
        /// </remarks>
        internal static void UpdateCachedArrayWithType<TBaseType, TItem>(this IEnumerable<TBaseType> collection, ref TItem[] cache) where TItem : TBaseType
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










        #region MakeHash

        /// <summary>
        /// Used by <see cref="MakeHash(object[])"/> methods to avoid allocations 
        /// </summary>
        static int m_h, m_i, m_c;

        static readonly int prime1 = unchecked((int)2166136261);
        static readonly int prime2 = 16777619;


 
        public static int MakeHash<T>(T[] v)
        {
            if (v == null) return 0;
            unchecked
            {
                m_h = prime1;
                m_c = v.Length;
                // we also hash the array length 
                m_h = m_h * prime2 + m_c.GetHashCode();
                for (m_i = 0; m_i < m_c; m_i++)
                    if (v[m_i] != null)
                        m_h = m_h * prime2 + v[m_i].GetHashCode();
                return m_h;
            }
        }



        public static int MakeHash<T0, T1>(T0 a, T1 b)
        { 
            unchecked
            {
                m_h = prime1;

                m_h = m_h * prime2 + a.GetHashCode();
                m_h = m_h * prime2 + b.GetHashCode();

                return m_h;
            }

        }


        public static int MakeHash<T0, T1, T2>(T0 a, T1 b, T2 c)
        { 
            unchecked
            {
                m_h = prime1;

                m_h = m_h * prime2 + a.GetHashCode();
                m_h = m_h * prime2 + b.GetHashCode();
                m_h = m_h * prime2 + c.GetHashCode();

                return m_h;
            }

        }

        public static int MakeHash<T0, T1, T2, T3>(T0 a, T1 b, T2 c, T3 d)
        { 
            unchecked
            {
                m_h = prime1;

                m_h = m_h * prime2 + a.GetHashCode();
                m_h = m_h * prime2 + b.GetHashCode();
                m_h = m_h * prime2 + c.GetHashCode();
                m_h = m_h * prime2 + d.GetHashCode();

                return m_h;
            }

        }

        public static int MakeHash<T0, T1, T2, T3, T4>(T0 a, T1 b, T2 c, T3 d, T4 e)
        { 
            unchecked
            {
                m_h = prime1;

                m_h = m_h * prime2 + a.GetHashCode();
                m_h = m_h * prime2 + b.GetHashCode();
                m_h = m_h * prime2 + c.GetHashCode();
                m_h = m_h * prime2 + d.GetHashCode();
                m_h = m_h * prime2 + e.GetHashCode();

                return m_h;
            }

        }

        public static int MakeHash<T0, T1, T2, T3, T4, T5>(T0 a, T1 b, T2 c, T3 d, T4 e, T5 f)
        { 
            unchecked
            {
                m_h = prime1;

                m_h = m_h * prime2 + a.GetHashCode();
                m_h = m_h * prime2 + b.GetHashCode();
                m_h = m_h * prime2 + c.GetHashCode();
                m_h = m_h * prime2 + d.GetHashCode();
                m_h = m_h * prime2 + e.GetHashCode();
                m_h = m_h * prime2 + f.GetHashCode();

                return m_h;
            }

        }

        public static int MakeHash<T0, T1, T2, T3, T4, T5, T6>(T0 a, T1 b, T2 c, T3 d, T4 e, T5 f, T6 g)
        { 
            unchecked
            {
                m_h = prime1;

                m_h = m_h * prime2 + a.GetHashCode();
                m_h = m_h * prime2 + b.GetHashCode();
                m_h = m_h * prime2 + c.GetHashCode();
                m_h = m_h * prime2 + d.GetHashCode();
                m_h = m_h * prime2 + e.GetHashCode();
                m_h = m_h * prime2 + f.GetHashCode();
                m_h = m_h * prime2 + g.GetHashCode();

                return m_h;
            }

        }

        public static int MakeHash<T0, T1, T2, T3, T4, T5, T6, T7>(T0 a, T1 b, T2 c, T3 d, T4 e, T5 f, T6 g, T7 h)
        { 
            unchecked
            {
                m_h = prime1;

                m_h = m_h * prime2 + a.GetHashCode();
                m_h = m_h * prime2 + b.GetHashCode();
                m_h = m_h * prime2 + c.GetHashCode();
                m_h = m_h * prime2 + d.GetHashCode();
                m_h = m_h * prime2 + e.GetHashCode();
                m_h = m_h * prime2 + f.GetHashCode();
                m_h = m_h * prime2 + g.GetHashCode();
                m_h = m_h * prime2 + h.GetHashCode();

                return m_h;
            }

        }

        public static int MakeHash<T0, T1, T2, T3, T4, T5, T6, T7, T8>(T0 a, T1 b, T2 c, T3 d, T4 e, T5 f, T6 g, T7 h, T8 i)
        { 
            unchecked
            {
                m_h = prime1;

                m_h = m_h * prime2 + a.GetHashCode();
                m_h = m_h * prime2 + b.GetHashCode();
                m_h = m_h * prime2 + c.GetHashCode();
                m_h = m_h * prime2 + d.GetHashCode();
                m_h = m_h * prime2 + e.GetHashCode();
                m_h = m_h * prime2 + f.GetHashCode();
                m_h = m_h * prime2 + g.GetHashCode();
                m_h = m_h * prime2 + h.GetHashCode();
                m_h = m_h * prime2 + i.GetHashCode();

                return m_h;
            }

        }

        public static int MakeHash<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(T0 a, T1 b, T2 c, T3 d, T4 e, T5 f, T6 g, T7 h, T8 i, T9 j)
        { 
            unchecked
            {
                m_h = prime1;

                m_h = m_h * prime2 + a.GetHashCode();
                m_h = m_h * prime2 + b.GetHashCode();
                m_h = m_h * prime2 + c.GetHashCode();
                m_h = m_h * prime2 + d.GetHashCode();
                m_h = m_h * prime2 + e.GetHashCode();
                m_h = m_h * prime2 + f.GetHashCode();
                m_h = m_h * prime2 + g.GetHashCode();
                m_h = m_h * prime2 + h.GetHashCode();
                m_h = m_h * prime2 + i.GetHashCode();
                m_h = m_h * prime2 + j.GetHashCode();

                return m_h;
            } 
        }

        /// <summary>
        /// 
        /// This overload will cause boxing, and should be avoided if possible, but will accept virtually "unlimited" parameters of mixed types.
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        /// <remarks>
        /// CodieMorgan: 
        ///  - I suggest using the Generic MakeHash alternatives instead since I actually took the time to create variations with up to 10 generic parameter types.
        /// </remarks>
        public static int MakeHash(params object[] v)
        {
            if (v == null) return 0;
            unchecked
            {

                m_h = prime1;
                m_c = v.Length;
                // we also hash the array length
                m_h = m_h * prime2 + m_c.GetHashCode();
                for (m_i = 0; m_i < m_c; m_i++)
                    if (v[m_i] != null)
                        m_h = m_h * prime2 + v[m_i].GetHashCode();
                return m_h;
            }

        }

        #endregion





        #region String Extensions


        /// <summary>
        /// Convert a "CamelCasePhrase" into a "Camel Case Phrase". Hopefully behaves exactly the same way as the Unity Editor's internal equivelant.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="tryPreserveAcronyms"></param>
        /// <param name="tryPreserveFullCapWords"></param>
        /// <returns></returns>
        /// 
        /// <remarks>
        /// BUG: (CodieMorgan) somewhere in here I screwed up a regex and I'm too braindead to fix it. 
        ///       - either TryPreserveFullCapWords or TryPreserveAcronyms was broken the last time I checked. 
        ///       - for now just use this method with default bool params (false).
        /// </remarks>
        public static string UnCamel(this string s, bool tryPreserveAcronyms = false, bool tryPreserveFullCapWords = false)
        {
            return Regex.Replace
            (
                s,
                tryPreserveFullCapWords
                    ? "(?<=[A - Z])([A - Z])(?=[a - z])"
                    : tryPreserveAcronyms
                        ? "(?<=[a-z])([A-Z])"
                        : "(\\B[A-Z])",
                " $1"
            );
        }

        /// <summary>
        /// Pretty-fy (UnCamel) a bunch of strings.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="tryPreserveAcronyms"></param>
        /// <param name="tryPreserveFullCapWords"></param>
        /// <returns></returns>
        public static string[] UnCamel(this string[] s, bool tryPreserveAcronyms = false, bool tryPreserveFullCapWords = false)
        {
            string[] r = new string[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                r[i] = s[i].UnCamel(tryPreserveAcronyms, tryPreserveFullCapWords);
            }
            return r;
        }


        #endregion

    }
}
