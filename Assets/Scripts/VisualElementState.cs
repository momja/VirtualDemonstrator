using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualDemonstrator {
    public class VisualElementState {
        public VisualElement element;
        public Transform transform;

        public VisualElementState(VisualElement element) {
            this.element = element;
        }

        public void setVizElementToState(float t) {
            // t can be used for interpolation
            this.element.gameObject.transform.position = this.transform.position;
            this.element.gameObject.transform.rotation = this.transform.rotation;
            this.element.gameObject.transform.localScale = this.transform.localScale;
        }
    }
}