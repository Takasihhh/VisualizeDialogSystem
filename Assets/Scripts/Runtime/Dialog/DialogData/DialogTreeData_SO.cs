using System.Collections.Generic;
using UnityEngine;

namespace DialogSystem.Data
{
    using DataStructure;
    [CreateAssetMenu(fileName ="New DialogTree",menuName = "Dialog/TreeData")]
    public class DialogTreeData_SO:ScriptableObject
    {
        [field: SerializeField] private string FileName { get; set; }
        [SerializeReference] private DialogTree _tree;
            

        
        public void Initialize(string fileName,NodeBase rootNode, SerializableStringListDictionary table, SerializableNodeDictionary nodeDic)
        {
            FileName = fileName;
            _tree = new DialogTree(rootNode,table,nodeDic);
        }
        
        

        public DialogTree m_Tree
        {
            get
            {
                if (_tree == null)
                {
                    Debug.LogError("树为空");
                    return null;
                }

                return _tree;
            }
            set => _tree = value;
        }
    }
}