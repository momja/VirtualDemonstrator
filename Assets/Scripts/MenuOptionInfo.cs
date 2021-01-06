using System;
using UnityEngine.Events;

namespace VirtualDemonstrator {
    [Serializable]
    public struct MenuOptionInfo {
        public string name;
        public UnityEvent action;
    }
} 