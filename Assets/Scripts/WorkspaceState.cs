using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VirtualDemonstrator {
    public class WorkspaceState {
        public WorkspaceState(List<GameObject> elementObjects)
        {
            // Create VisualElements that correspond to each GameObject.
            visualElements_ = new List<VisualElement>();
            for (int i = 0; i < elementObjects.Count; i++)
            {
                visualElements_.Add(new VisualElement(elementObjects[i]));
            }
        }

        // This function checks whether a given element exists in the workspace state based on its object.
        // If so, its index is returned.
        public int ElementExists(GameObject elementObject)
        {
            for (int i = 0; i < visualElements_.Count; i++)
            {
                if (visualElements_[i].GetElementObject() == elementObject)
                {
                    return i;
                }
            }

            return -1;
        }

        // This functions updates/adds new elements as a state in the workspace.
        public void AddState(List<GameObject> elementObjects)
        {
            // Either append a new state to each element or create new elements.
            for (int i = 0; i < elementObjects.Count; i++)
            {
                int index = ElementExists(elementObjects[i]);
                if (index != -1)
                {
                    visualElements_[index].AddState();
                }
                else
                {
                    visualElements_.Add(new VisualElement(elementObjects[i]));
                }
            }
        }

        private List<VisualElement> visualElements_;

        public List<VisualElementState> elementStates;

        public WorkspaceState() {
            this.elementStates = new List<VisualElementState>();
        }

        public void updateAllStates(float t) {
            // t is used as the interpolation parameter.
            foreach(VisualElementState vizState in this.elementStates) {
                // set new transform for visual element
                // vizState.setVizElementToState(t);
            }
        }
    }
}
