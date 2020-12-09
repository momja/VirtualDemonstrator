using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualDemonstrator {
    public class VisualElement : MonoBehaviour {
        public List<VisualElementState> stateHistory;

        private void Start() {
            this.stateHistory = new List<VisualElementState>();
        }
    }
}