using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        private WorkspaceState _curState;

        // Start is called before the first frame update
        private void Start()
        {
            this.stateHistory_ = new List<WorkspaceState>();
            // this.visualElements_ = new List<VisualElement>();
            this.visualElements_ = new List<GameObject>();
            this.timeline.workspace = this;
        }

        // Update is called once per frame
        private void Update()
        {

        }

        // This function
        public void InsertNewState(int index = -1)
        {
            // Create the new state based on the current workspace conditions.
            WorkspaceState state = new WorkspaceState(visualElements_);

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
            int timeIndex = this.timeline.getTimeIndex(time);
            return this.stateHistory_[timeIndex];
        }

        public void updateCurrentState(int frame) {
            this._curState = this.stateHistory_[frame];
            // set the transforms
            this._curState.updateAllStates(1);
        }

        public void toggleMode(InteractionModes mode) {
            // Handle change in mode
            print("toggling");
        }
    }
}
