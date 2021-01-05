using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualDemonstrator {
    public class VisualElement : MonoBehaviour {

        public Color hoverColor = new Color(0.902f,0.945f,0.0706f);
        public Color selectColor = new Color(0.109f,0.925f,0.270f);
        public UnityEvent OnHover = new UnityEvent();
        public UnityEvent OnHoverExit = new UnityEvent();
        public UnityEvent OnSelect = new UnityEvent();
        public UnityEvent OnSelectExit = new UnityEvent();
        protected Outline outline;
        protected bool selected = false;

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

        public virtual void HoverEntered() {
            OnHover.Invoke();
            outline.enabled = true;
            if (!selected) {
                outline.OutlineColor = hoverColor;
            }
        }

        public virtual void HoverExited() {
            OnHoverExit.Invoke();
            if (!selected) {
                outline.enabled = false;
            }
        }

        public virtual void SelectEntered() {
            OnSelect.Invoke();
            outline.enabled = true;
            outline.OutlineColor = selectColor;
            selected = true;
        }

        public virtual void SelectExited() {
            OnSelectExit.Invoke();
            outline.enabled = false;
            selected = false;
        }

        /// Compute the GameObjects bounds
        public virtual Bounds GetBounds() {
            return GetComponent<Renderer>().bounds;
        }


        // This function returns the Element's GameObject.
        public GameObject GetElementObject() { return this.gameObject; }

        // This is the GameObject representing the Visual Element.
        // private GameObject elementObject_;

        // This list holds all the states that the element has gone through.

        public Workspace workspace;
    }
}