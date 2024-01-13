    
using System;
using System.Collections.Generic;
using DialogSystem.Data;
using DialogSystem.DataStructure;
using UnityEngine;

public class BuildTreeTest:MonoBehaviour
{
    [SerializeField] private DialogTreeData_SO _data;

    private string debugResult = "";
    private void Start()
    {
        // TraceTree(_data.m_Tree.m_CurNode);
        Debug.LogError(debugResult);
    }


    [ContextMenu("测试树")]
    private void DebugTree()
    {
        debugResult = "";
        _data.m_Tree.ResetTree();
        TraceTreeByList(_data.m_Tree.m_CurNode);
        Debug.LogError(debugResult);
        _data.m_Tree.ResetTree();
    }
    
    // private void TraceTree(NodeBase node)
    // {
    //     debugResult += "父节点名称" + node.m_textHeader + node.m_nodeType+ (node is DialogNode) + (node is EventNode) + "\n";
    //     foreach (var pair in node.m_child)
    //     {
    //         debugResult += "    子节点名称: " +pair.Key + pair.Value.m_nodeType + (pair.Value is DialogNode) + (pair.Value is EventNode) + "父"+ node.m_textHeader + "\n";
    //         if (pair.Value.m_child.Count > 0)
    //         {
    //             TraceTree(pair.Value);
    //         }
    //     }
    // }

    private void TraceTreeByList(NodeBase node)
    {
        debugResult += "父节点名称" + node.m_textHeader + node.m_nodeType+ (node is DialogNode) + (node is EventNode) + "\n";
        foreach (var pair in node.m_childIDDic)
        {
            //
            // debugResult +=  "    子节点名称: " + _data.m_Tree.m_NodeDic[pair.Value].m_textHeader + 
            //                 (_data.m_Tree.m_NodeDic[pair.Value] is DialogNode) + (_data.m_Tree.m_NodeDic[pair.Value] is EventNode) 
            //                 + "父亲" + node.m_textHeader + "\n";

            if (_data.m_Tree.FindNodeByID(pair.Value, out NodeBase res))
            {
                debugResult +=  "    子节点名称: " + res.m_textHeader + 
                                (res is DialogNode) + (res is EventNode) 
                                + "父亲" + node.m_textHeader + "\n";
            }
            else return;
            if (_data.m_Tree.m_ChildrenTable.TryGetValue(pair.Value, out List<string> childID))
            {
                if (childID.Count > 0)
                {
                    TraceTreeByList(res);
                }
            }
        }
    }
}

[Serializable]
public struct TestStruct
{
    public DialogContext info;
    public string text;
}
