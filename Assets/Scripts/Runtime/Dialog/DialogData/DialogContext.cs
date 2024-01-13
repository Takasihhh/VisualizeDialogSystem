using System;
using System.Collections.Generic;
using UnityEngine;
namespace DialogSystem.Data
{
    using Utilits;
    
    [Serializable]
    public class DialogContext
    {
        [SerializeField] private List<ContextInfo> _contextInfos;

        public List<ContextInfo> m_contextInfos
        {
            get => _contextInfos;
            set => _contextInfos = value;
        }
    }

    [Serializable]
    public struct ContextInfo
    {
        public CharacterName name;
        public Sprite characterImg;
        public List<string> singleSentence;
    }
}