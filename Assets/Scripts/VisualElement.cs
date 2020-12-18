using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualDemonstrator {
    public class VisualElement : MonoBehaviour {
        private void Start()
        {
            // this.stateHistory = new List<VisualElementState>();
            // Add the initial state.
            // AddState();
            this.workspace = GameObject.Find("Workspace").GetComponent<Workspace>();
        } 
        
        public void UpdateState()
        {
            VisualElementState state = this.workspace.GetCurrentState().elementStates[this];
            state.SetStatePosition(this.gameObject.transform.position);
            state.SetStateScale(this.gameObject.transform.localScale);
            state.SetStateRotation(this.gameObject.transform.rotation);
        }


        // This function returns the Element's GameObject.
        public GameObject GetElementObject() { return this.gameObject; }

        // This is the GameObject representing the Visual Element.
        // private GameObject elementObject_;

        // This list holds all the states that the element has gone through.

        public Workspace workspace;
    }
}