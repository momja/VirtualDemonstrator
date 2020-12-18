using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VirtualDemonstrator {
    public class Timeline : MonoBehaviour {
        
        // public toggleModeButton toggleButton;
        public Workspace workspace;
        public float curTime;
        public int stateCount;
        public TimelineSlider slider;

        private void Start() {
            this.curTime = 0;
            this.stateCount = 0;
            this.slider.timelineDelegate = this;
            this.slider.slider.maxValue = this.stateCount;
        }

        private void Update() {
            
        }

        public void pointerUp() {
            this.workspace.OnTimelineChanged((int)slider.slider.value);
        }

        public void toggleMode(InteractionModes mode) {
            this.workspace.toggleMode(mode);
        }

        public void addStateToWorkspace() {
            print("Adding New State");
            int t = (int)slider.slider.value;
            workspace.InsertNewState(t);
        }

        public void setStateCount(int count) {
            this.stateCount = count;
            this.slider.slider.maxValue = this.stateCount;
        }
    }
}