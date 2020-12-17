using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VirtualDemonstrator {
    public class VisualElementState {
        // A constructor that takes in a game object and saves its current material and transform.
        public VisualElementState(VisualElement element)
        {
            this.element_ = element;
            stateMaterial_ = element.gameObject.GetComponent<Renderer>().material;
            stateTransform_ = element.gameObject.transform;
        }

        // This function assigns a blend of this state with another.
        public void BlendStates(VisualElementState secondState, float t)
        {
            // A t-value of 0 will assign the state stored in the caller.
            // A t-value of 1 will assign the state passed into the function.
            // A t-value between 0 and 1 will assign a blend of the two states.
            if (t >= 0 && t <= 1) {
                // Interpolate the transform values and assign them.
                Transform secondTransform = secondState.GetStateTransform();
                Vector3 lerpedPosition = Vector3.Lerp(this.stateTransform_.localPosition, secondTransform.localPosition, t);
                Vector3 lerpedScale = Vector3.Lerp(this.stateTransform_.localScale, secondTransform.localScale, t);
                Quaternion lerpedRotation = Quaternion.Slerp(this.stateTransform_.localRotation, secondTransform.localRotation, t);
                element_.gameObject.transform.localPosition = lerpedPosition;
                element_.gameObject.transform.localScale = lerpedScale;
                element_.gameObject.transform.localRotation = lerpedRotation;

                // Lerp the main colors of each material and assign it.
                Color lerpedColor = Color.Lerp(this.stateMaterial_.color, secondState.GetStateMaterial().color, t);
                element_.gameObject.GetComponent<Renderer>().material.color = lerpedColor;
            }
        }

        // Getter functions for the state's data.
        public VisualElement GetStateObject() { return element_; }
        public Material GetStateMaterial() { return stateMaterial_; }
        public Transform GetStateTransform() { return stateTransform_; }

        // Each visual state keeps track of the correspending element's transform and material
        // at the time the state was instantiated.
        private VisualElement element_;
        private Material stateMaterial_;
        private Transform stateTransform_;
    }
}
