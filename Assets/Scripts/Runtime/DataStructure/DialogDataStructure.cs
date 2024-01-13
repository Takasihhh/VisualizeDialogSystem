using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace DialogSystem.DataStructure
{
    using Data;
    using Utilits;
    public sealed partial class DialogTree
    {
        //================================================Field
        [SerializeReference]private NodeBase _rootNode;
        [SerializeReference]private NodeBase _curNode;
        [SerializeReference]private SerializableStringListDictionary childrenTable;
        [SerializeReference] private List<NodeBase> _nodeList;
        [SerializeReference] private SerializableIndexDictionary _indexDic;
        
        public DialogTree(NodeBase rootNode)
        {
            _rootNode = rootNode;
            _curNode = _rootNode;
        }

        public DialogTree(NodeBase rootNode, SerializableStringListDictionary table, SerializableNodeDictionary nodeDic)
        {
            _rootNode = rootNode;
            _curNode = _rootNode;
            childrenTable = table;
            _nodeList = new List<NodeBase>();
            _indexDic = new SerializableIndexDictionary();
            foreach (var pair in nodeDic)
            {
                _nodeList.Add(pair.Value);
                _indexDic.Add(pair.Key,_nodeList.Count-1);
            }
        }

        public bool FindNodeByID(string id,out NodeBase node)
        {
            node = null;
            if (_indexDic.TryGetValue(id, out int result))
            {
                node = _nodeList[result];
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// 通过选项移动当前节点
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public bool MoveCurNodeByHeader(string header)
        {
            if (_curNode.TryGetChildIDByHeader(header, out string id))
            {
                FindNodeByID(id,out _curNode);
                return true;
            }
            return false;
        }
        
        public void ResetTree()
        {
            _curNode = _rootNode;
        }
        //==================================================MemberMethod
        public partial bool TryGetChild(out NodeBase node,string key = "");
        
    }

    [Serializable]
    public partial class NodeBase:ISerializationCallbackReceiver
    {
        //==================================================Serialized
        [field: SerializeField] public SerialzedNode m_NodeInfo;
        [SerializeReference]protected SerializableStringDictionary childIDDic;
        
        //================================================Field
        protected string _textHeader;
        protected DialogContext _context;
        protected string _id;
        protected DialogNodeType _nodeType;
        //==================================================MemberMethod
        public virtual bool AddChild(NodeBase node) => false;
        public virtual void OnBeforeSerialize()
        {
            m_NodeInfo = new SerialzedNode
            {
                TextHeader = _textHeader,
                Context = _context,
            };

            if (this is EventNode node)
            {
                node.OnBeforeSerialize();
            }
        }

        public virtual void OnAfterDeserialize()
        {
            _textHeader = m_NodeInfo.TextHeader;
            _context = m_NodeInfo.Context;

            if (this is EventNode node)
            {
                // node._triggerEvtID = m_NodeInfo.TriggerEventID;
                node.OnAfterDeserialize();
            }
        }

        public bool TryGetChildIDByHeader(string header,out string id)
        {
            return childIDDic.TryGetValue(header,out id);
        }
    }
    
    
    [Serializable]
    public sealed partial class DialogNode:NodeBase
    {
        //================================================Field
        [SerializeField]private bool _visible;
        [SerializeField]private bool _onRead;
        
    }
    
    [Serializable]
    public sealed partial class EventNode:NodeBase
    {
        [SerializeReference] public List<string> _triggerEvtID;
    }
}