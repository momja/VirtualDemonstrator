using System;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualDemonstrator {
    [Serializable]
    public struct MenuOptionInfo {
        public string name;
        public Material overrideMaterial;
        public UnityEvent action;
    }
} 