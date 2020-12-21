using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;


namespace VirtualDemonstrator
{   
    // This enum helps determine which action is currently being done to an object.
    enum TransformAction
    {
        TRANSLATION = 0,
        ROTATION = 1,
        SCALING = 2,
        IDLE = 3
    }

    // This class handles the users actions while an object is selected by the right hand.
    public class BimanualTransformer : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            // Attempt to reconnect with the left controller if it disconnects.
            if (reconnecting)
            {
                ConnectController();
                return;
            }

            // Get updated input from the "x" button on the Quest controller.
            bool prevRecessiveGripDown = recessiveGripDown;
            bool prevDominantGripDown = dominantGripDown;
            bool prevIdle = !prevRecessiveGripDown && !prevDominantGripDown;
            recessiveController.TryGetFeatureValue(CommonUsages.gripButton, out recessiveGripDown);
            dominantController.TryGetFeatureValue(CommonUsages.gripButton, out dominantGripDown);

            // If the "x" button was just pressed, cycle to the next interaction method.
            if (recessiveGripDown && dominantGripDown && (!prevDominantGripDown || !prevRecessiveGripDown)) {
                // Scaling
                scalingController = dominantControllerObject.transform.position - recessiveControllerObject.transform.position;
                scalingElement = selectedElement.transform.localScale;
                action = TransformAction.SCALING;
            }
            else if (recessiveGripDown && !prevRecessiveGripDown)
            {
                // Rotation
                rotationController = recessiveControllerObject.transform.localRotation;
                rotationElement = selectedElement.transform.localRotation;
                action = TransformAction.ROTATION;
            }
            else if (dominantGripDown && !prevDominantGripDown) {
                // Translation
                action = TransformAction.TRANSLATION;
            }
            else if (prevIdle) {
                // None
                selectedVisualElement.UpdateState();
                action = TransformAction.IDLE;
            }

            // If a selection is in progress, manage the user's transformations using bimanual techniques.
            if (selecting)
            {
                if (action == TransformAction.TRANSLATION)
                {
                    // Set selected object as child as of dominant controller
                    if (prevParent == null) {
                        prevParent = selectedElement.transform.parent;
                        selectedElement.transform.SetParent(dominantControllerObject.transform);
                    }
                }
                else if (action == TransformAction.ROTATION)
                {
                    if (selectedElement.transform.parent != null) {
                        selectedElement.transform.SetParent(prevParent);
                        prevParent = null;
                    }
                    // While holding the object with your right hand, it can mirror your left hand's rotation.
                    // selectedElement.transform.localRotation = recessiveControllerObject.transform.localRotation;
                    // selectedElement.transform.localRotation = rotationDifference * selectedElement.transform.localRotation;
                    Quaternion controllerDiff = recessiveControllerObject.transform.localRotation * Quaternion.Inverse(rotationController);
                    selectedElement.transform.localRotation = controllerDiff * rotationElement;
                }
                else if (action == TransformAction.SCALING)
                {
                    if (selectedElement.transform.parent != null) {
                        selectedElement.transform.SetParent(prevParent);
                        prevParent = null;
                    }
                    // float xFactor = scalingElement.x + scalingController.x - (dominantControllerObject.transform.position.x - recessiveControllerObject.transform.position.x);
                    // float yFactor = Math.Abs(dominantControllerObject.transform.position.y - recessiveControllerObject.transform.position.y);
                    // float zFactor = Math.Abs(dominantControllerObject.transform.position.z - recessiveControllerObject.transform.position.z);
                    selectedElement.transform.localScale = scalingElement + (dominantControllerObject.transform.position - recessiveControllerObject.transform.position) - scalingController;
                    // selectedElement.transform.localScale = new Vector3(xFactor, yFactor, zFactor);
                }
                else if (action == TransformAction.IDLE) {
                    if (selectedElement.transform.parent != null) {
                        selectedElement.transform.SetParent(prevParent);
                        prevParent = null;
                    }
                }
            }
        }


        // This function attempts to connect the controllers if it disconnects.
        private void ConnectController()
        {
            List<InputDevice> leftHandedControllers = new List<InputDevice>();
            var leftCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left;
            InputDevices.GetDevicesWithCharacteristics(leftCharacteristics, leftHandedControllers);

            if (leftHandedControllers.Count == 0)
            {
                Debug.Log("Attempting to connect to the left controller...");
                reconnecting = true;
            }
            else
            {
                reconnecting = false;
                recessiveController = leftHandedControllers[0];
            }

            List<InputDevice> rightHandedControllers = new List<InputDevice>();
            var rightCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right;
            InputDevices.GetDevicesWithCharacteristics(rightCharacteristics, rightHandedControllers);

            if (rightHandedControllers.Count == 0)
            {
                Debug.Log("Attempting to connect to the right controller...");
                reconnecting = true;
            }
            else
            {
                reconnecting = false;
                dominantController = rightHandedControllers[0];
            }
        }


        // This method gets called whenever the dominant controller selects an object.
        public void ElementSelected()
        {
            selecting = true;
            selectedElement = dominantInteractor.selectTarget.gameObject;
            selectedVisualElement = selectedElement.GetComponent<VisualElement>();
            // rotationDifference = selectedElement.transform.localRotation * Quaternion.Inverse(recessiveControllerObject.transform.localRotation);
        }


        // This method gets called whenever the dominant controller releases an object.
        public void ElementReleased() 
        {
            selecting = false;
            if (selectedElement.transform.parent != null) {
                selectedElement.transform.SetParent(prevParent);
                prevParent = null;
            }
        }


        // These objects help with getting transform data.
        public GameObject dominantControllerObject;
        public GameObject recessiveControllerObject;
        public XRRayInteractor dominantInteractor;
        private InputDevice dominantController;
        private InputDevice recessiveController;

        // These values help with the transformation actions.
        private bool selecting = false;
        private GameObject selectedElement = null;
        private VisualElement selectedVisualElement = null;
        private Vector3 scalingController;
        private Vector3 scalingElement;
        private Quaternion rotationController;
        private Quaternion rotationElement;
        private TransformAction action = TransformAction.IDLE;
        private Transform prevParent;
        private bool dominantGripDown = false;

        // This value is true when the left controller loses connection.
        private bool reconnecting = true;

        bool recessiveGripDown = false;
        bool xButtonPrev = false;
    }
}
