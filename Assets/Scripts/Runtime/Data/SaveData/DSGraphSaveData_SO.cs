using System.Collections.Generic;
using DialogSystem.DataStructure;
using UnityEngine;

namespace DialogSystem.Data.SaveData
{
    public class DSGraphSaveData_SO:ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
 
        [field: SerializeField] public List<NodeSaveData> Nodes { get; set; }
        // [field: SerializeField] public Dictionary<string, NodeBase> NodeDic;
        public void Initialize(string fileName)
        {
            FileName = fileName;

            Nodes = new List<NodeSaveData>();
        }
    }
}