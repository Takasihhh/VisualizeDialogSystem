using System;
using System.Collections.Generic;
using System.Linq;
using DialogSystem.Data;
using DialogSystem.Data.SaveData;
using DialogSystem.Editor.GraphicsWindow;
using DialogSystem.Utilits;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;

namespace DialogSystem.Editor.Utilites
{
    using DataStructure;    
    using GraphicsElements;
    /// <summary>
    /// 可视化存储处理部分
    /// </summary>
    public static partial class DSIOUtility
    {
        public static Action<SerializedObject> DrawInspectorEvt;
        
        private static DialogGraphView graphView;
        //静态的存/取节点数据
        // private static Dictionary<string, NodeBase> nodeDataDic;
        private static Dictionary<string, SerializedNodeData_SO> nodeDataDic;
        private static Dictionary<string, DSNode> loadedNodes;
        private static Dictionary<string, DialogContext> loadedContext;
        private static Dictionary<string, List<string>> loadedConnections;
        
        private static string graphFileName;
        private static string treeDataFolderPath;

        private static List<DSNode> nodes;

        private static DSNode rootNode;
        
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="dsGraphView"></param>
        /// <param name="graphName"></param>
        public static void Initialize(DialogGraphView dsGraphView, string graphName)
        {
            graphView = dsGraphView;

            graphFileName = graphName;
            treeDataFolderPath = $"Assets/DialogueSystem/Dialogues/{graphName}";

            nodes = new List<DSNode>();


            loadedNodes ??= new Dictionary<string, DSNode>();
            nodeDataDic ??= new Dictionary<string, SerializedNodeData_SO>();
            loadedContext ??= new Dictionary<string, DialogContext>();
            loadedConnections??= new Dictionary<string, List<string>>();
        }



        #region 保存
        
        /// <summary>
        /// 存取：先保存到本地的树文件里面，然后保存到图标的数据里面
        /// </summary>
        public static void Save()
        {
            CreateDefaultFolders();
            
            GetElementsFromGraphView();
            

            
            DialogTreeData_SO localTreeData = CreateAsset<DialogTreeData_SO>(treeDataFolderPath, graphFileName);
            
            if (SaveToScriptableObject(localTreeData))
            {
                DSGraphSaveData_SO graphData = CreateAsset<DSGraphSaveData_SO>("Assets/Editor/DialogueSystem/Graphs", $"{graphFileName}Graph");
            
                graphData.Initialize(graphFileName);
                
                SaveToGraph(graphData);

                // SaveNodes(graphData, dialogueContainer);
                //
                SaveAsset(graphData);
                SaveAsset(localTreeData);
            }
            else
            {
                Debug.LogError("保存失败");
            }
        }
        
        private static void SaveToGraph(DSGraphSaveData_SO graphData)
        {
            foreach (var gNode in nodes)
            {
                List<string> connections = new List<string>();
                var venumrable = gNode.GetOutputConnection();
                var iterator = venumrable?.GetEnumerator();

                // iterator.MoveNext();
                while (iterator.MoveNext())
                {
                    if (iterator.Current == null)
                    {
                        Debug.LogError("iterator为空");
                        break;
                    }
                    var tnode = iterator.Current.input.node;
                    // iterator.Current.in
                    if (tnode is DSNode node)
                    {
                        connections.Add(node.ID);
                        Debug.LogWarning($"{node.ID}");
                    }
                    else
                    {
                        Debug.LogError("node不是DSNode类型的" + $"{tnode.name}");
                    }
                    //iterator.MoveNext();
                }

                NodeSaveData data = new NodeSaveData
                {
                    ID = gNode.ID,
                    Name = gNode.Name,
                    NodeType = gNode.NodeType,
                    ConnectionID = connections,
                    Position = gNode.GetPosition().position,
                    ContextData = loadedContext[gNode.ID],
                    isRootNode = gNode.IsRootNode,
                    TextHeader = nodeDataDic[gNode.ID].NodeData.m_textHeader,
                    EventIDs = (nodeDataDic[gNode.ID].NodeData is EventNode enode)?enode._triggerEvtID:null
                };
                graphData.Nodes.Add(data);
            }          
        }

        private static bool SaveToScriptableObject(DialogTreeData_SO localTreeData)
        {
            // localTreeData.m_Tree.m_CurNode.AddChild()
            if (rootNode.BuildTree())
            {
                SerializableNodeDictionary nodeDictionary = new SerializableNodeDictionary();
                SerializableStringListDictionary childtable = new SerializableStringListDictionary();
                if (nodeDataDic.Count <= 0)
                {
                    Debug.LogError("保存到ScriptableObject失败");
                    return false;
                }

                foreach (var keyvaluePair in nodeDataDic)
                {
                    nodeDictionary.Add(keyvaluePair.Key, keyvaluePair.Value.NodeData);
                }

                foreach (var keyvaluePair in loadedConnections)
                {
                    childtable.Add(keyvaluePair);
                }
                
                localTreeData.Initialize(graphFileName,
                    nodeDataDic[rootNode.ID].NodeData,
                    childtable,
                    nodeDictionary
                );
                return true;
            }
            return false;
        }
        #endregion

