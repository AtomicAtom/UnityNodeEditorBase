using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace UNEB.Example_Usage
{
    /// <summary>
    /// Basic Example which demonstrates what a tree of nodes nested inside of other nodes (A Node Tree) might look like.
    /// </summary>
    [Serializable]
    class TreeNode : Node
    {
        /// <summary>
        /// a reference to the parent node - stored as a reference. This must be set when a treenode is added to its parent treenode.
        /// </summary>
        [SerializeField]
        Reference m_ParentNode;

        /// <summary>
        /// Children of a treenode are stored here directly instead of in the parent graph. But since References are tracked by GUIDS - 
        /// then creating connections between nodes inside a node tree to a graph containing the treenode would be a simple matter.
        /// </summary>
        [SerializeField]
        List<TreeNode> m_ChildNodes;


        public event Action<TreeNode>
            OnAddChild,
            OnChildRemoved,
            OnParentChanged;

        public TreeNode Parent
        {
            get
            { 
                return m_ParentNode.TryGetValueAsType<TreeNode>(); 
            }
            private set
            {
                if (IsDestroyed) return;
                if(m_ParentNode != (Reference)value)
                {
                    m_ParentNode = value;
                    OnParentChanged.TryInvoke(value);
                }
            }
        }


        /*
         * TODO: Treenode Members here.
         */

        public T AddChild<T>() where T : TreeNode
        {
            T child = CreateInstance<T>();
            AddChild(child);
            return child;
        }

        public void AddChild(TreeNode child)
        {
            m_ChildNodes.Add(child);
            OnAddChild.TryInvoke(child);
            child.Parent = this;
        }

        public bool RemoveChild<T>(T child) where T : TreeNode
        {
            if(m_ChildNodes.Remove(child))
            {
                OnChildRemoved.TryInvoke(child);
                child.Parent = null;
                return true;
            }
            return false;
        }

        protected override void OnCreated()
        {
            base.OnCreated();
            OnDestroy += TreeNode_OnDestroy;
        }

        private void TreeNode_OnDestroy()
        {
            if(Parent) 
                Parent.RemoveChild(this); 
            foreach (TreeNode child in m_ChildNodes)
                child.Destroy();
        }
    }
}
