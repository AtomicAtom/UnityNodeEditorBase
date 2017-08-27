using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
namespace Nodes.Core
{
    /// <summary>
    /// 
    /// </summary> 
    /// <remarks>
    /// codiemorgan: This static class is probably like a magic school buss.
    /// </remarks>
    static internal class ReferencedTypeSerializationHelper
    {

        static List<Type> m_TempTypes = new List<Type>();

        ///// <summary>
        ///// Static Cache of serializable fields in an object
        ///// </summary>
        //static Dictionary<string, FieldInfo[]> m_FieldCache = new Dictionary<string, FieldInfo[]>();

        //static List<FieldInfo> m_TmpMembers = new List<FieldInfo>();
        //static List<FieldInfo> m_TmpMembers2 = new List<FieldInfo>();


        //static internal FieldInfo[] GetSerializableFields(this ReferencedType t)
        //{
        //    if (!m_FieldCache.ContainsKey(t.GetType().FullName))
        //    {
        //        m_FieldCache.Add(t.GetType().FullName, FindSerializableFields(t.GetType()));
        //    }
        //    return m_FieldCache[t.GetType().FullName];
        //}


        //static internal FieldInfo[] FindSerializableFields(Type t)
        //{
        //    m_TmpMembers.Clear();
        //    m_TmpMembers2.Clear();
        //    m_TmpMembers.AddRange(t.GetFields(BindingFlags.Public | BindingFlags.NonPublic));
        //    FieldInfo current;
        //    while (m_TmpMembers.Count > 0)
        //    {
        //        current = m_TmpMembers[0];
        //        if (FilterFieldMemberInfo(current, null))
        //        {
        //            m_TmpMembers2.Add(current);
        //        }
        //    }
        //    return m_TmpMembers2.ToArray();
        //}

        //static bool FilterFieldMemberInfo(FieldInfo f, object unused)
        //{
        //    // TODO: any other filtering stuff.
        //    if (CanSerializeFieldAttribute(f))
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        //static bool CanSerializeFieldAttribute(FieldInfo f)
        //{
        //    if (f.IsStatic || f.IsInitOnly || f.IsNotSerialized)
        //        return false;

        //    if (f.IsPublic) return true;

        //    object[] attrib = f.GetCustomAttributes(true);

        //    for (int i = 0; i < attrib.Length; i++)
        //    {
        //        if (attrib[i] is NonSerializedAttribute) return false;
        //        if (attrib[i] is SerializeField) return true;
        //    }
        //    return false;
        //}

        static bool m_AreTypesInitialized;

        /// <summary>
        /// helper which attempts to find and register, all external loaded types inheriting from <see cref="ReferencedType"/> 
        /// in all .NET/Moino assembies (including asset script assembly) currently loaded by Unity
        /// so we can actually recognize ALL serialized objects in a graph correctly on deserialization.
        /// </summary>
        static internal void CheckSerializationInitialized()
        {
            if(!m_AreTypesInitialized)
            {
                Assembly myAssembly = Assembly.GetAssembly(typeof(ReferencedType));
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
            if (t.IsSubclassOf(typeof(ReferencedType)) || typeof(ReferencedType).IsAssignableFrom(t))
                return true;
            return false;
        }


        /// <summary>
        /// Cached dictionary of all known types that inherit from <see cref="ReferencedType"/>
        /// </summary>
        static Dictionary<string, Type> m_KnownTypes = new Dictionary<string, Type>();


        internal static void MarkKnownType(ReferencedType obj)
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

        internal static Type TryGetKnownType(string typeName)
        {
            return m_KnownTypes.ContainsKey(typeName)
                ? m_KnownTypes[typeName]
                : Type.GetType(typeName, false);
        }





        /// <summary>
        /// Helper to get all types that inherit from '<typeparamref name="T"/>, <see cref="ReferencedType"/>'.
        /// The results include types in ALL loaded .NET/MONO assemblies currently loaded by unity - including those defined in loose
        /// C# scripts within the assets folder.
        /// </summary>
        public static Type[] GetAllInheritedTypes<T>() where T : ReferencedType
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
