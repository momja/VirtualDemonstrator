using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VirtualDemonstrator {
    public class WorkspaceState {
        public WorkspaceState() {
            this.visualElements_ = new List<VisualElement>();
            this.elementStates = new Dictionary<VisualElement, VisualElementState>();
        }

        public WorkspaceState(WorkspaceState prevState) {
            // foreach(ele)
        }

        // This function checks whether a given element exists in the workspace state based on its object.
        // If so, its index is returned.
        public bool ElementExists(VisualElement element)
        {
            return this.elementStates.ContainsKey(element);
        }

        // This functions updates/adds new element as a state in the workspace.
        public void AddState(VisualElement element)
        {
            VisualElementState newState = new VisualElementState(element);
            if (!this.ElementExists(element)) {
                visualElements_.Add(element);
                elementStates.Add(element, newState);
            }
            else {
                elementStates[element] = newState;
            }
        }

        private List<VisualElement> visualElements_;

        public Dictionary<VisualElement, VisualElementState> elementStates;

        public void updateAllStates(WorkspaceState prevState, float t) {
            // t is used as the interpolation parameter.
            foreach(VisualElement element in this.elementStates.Keys) {
                if (prevState.ElementExists(element)) {
                    // Interpolate between states
                }
                else {
                    // Enable
                }
            }
            foreach(VisualElement element in prevState.elementStates.Keys) {
                if (!this.ElementExists(element)) {
                    // Disable
                }
            }
        }
    }
}
