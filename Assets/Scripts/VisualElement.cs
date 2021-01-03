using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualDemonstrator {
    public class VisualElement : MonoBehaviour {

        public UnityEvent OnHover = new UnityEvent();
        public UnityEvent OnHoverExit = new UnityEvent();
        public UnityEvent OnSelect = new UnityEvent();
        public UnityEvent OnSelectExit = new UnityEvent();
        private Color hoverColor = new Color(230f/255f,241f/255f,18f/255f);
        private Color selectColor = new Color(28f/255f,236f/255f,69f/255f);

        private Outline outline;
        private bool selected = false;
        private bool firstTransform = false;

        protected virtual void Start()
        {
            this.workspace = Workspace.Instance;
            this.outline = gameObject.GetComponent<Outline>();
        } 
        
        public void UpdateState()
        {
            VisualElementState state = this.workspace.GetCurrentState().elementStates[this];
            state.SetStatePosition(this.gameObject.transform.position);
            state.SetStateScale(this.gameObject.transform.localScale);
            state.SetStateRotation(this.gameObject.transform.rotation);
            state.SetStateMaterial(this.gameObject.GetComponent<Renderer>().material);
        }

        public void HoverEntered() {
            OnHover.Invoke();
            outline.enabled = true;
            if (!selected) {
                outline.OutlineColor = hoverColor;
            }
        }

        public void HoverExited() {
            OnHoverExit.Invoke();
            if (!selected) {
                outline.enabled = false;
            }
        }

        public void SelectEntered() {
            OnSelect.Invoke();
            outline.enabled = true;
            outline.OutlineColor = selectColor;
            selected = true;
        }

        public void SelectExited() {
            OnSelectExit.Invoke();
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