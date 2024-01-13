using System.Collections.Generic;
using DialogSystem.Editor.GraphicsWindow;
using DialogSystem.Editor.Utilites;
using DialogSystem.Utilits;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogSystem.Editor.GraphicsElements
{
    public class DSDialogNode:DSNode
    {
        private Port outputPort;

        public override void Initialize(string nodeName, DialogGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);
            NodeType = DialogNodeType.Dialog;
            mainContainer.style.backgroundColor = defaultBackgroundColor;
        }

        public override void Draw()
        {
            base.Draw();
            /* TITLE CONTAINER */

            TextField dialogueNameTextField = DSElementUtility.CreateTextField(m_DialogueName, null, callback =>
            {
                TextField target = (TextField) callback.target;

                target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
                
                m_DialogueName = target.value;
            });

            dialogueNameTextField.AddClasses(
                "ds-node__text-field",
                "ds-node__text-field__hidden",
                "ds-node__filename-text-field"
            );

            titleContainer.Insert(0, dialogueNameTextField);
            
            /*OUTPUT CONTAINER*/
            outputPort = this.CreatePort("输出端口", Orientation.Horizontal, Direction.Output, Port.Capacity.Multi);

            outputContainer.Add(outputPort);
        }
        
        public override IEnumerable<Edge> GetOutputConnection()
        {
            Debug.Log("查询接口");
            return outputPort.connections;
        }
    }
}