using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
namespace UNEB.Editor
{
    public static class EditorHelpers
    {



        public static SerializedObject ToSerializedObject(this Graph graph)
        {
            if (!graph) return null;
            return new SerializedObject(graph as object as UnityEngine.Object);
        }


    }
}
