using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualDemonstrator {
    public class VisualElement : MonoBehaviour {
        private Color hoverColor = new Color(230f/255f,241f/255f,18f/255f);
        private Color selectColor = new Color(28f/255f,236f/255f,69f/255f);

        private Outline outline;
        private bool selected = false;

        private void Start()
        {
            // this.stateHistory = new List<VisualElementState>();
            // Add the initial state.
            // AddState();
            this.workspace = GameObject.Find("Workspace").GetComponent<Workspace>();
            this.outline = gameObject.GetComponent<Outline>();
        } 
        
        public void UpdateState()
        {
            VisualElementState state = this.workspace.GetCurrentState().elementStates[this];
            state.SetStatePosition(this.gameObject.transform.position);
            state.SetStateScale(this.gameObject.transform.localScale);
            state.SetStateRotation(this.gameObject.transform.rotation);
        }

        public void onHoverEntered() {
            outline.enabled = true;
            if (!selected) {
                outline.OutlineColor = hoverColor;
            }
        }

        public void onHoverExited() {
            if (!selected) {
                outline.enabled = false;
            }
        }

        public void onSelectEntered() {
            outline.enabled = true;
            outline.OutlineColor = selectColor;
            selected = true;
        }

        public void onSelectExited() {
            outline.enabled = false;
            selected = false;
        }


        // This function returns the Element's GameObject.
        public GameObject GetElementObject() { return this.gameObject; }

        // This is the GameObject representing the Visual Element.
        // private GameObject elementObject_;

        // This list holds all the states that the element has gone through.

        public Workspace workspace;
    }
}