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

        /// Finds the existing element and removes it from the state workspace,
        /// while also deleting the VisualElementState. Does nothing if the object
        /// does not exist
        public void RemoveState(VisualElement element) {
            if (!this.ElementExists(element)) {
                return;
            }
            this.elementStates.Remove(element);
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
                    VisualElementState pseudoState = new VisualElementState(curElementState);
                    pseudoState.SetStateScale(Vector3.zero);
                    pseudoState.BlendStates(curElementState, t);
                }
            }
            foreach(VisualElement element in prevState.elementStates.Keys) {
                if (!this.ElementExists(element)) {
                    // Disable
                    VisualElementState prevElementState = prevState.elementStates[element];
                    VisualElementState pseudoState = new VisualElementState(prevElementState);
                    pseudoState.SetStateScale(Vector3.zero);
                    prevElementState.BlendStates(pseudoState, t);
                }
            }
        }
    }
}
