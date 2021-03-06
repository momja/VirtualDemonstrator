using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VirtualDemonstrator {
    public class VisualElementState {
        // A constructor that takes in a game object and saves its current material and transform.
        public VisualElementState(VisualElement element)
        {
            this.element_ = element;
            stateMaterial_ = element.GetComponent<Renderer>().material;
            statePosition_ = element.gameObject.transform.position;
            stateScale_ = element.gameObject.transform.localScale;
            stateRotation_ = element.gameObject.transform.rotation;
        }

        public VisualElementState(VisualElementState newState)
        {
            this.element_ = newState.GetStateElement();
            this.statePosition_ = newState.GetStatePosition();
            this.stateScale_ = newState.GetStateScale();
            this.stateRotation_ = newState.GetStateRotation();
            this.stateMaterial_ = newState.GetStateMaterial();
        }

        // This function assigns a blend of this state with another.
        public void BlendStates(VisualElementState secondState, float t)
        {
            // A t-value of 0 will assign the state stored in the caller.
            // A t-value of 1 will assign the state passed into the function.
            // A t-value between 0 and 1 will assign a blend of the two states.
            if (t >= 0 && t <= 1) {
                // Interpolate the transform values and assign them.
                Vector3 secondPosition = secondState.GetStatePosition();
                Vector3 secondScale = secondState.GetStateScale();
                Quaternion secondRotation = secondState.GetStateRotation();
                Vector3 lerpedPosition = Vector3.Lerp(this.statePosition_, secondPosition, t);
                Vector3 lerpedScale = Vector3.Lerp(this.stateScale_, secondScale, t);
                Quaternion lerpedRotation = Quaternion.Slerp(this.stateRotation_, secondRotation, t);
                element_.gameObject.transform.position = lerpedPosition;
                element_.gameObject.transform.localScale = lerpedScale;
                element_.gameObject.transform.rotation = lerpedRotation;

                // Lerp the main colors of each material and assign it.
                Renderer rend = element_.GetComponent<Renderer>();
                rend.material.Lerp(this.stateMaterial_, secondState.GetStateMaterial(), t);
            }
        }

        // Getter functions for the state's data.
        public VisualElement GetStateElement() { return element_; }
        public Material GetStateMaterial() { return stateMaterial_; }
        public Vector3 GetStatePosition() { return statePosition_; }
        public Vector3 GetStateScale() { return stateScale_; }
        public Quaternion GetStateRotation() { return stateRotation_; }

        public void SetStatePosition(Vector3 newPosition) { statePosition_ = newPosition; }
        public void SetStateScale(Vector3 newScale) { stateScale_ = newScale; }
        public void SetStateRotation(Quaternion newRotation) { stateRotation_ = newRotation; }
        public void SetStateMaterial(Material material) { stateMaterial_ = material; }

        // Each visual state keeps track of the correspending element's transform and material
        // at the time the state was instantiated.
        private VisualElement element_;
        private Material stateMaterial_;
        private Vector3 statePosition_;
        private Vector3 stateScale_;
        private Quaternion stateRotation_;
        
    }
}
