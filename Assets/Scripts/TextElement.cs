using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace VirtualDemonstrator {
    public class TextElement : VisualElement {
        public TextMeshProUGUI text;
        public bool isEditing = false;
        public VRKeys.Keyboard keyboard;

        protected override void Start()
        {
            base.Start();
            this.keyboard = FindObjectOfType<VRKeys.Keyboard>();
            this.OnSelect.AddListener(ActivateKeyboard);
            this.OnSelectExit.AddListener(DeactivateKeyboard);
        }

        public void ActivateKeyboard() {
            isEditing = true;
            this.keyboard.Enable();
            this.keyboard.displayText = this.text;
        }

        public void DeactivateKeyboard() {
            isEditing = false;
            this.keyboard.Disable();
            this.keyboard.displayText = null;
        }

    }
}