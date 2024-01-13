using DialogSystem.DataStructure;
using UnityEngine;

namespace DialogSystem.Data
{
    public class SerializedNodeData_SO:ScriptableObject
    {
        [field: SerializeField] public NodeBase NodeData { get; set; }
    }
}