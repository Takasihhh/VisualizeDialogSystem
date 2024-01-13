using System;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogSystem.Editor.GraphicsWindow
{
    using Editor.Utilites;
    using Utilits;

    public class DialogEditorWnd:EditorWindow
    {
        private DialogGraphView _graphView;
        private static TextField fileNameTextField;
        [MenuItem("DialogSystem/DialogEditor")]
        public static void OpenWnd()
        {
            var wnd = GetWindow<DialogEditorWnd>("对话编辑器");
            wnd.maxSize = new Vector2(1280, 720);
        }

        private void OnEnable()
        {
            DSIOUtility.DrawInspectorEvt+=UpdateInspector;
            DrawEditorWnd();
        }

        private void OnDisable()
        {
            DSIOUtility.DrawInspectorEvt-=UpdateInspector;
            DSIOUtility.ClearData();
        }

        #region 加载窗口

        private void UpdateInspector(SerializedObject serializedObject)
        {
            var container = rootVisualElement.Q<VisualElement>("ContextContainer");
            container.Clear();

            // var nodeProperty = serializedObject.GetIterator();
            // nodeProperty.Next(true);
            // // nodeProperty.NextVisible(false);
            // while (nodeProperty.NextVisible(false))
            // {
            //     //构造一个PropertyField用于显示
            //     PropertyField field = new PropertyField(nodeProperty);
            //     //与实际的节点数据绑定
            //     Debug.Log(nodeProperty.name);
            //     field.Bind(serializedObject);
            //     //加入到Inspector
            //     container.Add(field);
            // }
            var nodeProperty = serializedObject.FindProperty("<NodeData>k__BackingField");
            Debug.Log(nodeProperty.type);
            PropertyField field = new PropertyField(nodeProperty);
            field.name = "节点信息";
            field.Bind(serializedObject);
            container.Add(field);
            nodeProperty.Reset();
            nodeProperty.Next(true);
        }
        
        private void DrawInitWnd()
        {
            
        }
        
        private void DrawEditorWnd()
        {
            rootVisualElement.Clear();
            DrawMainContainer();
            AddGraphView();
            BindItems();
        }

        #endregion

        #region 加载窗口元素

        
        private void AddGraphView()
        {
            _graphView = new DialogGraphView(this);
            _graphView.style.minHeight = 1000;
            _graphView.style.minWidth = 500;
            _graphView.StretchToParentSize();
            rootVisualElement.Q<VisualElement>("RightPanel").Add(_graphView);
        }
        
        private void DrawMainContainer()
        {
            //加载主界面
            VisualTreeAsset editorViewTree = (VisualTreeAsset)EditorGUIUtility.Load("DialogWnd.uxml");
            TemplateContainer editorInstance = editorViewTree.CloneTree();
            editorInstance.StretchToParentSize();
            rootVisualElement.Add(editorInstance);
        }

        private void BindItems()
        {
            rootVisualElement.Q<Button>("MiniMapBtn").clicked += ToggleMiniMap;
            rootVisualElement.Q<Button>("SaveBtn").clicked += SaveFile;
            rootVisualElement.Q<Button>("LoadBtn").clicked += LoadFile;
            rootVisualElement.Q<Button>("ClearBtn").clicked += Clear;
            fileNameTextField = rootVisualElement.Q<TextField>("FileName");
           
            fileNameTextField.RegisterValueChangedCallback( callback =>
            {
                fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
            });
        }

        private void ToggleMiniMap()
        {
            _graphView.ToggleMiniMap();
        }
        
        #endregion


        #region 功能

        private void CreateNewFile()
        {
            
        }

        private void SaveFile()
        {
            if (string.IsNullOrEmpty(fileNameTextField.value))
            {
                EditorUtility.DisplayDialog("Invalid file name.", "Please ensure the file name you've typed in is valid.", "Roger!");

                return;
            }

            DSIOUtility.Initialize(_graphView, fileNameTextField.value);
            DSIOUtility.Save();
        }

        private void LoadFile()
        {
            string filePath = EditorUtility.OpenFilePanel("Dialogue Graphs", "Assets/Editor/DialogueSystem/Graphs", "asset");

            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            Clear();
            DSIOUtility.Initialize(_graphView, Path.GetFileNameWithoutExtension(filePath));
            DSIOUtility.Load();
        }

        private void Clear()
        {
            _graphView.ClearGraph();
            var container = rootVisualElement.Q<VisualElement>("ContextContainer");
            container.Clear();
            DSIOUtility.ClearData();
        }
        public static void UpdateFileName(string fileName)
        {
            fileNameTextField.value = fileName;
        }

        #endregion
    }
}