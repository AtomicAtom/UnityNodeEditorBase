using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using UNEB.Internal;

namespace UNEB
{
    /// <summary>
    /// 
    /// </summary> 
    /// <remarks>
    /// codiemorgan: This static class is probably like a magic school buss.
    /// </remarks>
    static internal class ReferencedTypeSerializationHelper
    {
        /// <summary>
        /// The base type of all our custom serializable objects.
        /// </summary>
        static readonly Type BaseType = typeof(Internal.SerializableObject);

        static List<Type> m_TempTypes = new List<Type>();

        static bool m_AreTypesInitialized;

        /// <summary>
        /// helper which attempts to find and register, all external loaded types inheriting from <see cref="Object"/> 
        /// in all .NET/Moino assembies (including asset script assembly) currently loaded by Unity
        /// so we can actually recognize ALL serialized objects in a graph correctly on deserialization.
        /// </summary>
        static internal void CheckSerializationInitialized()
        {
            // We do this only once and cache all the types.
            if (!m_AreTypesInitialized)
            {
                Assembly myAssembly = Assembly.GetAssembly(typeof(Object));
                Assembly context = Assembly.GetEntryAssembly();
                Assembly[] loaded = AppDomain.CurrentDomain.GetAssemblies();
                //List<System.Type> t = new List<Type>();
                foreach (Assembly ass in loaded)
                {
                    if (ass.FullName == myAssembly.FullName)
                        continue; // skip our own assembly

                    // Ignore these assembly names:
                    if
                    (
                        ass.FullName == "UnityEngine" ||
                        ass.FullName == "UnityEditor" ||
                        ass.FullName == "UnityPlayer" || // soon(tm)
                        ass.FullName.Contains("UnityEngine.") ||
                        ass.FullName.Contains("UnityEditor.")
                    ) continue;

                    // cannot use GetExportedTypes() as an exception is thrown from dynamically loaded assemblies.
                    foreach (Type type in ass.GetTypes().Where(FilterType))
                    {
                        //t.Add(type);
                        MarkKnownType(type);
                    }
                }
                m_AreTypesInitialized = true;
            }

        }

        static bool FilterType(Type t)
        {
            if (t.IsAbstract) return false;
            if (t.IsSubclassOf(BaseType) || BaseType.IsAssignableFrom(t))
                return true;
            return false;
        }


        /// <summary>
        /// Cached dictionary of all known types that inherit from <see cref="Object"/>
        /// </summary>
        static Dictionary<string, Type> m_KnownTypes = new Dictionary<string, Type>();


        internal static void MarkKnownType(Object obj)
        {
            if (!obj) return;
            MarkKnownType(obj.GetType());
        }

        internal static void MarkKnownType(Type type)
        {
            if (type == null) return;
            if (!m_KnownTypes.ContainsKey(type.FullName))
                m_KnownTypes.Add(type.FullName, type);
        }

        /// <summary>
        /// Get known <see cref="Internal.SerializableObject"/> type.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        internal static Type TryGetKnownType(string typeName)
        {
            return m_KnownTypes.ContainsKey(typeName)
                ? m_KnownTypes[typeName]
                : Type.GetType(typeName, false);
        }





        /// <summary>
        /// Helper to get all types that inherit from '<typeparamref name="T"/>, <see cref="Object"/>'.
        /// The results include types in ALL loaded .NET/MONO assemblies currently loaded by unity - including those defined in loose
        /// C# scripts within the assets folder.
        /// </summary>
        public static Type[] GetAllInheritedTypes<T>() where T : Internal.SerializableObject
        {
            CheckSerializationInitialized();
            m_TempTypes.Clear();
            Type desiredType = typeof(T);
            foreach
            (
                KeyValuePair<string, Type> kv in m_KnownTypes.Where
                (
                    (KeyValuePair<string, Type> k) =>
                    {
                        return !k.Value.IsAbstract && (k.Value.IsSubclassOf(desiredType) || desiredType.IsAssignableFrom(k.Value));
                    }
                )
            )
            {
                m_TempTypes.Add(kv.Value);
            }
            return m_TempTypes.ToArray();
        }







