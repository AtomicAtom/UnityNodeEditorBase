using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace UNEB.Editor.Windows
{
    /// <summary>
    /// Baseclass for a <see cref="Graph"/> editor.
    /// </summary>
    /// <remarks>
    /// Node - since we are no longer dealing with <see cref="ScriptableObject"/> but instead Serializable classes - initializing a new SerializableObject from a 
    /// UnityEngine.Object type is not possible. 
    /// </remarks>
    class GraphEditorBaseWindow : EditorWindow
    {
        static GraphEditorBaseWindow m_Instance;

        public static GraphEditorBaseWindow Instance => m_Instance;


        Graph m_Graph; 
        SerializedObject m_SerializedObject;
        string m_LastGraphID;

        internal Graph Graph => m_Graph;

        protected SerializedObject SerializedObject
        {
            get
            {
                if (!HasGraph) return null;
                if(HasGraphInstanceChanged)
                {
                    // I do this because SeializedObject does not give is a way to determine if it has been disposed.
                    if (m_SerializedObject != null)
                        try { m_SerializedObject.Dispose(); } catch { } 

                    m_SerializedObject = m_Graph.ToSerializedObject();
                    m_LastGraphID = m_Graph.GUID;
                }
                return m_SerializedObject;
            }
        }


        string GraphGUID => m_Graph ? m_Graph.GUID : null;
        string LastGraphGUID => m_LastGraphID;

        bool HasGraph => m_Graph;
        bool HasGraphInstanceChanged
        {
            get
            {
                return (string.IsNullOrEmpty(m_LastGraphID) || string.IsNullOrEmpty(GraphGUID)) || m_LastGraphID != GraphGUID;
            }
        }

        public static void ShowEditor(Graph graph)
        {
            if (!graph) return;
            if (m_Instance) 
                m_Instance.Show();
            else 
                m_Instance = EditorWindow.GetWindow<GraphEditorBaseWindow>();
        }


        private void OnGUI()
        {
            if(!HasGraph)
            {
                EditorGUILayout.HelpBox("No Graph Loaded", MessageType.Error);
            }
            else
            {
                IEnumerator<GraphObject> enumerator = Graph.GetEnumerator();
                int pos = 0;
                while(enumerator.MoveNext())
                {
                    EditorGUILayout.LabelField
                    (
                        string.Format("{0:00}: {1} ({2})", ++pos, enumerator.Current.GetType().FullName, enumerator.Current.GUID)
                    );
                }
            }
        }

    }
}
