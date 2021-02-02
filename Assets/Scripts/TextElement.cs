using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace VirtualDemonstrator
{
    public class TextElement : VisualElement
    {
        public TextMeshProUGUI text;
        public Image panel;
        public bool isEditing = false;
        public VRKeys.Keyboard keyboard;

        protected override void Start()
        {
            base.Start();
            this.keyboard = FindObjectOfType<VRKeys.Keyboard>();
            this.keyboard.OnSubmit.AddListener(text => DeactivateKeyboard());
            this.OnSelectExit.AddListener(DeactivateKeyboard);
            this.outline = this.text.GetComponent<Outline>();
        }

        public void ActivateKeyboard()
        {
            isEditing = true;
            this.keyboard.Enable();
            this.keyboard.displayText = this.text;
            this.keyboard.SetText(this.text.text);
            workspace.KeyboardMode(true);
        }

        public void DeactivateKeyboard()
        {
            isEditing = false;
            this.keyboard.Disable();
            this.keyboard.displayText = null;
            workspace.KeyboardMode(false);
        }

        public override void HoverEntered()
        {
            if (!selected)
            {
                panel.color = hoverColor;
            }
            base.HoverEntered();
        }

        public override void HoverExited()
        {
            if (!selected)
            {
                panel.color = Color.clear;
            }
            base.HoverExited();
        }

        public override void SelectEntered()
        {
            panel.color = selectColor;
            base.SelectEntered();
        }

        public override void SelectExited()
        {
            panel.color = Color.clear;
            base.SelectExited();
        }

        public override Bounds GetBounds()
        {
            Bounds bounds = new Bounds(text.transform.position, new Vector3(text.rectTransform.rect.width, text.rectTransform.rect.height, 0.0f));
            return bounds;
        }
    }
}