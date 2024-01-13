using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogSystem.Editor.GraphicsWindow
{
    using Utilits;
    using Utilites;
    using GraphicsElements;
    public class DialogGraphView : GraphView
    {
        public DialogEditorWnd editorWindow;
        private MiniMap miniMap;
        public DialogGraphView(DialogEditorWnd dsEditorWindow)
        {
            editorWindow = dsEditorWindow;
            
            AddManipulators();
            AddGridBackground();
            // AddSearchWindow();
            AddMiniMap();

            OnElementsDeleted();
            OnGraphViewChanged();

            AddStyles();
            AddMiniMapStyles();
        }

        #region 初始化
        
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort == port)
                {
                    return;
                }

                if (startPort.node == port.node)
                {
                    return;
                }

                if (startPort.direction == port.direction)
                {
                    return;
                }

                if (port.parent.name == "rootNode")
                {
                    return;
                }

                compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        #endregion

        #region 绘制
        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();

            gridBackground.StretchToParentSize();

            Insert(0, gridBackground);
        }
        private void AddStyles()
        {
            this.AddStyleSheets(
                "DialogueSystem/DSGraphViewStyles.uss",
                "DialogueSystem/DSNodeStyles.uss"
            );
        }

        private void AddMiniMapStyles()
        {
            StyleColor backgroundColor = new StyleColor(new Color32(29, 29, 30, 255));
            StyleColor borderColor = new StyleColor(new Color32(51, 51, 51, 255));

            miniMap.style.backgroundColor = backgroundColor;
            miniMap.style.borderTopColor = borderColor;
            miniMap.style.borderRightColor = borderColor;
            miniMap.style.borderBottomColor = borderColor;
            miniMap.style.borderLeftColor = borderColor;
        }
        private void AddMiniMap()
        {
            miniMap = new MiniMap()
            {
                anchored = true
            };

            miniMap.SetPosition(new Rect(0, 0, 200, 180));

            Add(miniMap);

            miniMap.visible = false;
        }

        #endregion
        
        #region 添加组件

        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(CreateNodeContextualMenu("Add Node (Dialog Node)", DialogNodeType.Dialog));
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Event Node)", DialogNodeType.Event));
        }

        private IManipulator CreateNodeContextualMenu(string actionTitle, DialogNodeType dialogueType)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle,
                    actionEvent =>
                    {
                        DSNode newNode;
                        if(graphElements.Any())
                        {newNode = CreateNode("对话节点", dialogueType,
                            GetLocalMousePosition(actionEvent.eventInfo.localMousePosition));
                        }
                        else
                        {
                         newNode = CreateNode("根节点", DialogNodeType.Dialog,
                             GetLocalMousePosition(actionEvent.eventInfo.localMousePosition));
                         newNode.InitRootNode();
                         newNode.inputContainer.name = "rootNode";
                        }
                        newNode.RegisterNode();
                        AddElement(newNode);
                    })
            );

            return contextualMenuManipulator;
        }

        public DSNode CreateNode(string nodeName, DialogNodeType dialogueType, Vector2 position, bool shouldDraw = true)
        {
            Type nodeType = Type.GetType($"DialogSystem.Editor.GraphicsElements.DS{dialogueType}Node");

            DSNode node = (DSNode) Activator.CreateInstance(nodeType);

            node.Initialize(nodeName, this, position);

            if (shouldDraw)
            {
                node.Draw();
            }
            
            return node;
        }
        
        private void OnElementsDeleted()
        {
            deleteSelection = (operationName, askUser) =>
            {
                Type edgeType = typeof(Edge);

                List<DSNode> nodesToDelete = new List<DSNode>();
                List<Edge> edgesToDelete = new List<Edge>();

                foreach (GraphElement selectedElement in selection)
                {
                    if (selectedElement is DSNode node)
                    {
                        nodesToDelete.Add(node);

                        continue;
                    }

                    if (selectedElement.GetType() == edgeType)
                    {
                        Edge edge = (Edge) selectedElement;

                        edgesToDelete.Add(edge);

                        continue;
                    }
                }

                DeleteElements(edgesToDelete);

                foreach (DSNode nodeToDelete in nodesToDelete)
                {
                    nodeToDelete.DisconnectAllPorts();
                    RemoveElement(nodeToDelete);
                }
            };
        }
        
        private void OnGraphViewChanged()
        {
            //TODO：图标变化时更新树
            graphViewChanged = (changes) =>
            {
                if (changes.edgesToCreate != null)
                {
                    foreach (Edge edge in changes.edgesToCreate)
                    {
                        DSNode nextNode = (DSNode) edge.input.node;
                    }
                }

                if (changes.elementsToRemove != null)
                {
                    Type edgeType = typeof(Edge);

                    foreach (GraphElement element in changes.elementsToRemove)
                    {
                        if (element.GetType() != edgeType)
                        {
                            continue;
                        }

                        Edge edge = (Edge) element;
                    }
                }

                return changes;
            };
        }

        public void ClearGraph()
        {
            graphElements.ForEach(graphElement => RemoveElement(graphElement));
        }

        #endregion
        
        #region 其他功能
        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
        {
            Vector2 worldMousePosition = mousePosition;

            if (isSearchWindow)
            {
                worldMousePosition = editorWindow.rootVisualElement.ChangeCoordinatesTo(editorWindow.rootVisualElement.parent, mousePosition - editorWindow.position.position);
            }

            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);

            return localMousePosition;
        }
        
        public void ToggleMiniMap()
        {
            // Debug.Log("打卡缩略图");
            miniMap.visible = !miniMap.visible;
        }


        
        #endregion
    }
}