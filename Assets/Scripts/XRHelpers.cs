using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace XRExtenders
{
    public static class XRHelpers {
        public static InputDevice GetCameraNode() {
            List<InputDevice> Cameras = new List<InputDevice>();
            var cameraCharacteristices = InputDeviceCharacteristics.HeadMounted | InputDeviceCharacteristics.TrackedDevice;
            InputDevices.GetDevicesWithCharacteristics(cameraCharacteristices, Cameras);
            return Cameras[0];
        }
    }
}
    