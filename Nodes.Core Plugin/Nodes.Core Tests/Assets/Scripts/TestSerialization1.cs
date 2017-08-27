using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nodes.Core;
 
public class TestSerialization1 : MonoBehaviour
{

    [SerializeField]
    Graph m_TestGraph;

	// Use this for initialization
	void Start ()
    { 
    }


    [ContextMenu("Add Test GraphObjects")]
    void Test1()
    {
        if(!m_TestGraph)
            m_TestGraph = ReferencedType.CreateInstance<Graph>();

        for (int i = 0; i < 10; i++)
        {
            m_TestGraph.AddObject<TestNode>();
        }

        for (int i = 0; i < 10; i++)
        {
            m_TestGraph.AddObject<Connection>();
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    [System.Serializable]
    public class TestNode : Node
    {
        // some random fields to verify mixed collection type serialization and deserialization.

        public int A = 6,
            B = 7;

        [Range(0, 1)]
        public float
            C,
            D,
            E,
            F,
            G;


        // test events

        public TestNode()
        {
            // These two events are working:
            OnBeforeSerialization += TestNode_OnBeforeSerialization;
            OnAfterDeserialization += TestNode_OnAfterDeserialization;
        }

        private void TestNode_OnAfterDeserialization()
        {
            //Debug.Log(Name + " - serialize test");
        }

        private void TestNode_OnBeforeSerialization()
        {
            //Debug.Log(Name + " - deserialize test");
        }
    }
      

}
