using System.Collections.Generic;
using DialogSystem.Editor.GraphicsWindow;
using DialogSystem.Editor.Utilites;
using DialogSystem.Utilits;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogSystem.Editor.GraphicsElements
{
    public class DSEventNode:DSNode
    {
        private Port outputPort;
        public override void Initialize(string nodeName, DialogGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);
            dialogueName = "事件节点";
            NodeType = DialogNodeType.Event;
            defaultBackgroundColor = new Color(100f / 255f, 29f / 255f, 50 / 255f);
            mainContainer.style.backgroundColor = defaultBackgroundColor;
        }

        public override void Draw()
        {
            base.Draw();
            Label title = new Label(dialogueName);
            
            titleContainer.Insert(0,title);
            
            /*OUTPUT CONTAINER*/
            outputPort = this.CreatePort("输出端口", Orientation.Horizontal, Direction.Output, Port.Capacity.Single);
            // outputPort.connections.GetEnumerator()
            outputContainer.Add(outputPort);

        }

        public override IEnumerable<Edge> GetOutputConnection()
        {
            Debug.Log("查询接口");
            return outputPort.connections;
        }
    }
}