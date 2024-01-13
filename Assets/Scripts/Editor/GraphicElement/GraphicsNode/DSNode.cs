using System.Collections;
using System.Collections.Generic;
using DialogSystem.Editor.Utilites;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogSystem.Editor.GraphicsElements
{
    using Editor.GraphicsWindow;
    using Utilits;
    public class DSNode:Node
    {
        public string ID { get; set; }
        public string Name => dialogueName;
        public bool IsRootNode { get; set; }
        protected GraphView graphView;
        protected string dialogueName;
        protected Color defaultBackgroundColor;
        protected Port inputPort;
        public DialogNodeType NodeType { get; set; }
        public virtual void Initialize(string nodeName,DialogGraphView dsGraphView,Vector2 position)
        {
            ID = System.Guid.NewGuid().ToString();
            dialogueName = nodeName;
            name = "节点";
            SetPosition(new Rect(position, Vector2.zero));

            graphView = dsGraphView;
            defaultBackgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);

            mainContainer.AddToClassList("ds-node__main-container");
            extensionContainer.AddToClassList("ds-node__extension-container");
        }

        public virtual void Draw()
        {

            /* INPUT CONTAINER */
            inputContainer.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            inputPort = this.CreatePort("输入端口", Orientation.Horizontal, Direction.Input, Port.Capacity.Single);
            
            inputContainer.Add(inputPort);


            // /* EXTENSION CONTAINER */
            //
            // VisualElement customDataContainer = new VisualElement();
            //
            // customDataContainer.AddToClassList("ds-node__custom-data-container");
            //
            // Foldout textFoldout = DSElementUtility.CreateFoldout("Dialogue Text");
            //
            // TextField textTextField = DSElementUtility.CreateTextArea(Text, null, callback => Text = callback.newValue);
            //
            // textTextField.AddClasses(
            //     "ds-node__text-field",
            //     "ds-node__quote-text-field"
            // );
            //
            // textFoldout.Add(textTextField);
            //
            // customDataContainer.Add(textFoldout);
            //
            // extensionContainer.Add(customDataContainer);
        }
        
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Disconnect Input Ports", actionEvent => DisconnectInputPorts());
            evt.menu.AppendAction("Disconnect Output Ports", actionEvent => DisconnectOutputPorts());
            base.BuildContextualMenu(evt);
        }
        public void DisconnectAllPorts()
        {
            DisconnectInputPorts();
            DisconnectOutputPorts();
        }

        private void DisconnectInputPorts()
        {
            DisconnectPorts(inputContainer);
        }

        private void DisconnectOutputPorts()
        {
            DisconnectPorts(outputContainer);
        }
        
        
        private void DisconnectPorts(VisualElement container)
        {
            foreach (Port port in container.Children())
            {
                if (!port.connected)
                {
                    continue;
                }

                graphView.DeleteElements(port.connections);
            }
        }

        public override void OnSelected()
        {
            base.OnSelected();
            this.DrawInspector();
        }
    
        
        public void InitRootNode()
        {
            IsRootNode = true;
            this.capabilities &= ~Capabilities.Deletable;
            this.SetRootNode();
            inputPort.SetEnabled(false);
        }

        public virtual IEnumerable<Edge> GetOutputConnection()
        {
            Debug.Log("父节点查询");
            return null;
        }
        
        public string m_DialogueName
        {
            get => dialogueName;
            set => dialogueName = value;
        }
    }
}
