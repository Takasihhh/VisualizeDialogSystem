using System;
using System.Collections.Generic;
using DialogSystem.Data;
using UnityEngine;
using UnityEngine.Events;

namespace DialogSystem.DataStructure
{
    using Utilits;
    public sealed partial class DialogTree
    {
        
        //TODO修改
        public partial bool TryGetChild(out NodeBase node, string key = "")
        {
            node = null;
            return false;
        }

        public NodeBase m_CurNode
        {
            get => _curNode ??= new NodeBase();
            set => _curNode = value; 
        }
        
        public SerializableStringListDictionary m_ChildrenTable
        {
            get =>childrenTable ??=new SerializableStringListDictionary();
            set => childrenTable = value;
        }
    }
    
    public  partial class NodeBase
    {
        
        public string m_textHeader
        {
            get =>_textHeader;
            private set=>_textHeader = value;
        }

        public DialogContext m_Context
        {
            get => _context;
            set => _context = value;
        }
        

        public string ID
        {
            get => _id;
            set => _id = value;
        }
        
        /// <summary>
        /// Header,ID
        /// </summary>
        public SerializableStringDictionary m_childIDDic
        {
            get => childIDDic ??= new SerializableStringDictionary();
            set => childIDDic = value;
        }
        public DialogNodeType m_nodeType => _nodeType;
        
    }
    
    public sealed partial class DialogNode
    {

        public bool Visible
        {
            get => _visible;
        }
        
        public DialogNode()
        {
            _nodeType = DialogNodeType.Dialog;
            _visible = true;
        }

        public DialogNode(string header)
        {
            _nodeType = DialogNodeType.Dialog;
            _textHeader = header;
            _visible = true;
        }

        public override bool AddChild(NodeBase node)
        {
            if (node == null || node.m_textHeader == "")
                return false;
            if (m_childIDDic.Count>0 && node.m_textHeader == "__EVENTNODE")
                return false;
            
            return m_childIDDic.TryAdd(node.m_textHeader, node.ID);
        }
    }
    
    
    public sealed partial class EventNode
    {
        public EventNode()
        {
            _nodeType = DialogNodeType.Event;
            _textHeader = "__EVENTNODE";
        }

        public EventNode(List<string> ids)
        {
            _nodeType = DialogNodeType.Event;
            _textHeader = "__EVENTNODE";
            _triggerEvtID = ids;
        }
        private List<string> m_TriggerEventID { get=>_triggerEvtID??=new List<string>(); set=>_triggerEvtID = value; }
        public override bool AddChild(NodeBase node)
        {
            if (node == null)
                return false;
            if (node.m_textHeader == "__EVENTNODE")
                return false;

            return m_childIDDic.TryAdd(node.m_textHeader, node.ID);
        }

        public override void OnBeforeSerialize()
        {
            // Debug.Log("调用事件节点的序列化");
            m_NodeInfo = new SerialzedNode
            {
                TextHeader = _textHeader,
                Context = _context,
                TriggerEventID = m_TriggerEventID
            };
        }
        
        public override void OnAfterDeserialize()
        {
            m_TriggerEventID = m_NodeInfo.TriggerEventID;
        }
    }
    
    [Serializable]
    public class SerialzedNode
    {
        public string TextHeader;
        public DialogContext Context;
        public List<string> TriggerEventID;
    }
}