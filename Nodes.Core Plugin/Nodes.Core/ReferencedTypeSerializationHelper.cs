using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
namespace Nodes.Core
{
    /// <summary>
    /// codiemorgan: For fuck sakes...
    /// </summary> 
    static internal class ReferencedTypeSerializationHelper
    {
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
        /// Try to find - and register, all external loaded types inheriting from <see cref="ReferencedType"/>
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

                    // Skip these assembly names:
                    if
                    (
                        ass.FullName == "UnityEngine" ||
                        ass.FullName == "UnityEngine.UI" ||
                        ass.FullName == "UnityEngine.Graphs" ||
                        ass.FullName == "UnityEditor"
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
        /// Alternative to try find types that can be found in assembly - but not found because of a UnityScript outside of assembly.
        /// 
        /// This is so we cann correctly serialize/deserialize mixed inheriting <see cref="GraphObject"/> array element types independently;
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
    }


 

}