        static List<object> tmp_Objects = new List<object>();
        static object[] tmp_ObjectArray = new object[0];

        ///// <summary>
        ///// Purge Null entries in array.
        ///// </summary> 
        //public static void PurgeNullEntries<T>(this T[] inArray)
        //{
        //    if (inArray == null || inArray.Length < 1) return;
        //    tmp_Objects.Clear();
        //    object curr;
        //    bool isInternalType = (typeof(T) == typeof(SerializableObject) || typeof(T).IsSubclassOf(typeof(SerializableObject)));
        //    bool isUnityType    = (typeof(T).IsSubclassOf(typeof(UnityEngine.Object)));

        //    for (int i = 0; i < inArray.Length; i++)
        //    {
        //        curr = inArray[i];

        //        if      (isInternalType && !(SerializableObject)(object)curr)
        //            continue;
        //        else if (isUnityType && !(UnityEngine.Object)(object)curr)
        //            continue;
        //        else if (curr == null)
        //            continue;

        //        tmp_Objects.Add(curr); 
        //    }

        //    Array.Resize(ref inArray, tmp_Objects.Count);

        //    // Not sure if this will work: If not , comment it out and replace with below version:
        //    (tmp_Objects as List<T>).CopyTo(inArray);

        //    /* UNCOMMENT BELOW IF ABOVE DOES NOT WORK */

        //    //Array.Resize(ref tmp_ObjectArray, tmp_Objects.Count);
        //    //tmp_Objects.CopyTo(tmp_ObjectArray, 0);
        //    //((T[])(object)tmp_Objects).CopyTo(inArray, 0);
        //}

        ///// <summary>
        ///// Purge Null entries in list.
        ///// </summary> 
        public static void PurgeNullEntries<T>(this List<T> input)
        {
            if (input == null || input.Count < 1) return; 
            object curr;
            bool isInternalType = (typeof(T) == typeof(SerializableObject) || typeof(T).IsSubclassOf(typeof(SerializableObject)));
            bool isUnityType = (typeof(T).IsSubclassOf(typeof(UnityEngine.Object)));

            for (int i = 0; i < input.Count; )
            {
                curr = input[i];

                if( 
                    (isInternalType && !(SerializableObject)(object)curr) ||
                    (isUnityType && !(UnityEngine.Object)(object)curr)    || 
                    (curr == null)
                )
                {
                    input.RemoveAt(i);
                    continue;
                }
                i++;
            }
        }

        //public static void PurgeNullEntries(this SerializableObject[] inArray)
        //{
        //    PurgeNullEntries<SerializableObject>(inArray);
        //}



        public static void Serialize_PerItem<T>(this List<T> input, ref string[] resultTypes, ref string[] resultSerialized)
        {
            input.PurgeNullEntries();
            int c = input.Count;
            if (resultTypes      == null) resultTypes      = new string[c];
            if (resultSerialized == null) resultSerialized = new string[c];

            if (resultTypes     .Length != c) Array.Resize(ref resultTypes     , c);
            if (resultSerialized.Length != c) Array.Resize(ref resultSerialized, c);

            T current;
            for (int i = 0; i < c; i++)
            {
                current = input [i];
                resultTypes     [i] = current.GetType().ToString();
                resultSerialized[i] = JsonUtility.ToJson(current);
            }
        }

        public static void Deserialize_PerItem<T>(this List<T> result, string[] types, string[] serialized, bool allowFallbackTobaseType = true)
        {
            System.Type currentType;
            int max = Math.Min(types.Length, serialized.Length);
            result.Clear();
            for(int i = 0; i < max; i++)
            {
                currentType = Type.GetType(types[i], false);
                if (currentType == null) currentType = TryGetKnownType(types[i]);

                if(currentType == null)
                {
                    Debug.LogError(string.Format("Could not deserialize object of type '{0}'. The type could not be found.", types[i]));
                    
                    if (allowFallbackTobaseType)
                        currentType = typeof(T); // use base type:
                    else
                        continue;
                }
                T item = (T)JsonUtility.FromJson(serialized[i], currentType);
                if (item != null)
                    result.Add(item);
            }
        }


    }


 

}
