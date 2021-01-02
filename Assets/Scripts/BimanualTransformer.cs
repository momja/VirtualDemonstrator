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

        void Start() {
            this.rig = FindObjectOfType<XRRig>();
        }

        // Update is called once per frame
        void Update()
        {
            // Attempt to reconnect with the left controller if it disconnects.
            if (reconnecting)
            {
                ConnectController();
                return;
            }

            // Get controller midpoint for scaling helper
            Vector3 ctrlMidpoint = ControllerMidpoint();

            // Get updated input from the "x" button on the Quest controller.
            bool prevRecessiveGripDown = recessiveGripDown;
            bool prevDominantGripDown = dominantGripDown;
            bool prevIdle = !prevRecessiveGripDown && !prevDominantGripDown;
            recessiveController.TryGetFeatureValue(CommonUsages.gripButton, out recessiveGripDown);
            dominantController.TryGetFeatureValue(CommonUsages.gripButton, out dominantGripDown);
            bool xPressed = false;
            recessiveController.TryGetFeatureValue(CommonUsages.primaryButton, out xPressed);

            if (recessiveGripDown && dominantGripDown && (!prevDominantGripDown || !prevRecessiveGripDown))
            {
                // Scaling (absolute)
                scalingController = (dominantControllerObject.transform.position - recessiveControllerObject.transform.position);
                // scalingController = new Vector3(Mathf.Abs(scalingController.x),
                //                                 Math.Abs(scalingController.y),
                //                                 Mathf.Abs(scalingController.z));

                // instantiate mini scale object
                miniObject = Instantiate(selectedElement.transform,
                                         ControllerMidpoint(),
                                         RotationAlongControllerAxis(),
                                         null);
                miniObject.transform.localScale = 0.1f*selectedElement.transform.localScale;
                lineBtwnControllers.enabled = true;
                scalingElement = selectedElement.transform.localScale;
                action = TransformAction.SCALING;
            }
            else if (recessiveGripDown && !prevRecessiveGripDown)
            {
                // Rotation
                if (miniObject != null) {
                    GameObject.Destroy(miniObject.gameObject, 0);
                    miniObject = null;
                    lineBtwnControllers.enabled = false;
                }
                rotationController = recessiveControllerObject.transform.localRotation;
                rotationElement = selectedElement.transform.localRotation;
                action = TransformAction.ROTATION;
            }
            else if (dominantGripDown && !prevDominantGripDown)
            {
                // Translation
                if (miniObject != null) {
                     GameObject.Destroy(miniObject.gameObject, 0);
                    miniObject = null;
                    lineBtwnControllers.enabled = false;
                }
                action = TransformAction.TRANSLATION;
            }
            else if (prevIdle)
            {
                // Idle on last update
                if (selectedVisualElement != null)
                {
                    if (miniObject != null) {
                        GameObject.Destroy(miniObject.gameObject, 0);
                        miniObject = null;
                        lineBtwnControllers.enabled = false;
                    }
                    selectedVisualElement.UpdateState();
                }
                action = TransformAction.IDLE;
            }

            // If a selection is in progress, manage the user's transformations using bimanual techniques.
            if (selecting)
            {
                if (action == TransformAction.TRANSLATION)
                {
                    // Set selected object as child as of dominant controller
                    if (prevParent == null)
                    {
                        prevParent = selectedElement.transform.parent;
                        selectedElement.transform.SetParent(dominantControllerObject.transform);
                    }
                }
                else if (action == TransformAction.ROTATION)
                {
                    if (selectedElement.transform.parent != null)
                    {
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
                    // Set miniobject position
                    miniObject.position = ctrlMidpoint;
                    miniObject.rotation = RotationAlongControllerAxis();
                    miniObject.transform.localScale = 0.1f*selectedElement.transform.localScale;

                    // Set positions of Line Renderer
                    lineBtwnControllers.SetPosition(0, recessiveControllerObject.transform.position);
                    lineBtwnControllers.SetPosition(1, ctrlMidpoint);
                    lineBtwnControllers.SetPosition(2, dominantControllerObject.transform.position);

                    if (selectedElement.transform.parent != null)
                    {
                        selectedElement.transform.SetParent(prevParent);
                        prevParent = null;
                    }
                    if (xPressed)
                    {
                        Vector3 scaling = (dominantControllerObject.transform.position - recessiveControllerObject.transform.position) - scalingController;
                        if (scaling.x > scaling.y && scaling.x > scaling.z)
                        {
                            selectedElement.transform.localScale = scalingElement + new Vector3(scaling.x, 0, 0);
                        }
                        else if (scaling.y > scaling.x && scaling.y > scaling.z)
                        {
                            selectedElement.transform.localScale = scalingElement + new Vector3(0, scaling.y, 0);
                        }
                        else if (scaling.z > scaling.y && scaling.z > scaling.x)
                        {
                            selectedElement.transform.localScale = scalingElement + new Vector3(0, 0, scaling.z);
                        }
                    }
                    else
                    {
                        Vector3 curControllerDiff = (dominantControllerObject.transform.position - recessiveControllerObject.transform.position);
                        Vector3 scalingFactor = curControllerDiff - scalingController;
                        selectedElement.transform.localScale = scalingElement + curControllerDiff - scalingController;
                    }
                }
                else if (action == TransformAction.IDLE)
                {
                    if (selectedElement.transform.parent != null)
                    {
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


        // Finds the world space point between the dominant and recessive controllers.
        private Vector3 ControllerMidpoint() {
            Vector3 domToRec = recessiveControllerObject.transform.position - dominantControllerObject.transform.position;
            float domToRecMid = domToRec.magnitude/2;
            Vector3 midpoint = dominantControllerObject.transform.position + domToRecMid * domToRec.normalized;
            return midpoint;
        }

        private Quaternion RotationAlongControllerAxis() {
            Vector3 forward = this.ControllerMidpoint() - this.rig.transform.position;
            forward.y = 0; // Cancel up direction component
            Vector3 upward = Vector3.up;
            return Quaternion.LookRotation(forward, upward);
        }

        // This method gets called whenever the dominant controller selects an object.
        public void ElementSelected()
        {
            selectedElement = dominantInteractor.selectTarget.gameObject;
            selectedVisualElement = selectedElement.GetComponent<VisualElement>();
            if (selectedVisualElement != null)
            {
                selecting = true;
            }
            else
            {
                selectedElement = null;
            }
            // rotationDifference = selectedElement.transform.localRotation * Quaternion.Inverse(recessiveControllerObject.transform.localRotation);
        }


        // This method gets called whenever the dominant controller releases an object.
        public void ElementReleased()
        {
            selecting = false;
            if (selectedElement.transform.parent != null)
            {
                selectedElement.transform.SetParent(prevParent);
                prevParent = null;
            }
        }


        // These objects help with getting transform data.
        public GameObject dominantControllerObject;
        public GameObject recessiveControllerObject;
        public XRRayInteractor dominantInteractor;
        public LineRenderer lineBtwnControllers;

        private InputDevice dominantController;
        private InputDevice recessiveController;
        private XRRig rig;
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
        private Transform miniObject = null;
        private bool xButtonPrev = false;
        private bool recessiveGripDown = false;
        // This value is true when the left controller loses connection.
        private bool reconnecting = true;

    }
}
