using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

namespace VirtualDemonstrator {
    public class ColorMenu : RadialMenu {
        public override void Attach(Transform element, Transform latestElement, bool showEdit=false)
        {
            if (CameraNode == null || CameraNode.characteristics == InputDeviceCharacteristics.None)
            {
                this.CameraNode = XRExtenders.XRHelpers.GetCameraNode();
            }
            this.gameObject.SetActive(true);
            UpdatePosition(element);
            attached = element;
        }

        public override void Detach()
        {
            attached = null;
            this.gameObject.SetActive(false);
            if (this.colorMenu != null) {
                this.colorMenu.GetComponent<RadialMenu>().Detach();
            }
        }

    }
}
