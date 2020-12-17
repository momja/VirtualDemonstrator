using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualDemonstrator {
    public class VisualElement : MonoBehaviour {
        public VisualElement(GameObject elementObject) {
            this.elementObject_ = elementObject;
            this.stateHistory = new List<VisualElementState>();

            // Add the initial state.
            AddState();
        }


        // This function adds a new state to the list based on the element's current state.
        public void AddState(int index = -1)
        {
            // Create the new state.
            VisualElementState state = new VisualElementState(elementObject_);

            // Either append or insert the new state.
            if (index < 0 || index >= stateHistory.Count)
            {
                stateHistory.Add(state);
            }
            else
            {
                stateHistory.Insert(index, state);
            }
        }


        // This function removes a state specified by the given index.
        public void RemoveState(int index)
        {
            // Check error bounds before removal.
            if (index < 0 || index >= stateHistory.Count)
            {
                return;
            }
            else
            {
                stateHistory.RemoveAt(index);
            }
        }

        // This function returns the Element's GameObject.
        public GameObject GetElementObject() { return elementObject_; }

        // This is the GameObject representing the Visual Element.
        private GameObject elementObject_;

        // This list holds all the states that the element has gone through.
        private List<VisualElementState> stateHistory;
    }
}