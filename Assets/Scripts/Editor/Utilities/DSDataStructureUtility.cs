using System;
using System.Collections.Generic;
using System.Linq;
using DialogSystem.Data;
using DialogSystem.Utilies;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DialogSystem.Editor.Utilites
{
    using DataStructure;
    using GraphicsElements;
    using Utilits;

    public static class DSDataStructureUtility
    {
        
        /// <returns></returns>
        public static bool BuildTree(this DSNode rootNode)
        {
            IOrderedEnumerable<Edge> vEnumrator;
            // Debug.Log(rootNode.GetOutputConnection());
            Debug.Log(rootNode);
            vEnumrator = rootNode.GetOutputConnection()?
                .OrderBy((edge) => edge.resolvedStyle.top);
            if (vEnumrator == null)
                return false;
            using var iterator = vEnumrator.GetEnumerator();
             
            while (iterator.MoveNext())
            {
                if (iterator.Current == null)
                    break;
                var cnode = iterator.Current.input.node;
                //iterator.MoveNext();
                if (cnode is DSNode node)
                {
                    if (DSIOUtility.TryGetNode(node.ID, out NodeBase res) &&
                        DSIOUtility.TryGetNode(rootNode.ID, out NodeBase parent))
                    {
                        //添加一个子节点
                        res.ID = node.ID;
                        parent.AddChild(res);
                        // foreach (var VARIABLE in parent.m_child)
                        // {
                        //     Debug.Log("这个节点" +(VARIABLE.Value is DialogNode) + "" + (VARIABLE.Value is EventNode));
                        // }
                        //子节点递归建树
                        node.BuildTree();
                        //是事件节点直接跳出
                        if (node.NodeType == DialogNodeType.Event)
                            break;
                    }
                }
            }
            Debug.Log(1);
            
            return true;
        }


        public static void RegisterNode(this DSNode node)
        {
            // Type nodeType = Type.GetType($"DialogSystem.DataStructure.{node.NodeType}Node");
            // NodeBase newNode = (NodeBase)ReflectionMethodExtension.CreateInstance($"{node.NodeType}Node");;
            // Debug.LogError($"{node.NodeType}Node");

            // NodeBase newNode = Object.Instantiate<NodeBase>();
            // Debug.Log(newNode == null);
            NodeBase newNode;
            if (node.NodeType == DialogNodeType.Dialog)
            {
                newNode = new DialogNode();
                Debug.Log("对话节点");
            }
            else
            {
                newNode = new EventNode();
            }
            
            Debug.Log(newNode == null);
            DialogContext context = new DialogContext
            {
                m_contextInfos = new List<ContextInfo>()
            };
            newNode.m_Context = context;
            DSIOUtility.AddNodeData(node,newNode);
        }

    }
}