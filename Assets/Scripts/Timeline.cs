using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VirtualDemonstrator {
    public class Timeline : MonoBehaviour {
        
        // public toggleModeButton toggleButton;
        public Workspace workspace;
        public const float maxTime = 1;
        public float curTime;
        public int stateCount;
        public TimelineSlider slider;

        private void Start() {
            this.curTime = 0;
            this.stateCount = 0;
            this.slider.timelineDelegate = this;
        }

        private void Update() {
            
        }

        public void pointerUp() {
            this.workspace.OnTimelineChanged(slider.GetComponent<Slider>().value);
        }

        public int getTimeIndex(float time) {
            return Mathf.RoundToInt(time * this.stateCount);
        }

        public void toggleMode(InteractionModes mode) {
            this.workspace.toggleMode(mode);
        }
    }
}