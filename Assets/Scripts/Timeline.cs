using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualDemonstrator {
    public class Timeline : MonoBehaviour {
        
        // public toggleModeButton toggleButton;
        public Workspace workspace;
        public const float maxTime = 1;
        public float curTime;
        public int stateCount;

        private void Start() {
            this.curTime = 0;
            this.stateCount = 0;
            // this.toggleButton.delegator = this;
        }

        private void Update() {
            
        }

        public int getTimeIndex(float time) {
            return Mathf.RoundToInt(time * this.stateCount);
        }

        public void toggleMode(InteractionModes mode) {
            this.workspace.toggleMode(mode);
        }
    }
}