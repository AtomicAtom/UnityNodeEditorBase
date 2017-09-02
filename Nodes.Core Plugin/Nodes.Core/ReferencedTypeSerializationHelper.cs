﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
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
            if(!m_AreTypesInitialized)
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
                    )   continue;

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
    }


 

}
