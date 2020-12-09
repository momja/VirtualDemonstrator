using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualDemonstrator {
    public class Timeline : MonoBehaviour {
        
        public const float maxTime = 1;
        public float curTime;
        public int stateCount;

        private void Start() {
            this.curTime = 0;
            this.stateCount = 0;
        }

        private void Update() {
            
        }

        public int getTimeIndex(float time) {
            return Mathf.RoundToInt(time * this.stateCount);
        }
    }
}