        #region 加载
        
        public static void Load()
        {
            loadedConnections = new Dictionary<string, List<string>>();
            DSGraphSaveData_SO graphData = LoadAsset<DSGraphSaveData_SO>("Assets/Editor/DialogueSystem/Graphs", graphFileName);

            if (graphData == null)
            {
                EditorUtility.DisplayDialog(
                    "找不到文件喵!",
                    "The file at the following path could not be found:\n\n" +
                    $"\"Assets/Editor/DialogueSystem/Graphs/{graphFileName}\".\n\n" +
                    "找不到文件捏.",
                    "好的喵!"
                );

                return;
            }

            DialogEditorWnd.UpdateFileName(graphData.FileName);

            LoadNodes(graphData.Nodes);
            LoadNodesConnections();
        }

        private static void LoadNodes(List<NodeSaveData> loadNodes)
        {
            foreach (NodeSaveData nodeData in loadNodes)
            {

                DSNode node = graphView.CreateNode(nodeData.Name, nodeData.NodeType, nodeData.Position, false);

                node.ID = nodeData.ID;
                node.Draw();
                
                // Debug.LogWarning(node.IsRootNode + node.Name);
                if (nodeData.isRootNode)
                {
                    Debug.LogWarning("是根节点");
                    node.InitRootNode();
                }

                graphView.AddElement(node);
                
                    NodeBase nodeBase;
                    if (nodeData.NodeType == DialogNodeType.Dialog)
                        nodeBase = new DialogNode(nodeData.TextHeader);
                    else
                    {
                        nodeBase = new EventNode(nodeData.EventIDs);
                        // Debug.Log("加载事件节点");
                    }

                    nodeBase.m_Context = nodeData.ContextData;

                    SerializedNodeData_SO nodeDataSo = ScriptableObject.CreateInstance<SerializedNodeData_SO>();
                    nodeDataSo.NodeData = nodeBase;

                nodeDataDic.Add(node.ID,nodeDataSo);
                loadedNodes.Add(node.ID, node);
                loadedContext.Add(node.ID,nodeData.ContextData);
                loadedConnections.Add(node.ID,nodeData.ConnectionID);
            }
        }

        private static void LoadNodesConnections()
        {
            foreach (KeyValuePair<string, DSNode> loadedNode in loadedNodes)
            {
                foreach (Port choicePort in loadedNode.Value.outputContainer.Children())
                {
                    // DSChoiceSaveData choiceData = (DSChoiceSaveData) choicePort.userData;
                    List<string> ids = loadedConnections[loadedNode.Value.ID];
                    
                    foreach (var id in ids)
                    {
                        if (string.IsNullOrEmpty(id))
                        {
                            continue;
                        }                        
                        
                        DSNode nextNode = loadedNodes[id];

                        Port nextNodeInputPort = (Port) nextNode.inputContainer.Children().First();

                        Edge edge = choicePort.ConnectTo(nextNodeInputPort);

                        graphView.AddElement(edge);

                        loadedNode.Value.RefreshPorts();
                    }
                }
            }
        }
        #endregion
        public static void DrawInspector(this DSNode node)
        {
            if (nodeDataDic == null)
            {
                // Debug.LogError("node字典为空");
                return;
            }
            Debug.Log(nodeDataDic[node.ID].NodeData.m_nodeType);
            SerializedObject sObj = new SerializedObject(nodeDataDic[node.ID]);
            DrawInspectorEvt?.Invoke(sObj);
        }

        public static bool TryGetNode(string key,out NodeBase result)
        {
            result = null;
            if (nodeDataDic.TryGetValue(key, out SerializedNodeData_SO data))
            {
                result = data.NodeData;
                return true;
            }

            return false;
        }


        #region 数据处理

        public static void AddNodeData(DSNode newNode, NodeBase newLocalNode)
        {
            nodeDataDic ??= new Dictionary<string, SerializedNodeData_SO>();
            loadedContext ??= new Dictionary<string, DialogContext>();
            loadedNodes ??= new Dictionary<string, DSNode>();

            SerializedNodeData_SO so = ScriptableObject.CreateInstance<SerializedNodeData_SO>();
            so.NodeData = newLocalNode;
            nodeDataDic.Add(newNode.ID,so);
            // Debug.Log(newNode.ID);
            loadedContext.Add(newNode.ID, newLocalNode.m_Context);
            loadedNodes.Add(newNode.ID, newNode);
        }

        public static void ClearData()
        {
            nodeDataDic = null;
            loadedNodes = null;
            loadedContext = null;
            nodes = null;
        }
        #endregion

        #region 数据结构管理

        public static void SetRootNode(this DSNode node)
        {
            rootNode = node;
        }
        #endregion
    }
}