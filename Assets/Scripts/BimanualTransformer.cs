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
        ROTATION = 0,
        SCALING = 1
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
            primaryButtonPrev = primaryButtonPressed;
            recessiveController.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonPressed);

            // If the "x" button was just pressed, cycle to the next interaction method.
            if (primaryButtonPressed && !primaryButtonPrev)
            {
                action = (TransformAction)(((int)action + 1) % 2);
            }

            // If a selection is in progress, manage the user's transformations using bimanual techniques.
            if (selecting)
            {
                if (action == TransformAction.ROTATION)
                {
                    // While holding the object with your right hand, it can mirror your left hand's rotation.
                    selectedElement.transform.localRotation = recessiveControllerObject.transform.localRotation;
                }
                else if (action == TransformAction.SCALING)
                {
                    float xFactor = Math.Abs(dominantControllerObject.transform.position.x - recessiveControllerObject.transform.position.x);
                    float yFactor = Math.Abs(dominantControllerObject.transform.position.y - recessiveControllerObject.transform.position.y);
                    float zFactor = Math.Abs(dominantControllerObject.transform.position.z - recessiveControllerObject.transform.position.z);
                    selectedElement.transform.localScale = new Vector3(xFactor, yFactor, zFactor);
                }
            }
        }


        // This function attempts to connect the recessive controller if it disconnects.
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
        }


        // This method gets called whenever the dominant controller selects an object.
        public void ElementSelected()
        {
            selecting = true;
            selectedElement = dominantInteractor.selectTarget.gameObject;
        }


        // This method gets called whenever the dominant controller releases an object.
        public void ElementReleased() { selecting = false; }


        // These objects help with getting transform data.
        public GameObject dominantControllerObject;
        public GameObject recessiveControllerObject;
        public XRRayInteractor dominantInteractor;
        private InputDevice recessiveController;

        // These values help with the transformation actions.
        private bool selecting = false;
        private GameObject selectedElement = null;
        private TransformAction action = TransformAction.ROTATION;

        // This value is true when the left controller loses connection.
        private bool reconnecting = false;

        bool primaryButtonPressed = false;
        bool primaryButtonPrev = false;
    }
}
