using UnityEngine;

namespace DialogSystem.Data
{
    [CreateAssetMenu(fileName ="New Context",menuName = "Dialog/Context")]
    public class DialogContext_SO:ScriptableObject
    {
        [field: SerializeField] public DialogContext ContextData { get; set; }
    }
}