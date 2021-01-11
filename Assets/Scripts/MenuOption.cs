using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace VirtualDemonstrator {
    public class MenuOption : MonoBehaviour {
        private MenuOptionInfo info;
        public string optionTitle;
        public Image icon;
        public Outline outline;
        public Color hoverColor = new Color(0.902f,0.945f,0.0706f);
        public Color selectColor = new Color(0.109f,0.925f,0.270f);
        public GameObject selectionIndicator;

        private void Start() {
            this.outline = GetComponent<Outline>();
        }

        private void Update() {
        }

        public void SetInfo(MenuOptionInfo info) {
            this.info = info;
        }

        public void HoverEntered() {
            selectionIndicator.SetActive(true);
            selectionIndicator.transform.SetParent(transform);
            selectionIndicator.transform.localPosition = new Vector3(0.7f,0,0);
            //outline.enabled = true;
            //outline.OutlineColor = hoverColor;
        }

        public void HoverExited() {
            selectionIndicator.SetActive(false);
            selectionIndicator.transform.SetParent(null);
            // outline.enabled = false;
        }

        public void SelectEntered() {
            if (this.info.action != null) {
                this.info.action.Invoke();
            }
            //outline.OutlineColor = selectColor;
        }

        public void SelectExited() {
            //outline.OutlineColor = hoverColor;
        }
    }
}