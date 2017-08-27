using System;
using System.Collections.Generic;
using UnityEngine;


namespace Nodes.Core
{
    [Serializable]
    public class Connection : GraphObject
    {
        [SerializeField, HideInInspector]
        Reference
            m_NodeA,
            m_NodeB;

        public Node Owner
        {
            get
            { 
                return m_NodeA.TryGetValueAsType<Node>();
            }
            set
            {
                m_NodeA = value;
            }
        }

        public Node Target
        {
            get
            {
                return m_NodeB.TryGetValueAsType<Node>();
            }
            set
            {
                m_NodeB = value;
            }
        }

        // todo: other connection stuff: (input/output here?)

    }
}
