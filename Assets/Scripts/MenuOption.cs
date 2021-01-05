using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VirtualDemonstrator {
    public class MenuOption : MonoBehaviour {
        public string optionTitle;
        public Image icon;

        public UnityEvent OnHoverEntered;
        public UnityEvent OnHoverExited;
        public UnityEvent OnSelectEntered;
        public UnityEvent OnSelectExited;

        private void Start() {

        }

        private void Update() {

        }

        public void HoverEntered() {
            OnHoverExited.Invoke();
        }

        public void HoverExited() {
            OnHoverExited.Invoke();
        }

        public void SelectEntered() {
            OnSelectEntered.Invoke();
        }

        public void SelectExited() {
            OnSelectExited.Invoke();
        }
    }
}