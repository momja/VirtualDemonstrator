using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace VirtualDemonstrator
{

    public enum InteractionModes {
        Create,
        Present
    }
    /// <summary>
    /// Main Driver class for the virtual demonstrator.
    /// Choreographs interactions between various components.
    /// </summary>
    public class Workspace : MonoBehaviour
    {
        public MenuPanel menuPanel;
        public Timeline timeline;
        private List<WorkspaceState> stateHistory_;
        private List<GameObject> visualElements_;
        private float prevSliderValue;

        private WorkspaceState _prevState;
        private WorkspaceState _curState;
        private bool isLerping = false;

        // Start is called before the first frame update
        private void Start()
        {
            this.stateHistory_ = new List<WorkspaceState>();
            // this.visualElements_ = new List<VisualElement>();
            this.visualElements_ = new List<GameObject>();
            this.timeline.workspace = this;
            // this.timelineSlider = this.timeline.GetComponentInChildren<Slider>();
            // this.timelineSlider.OnPointerUp.AddListener(delegate { OnSliderChanged(); });
        }

        // Update is called once per frame
        private void Update()
        {
        
        }

        public void OnTimelineChanged(float t)
        {
            if (!isLerping) {
                Debug.Log(t);
                this._prevState = this._curState;
                _curState = GetStateAtTime(t);
                if (this._curState != null) {
                    this.updateCurrentState();
                }
            }
        }

        // This function
        public void InsertNewState(int index = -1)
        {
            // Create the new state based on the current workspace conditions.
            WorkspaceState state = new WorkspaceState();

            if (index < 0)
            {
                // Set to end
                this.stateHistory_.Add(state);
            }
            else
            {
                this.stateHistory_.Insert(index, state);
            }
            this.timeline.stateCount += 1;
        }

        public bool DeleteState(WorkspaceState state)
        {
            bool result = this.stateHistory_.Remove(state);
            if (result)
            {
                this.timeline.stateCount -= 1;
            }
            return result;
        }

        public bool DeleteState(int index)
        {
            if (index < 0 || index >= this.stateHistory_.Count)
            {
                return false;
            }
            this.stateHistory_.RemoveAt(index);
            return true;
        }

        public WorkspaceState GetStateAtTime(float time)
        {
            if (this.stateHistory_.Count == 0) {
                return null;
            }
            int stateIndex = Mathf.RoundToInt(time * this.stateHistory_.Count);
            return this.stateHistory_[stateIndex];
        }

        public void updateCurrentState() {
            // set the transforms
            this._curState.updateAllStates(this._prevState, 1);
        }

        public void toggleMode(InteractionModes mode) {
            // Handle change in mode
            print("toggling");
        }
    }
}
