using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogSystem.Data.SaveData
{
    using DataStructure;
    using Utilits;
    [Serializable]
    public class NodeSaveData
    {
        [field:SerializeField]public string ID { get; set; }
        [field:SerializeField]public string Name { get; set; }
        [field:SerializeField]public DialogNodeType NodeType { get; set; }
        [field: SerializeField] public List<string> ConnectionID { get; set; }
        [field:SerializeField] public Vector2 Position { get; set; }
        
        [field: SerializeField] public DialogContext ContextData { get; set; }
        [field:SerializeField] public List<string> EventIDs { get; set; }
        [field:SerializeField] public string TextHeader { get; set; }
        [field:SerializeField] public bool isRootNode { get; set; }
    }
}