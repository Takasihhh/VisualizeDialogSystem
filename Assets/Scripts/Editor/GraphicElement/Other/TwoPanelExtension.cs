using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogSystem.Editor.GraphicsElements
{
    public class TwoPanelExtensionHorizontal : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<TwoPanelExtensionHorizontal, UxmlTraits>{ }

        public VisualElement leftPanel,rightPanel;
        public TwoPanelExtensionHorizontal()
        {
            style.minHeight = 1000;
            leftPanel = new VisualElement();
            leftPanel.style.minWidth = 200;
            leftPanel.name = "LeftPanel";
            Label inspector = new Label("Inspector");
            inspector.style.color = new StyleColor(Color.black);
            inspector.style.backgroundColor = new StyleColor(Color.white);
            inspector.style.paddingLeft = 65;
            VisualElement contextContainer = new VisualElement();
            contextContainer.name = "ContextContainer";
            contextContainer.style.minHeight = 1000;
            leftPanel.Add(inspector);
            leftPanel.Add(contextContainer);
            rightPanel = new VisualElement();
            rightPanel.name = "RightPanel";
            rightPanel.style.minWidth = 200;
            this.Add(leftPanel);
            this.Add(rightPanel);
            this.style.minWidth = 2000;
            this.style.minHeight = 1000;
            this.orientation = TwoPaneSplitViewOrientation.Horizontal;
        }
    }
    
    public class TwoPanelExtensionVerticle : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<TwoPanelExtensionVerticle, UxmlTraits>{ }

        public VisualElement topPanel,buttomPanel;
        public TwoPanelExtensionVerticle()
        {
            topPanel = new VisualElement();
            topPanel.style.minHeight = 200;
            buttomPanel = new VisualElement();
            buttomPanel.style.minHeight = 400;
            this.Add(topPanel);
            this.Add(buttomPanel);
            
            this.orientation = TwoPaneSplitViewOrientation.Horizontal;
        }
    }
}