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
            this.stateCount = 1;
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

        public void removeStateFromWorkspace() {
            if (stateCount == 1) {
                return;
            }
            print("Removing Current State");
            int t = (int)slider.slider.value;
            workspace.DeleteState(t);
            if (t > 0) {
                SetSlider(t-1);
            }
            this.setStateCount(stateCount-1);
        }

        public void setStateCount(int count) {
            this.stateCount = count;
            this.slider.slider.maxValue = this.stateCount - 1;
        }

        public void SetSlider(int value) {
            this.slider.slider.SetValueWithoutNotify(value);
        }
    }
}