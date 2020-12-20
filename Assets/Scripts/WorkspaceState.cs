using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VirtualDemonstrator {
    public class WorkspaceState {
        public WorkspaceState() {
            this.elementStates = new Dictionary<VisualElement, VisualElementState>();
        }

        public WorkspaceState(WorkspaceState prevState) {
            this.elementStates = new Dictionary<VisualElement, VisualElementState>();
            foreach(VisualElement element in prevState.elementStates.Keys) {
                VisualElementState elementState = prevState.elementStates[element];
                VisualElementState newElementState = new VisualElementState(elementState);
                this.elementStates.Add(element, newElementState);
            }
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
                elementStates.Add(element, newState);
            }
            else {
                elementStates[element] = newState;
            }

            Debug.Log("Element count: " + elementStates.Count);
        }

        public Dictionary<VisualElement, VisualElementState> elementStates;

        public void updateAllStates(WorkspaceState prevState, float t) {
            // t is used as the interpolation parameter.
            foreach(VisualElement element in this.elementStates.Keys) {
                VisualElementState curElementState = elementStates[element];
                if (prevState.ElementExists(element)) {
                    // Interpolate between states
                    VisualElementState prevElementState = prevState.elementStates[element];
                    prevElementState.BlendStates(curElementState, t);
                }
                else {
                    // Pop in from zero scale

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
