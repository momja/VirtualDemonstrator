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
<<<<<<< HEAD
        void Start() {
=======
        private void Start()
        {
>>>>>>> master
            this.rig = FindObjectOfType<XRRig>();
            this.xrInteractionManager = FindObjectOfType<XRInteractionManager>();
        }

        // Update is called once per frame
        private void Update()
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
            recessiveController.TryGetFeatureValue(CommonUsages.gripButton, out recessiveGripDown);
            dominantController.TryGetFeatureValue(CommonUsages.gripButton, out dominantGripDown);
            bool idle = (prevRecessiveGripDown || prevDominantGripDown) && (!recessiveGripDown && !dominantGripDown);

            bool triggerDown;
            dominantController.TryGetFeatureValue(CommonUsages.triggerButton, out triggerDown);
            if (triggerDown) {
                // if no intersection with visual element, remove all elements
                if (dominantInteractor.selectTarget == null) {
                    // remove all selection
                    Workspace.Instance.ClearSelectedElements();
                    selecting = false;
                    Workspace.Instance.DetachMenu();
                }
            }

            Workspace.Instance.selectionParent.SetParent(null);

            if (recessiveGripDown && dominantGripDown && (!prevDominantGripDown || !prevRecessiveGripDown))
            {
                // Scaling (absolute)
                scalingController = (dominantControllerObject.transform.position - recessiveControllerObject.transform.position);
                // instantiate mini scale object
                miniObject = Instantiate(Workspace.Instance.selectionParent.transform,
                                         ControllerMidpoint(),
                                         RotationAlongControllerAxis(),
                                         null);
                miniObject.transform.localScale = 0.1f * Workspace.Instance.selectionParent.transform.localScale;
                lineBtwnControllers.enabled = true;
                scalingElement = Workspace.Instance.selectionParent.transform.localScale;
                action = TransformAction.SCALING;

                // If a scaling operation was just started, define which axis to scale.
                if (!scalingDown)
                {
                    scalingDown = true;
                    scalingX = false;
                    scalingY = false;
                    scalingZ = false;

                    // get the current distance between the controllers to act as the "0-value".
                    Vector3 positionDifference = recessiveControllerObject.transform.InverseTransformPoint(dominantControllerObject.transform.position);
                    startingDistance = positionDifference.magnitude;

                    // Determine which axis to start scaling based on the xyz distances of the controllers.
                    float maxAxis = Mathf.Max(Mathf.Abs(positionDifference.x), Mathf.Max(Mathf.Abs(positionDifference.y), positionDifference.z));
                    if (maxAxis == Mathf.Abs(positionDifference.x))
                    {
                        scalingX = true;
                    }
                    else if (maxAxis == Mathf.Abs(positionDifference.y))
                    {
                        scalingY = true;
                    }
                    else
                    {
                        scalingZ = true;
                    }
                }
            }
            else if (recessiveGripDown && !prevRecessiveGripDown)
            {
                // Rotation
                Workspace.Instance.HideMenuAndTimeline(true);
                if (miniObject != null)
                {
                    GameObject.Destroy(miniObject.gameObject, 0);
                    miniObject = null;
                    lineBtwnControllers.enabled = false;
                }
                rotationController = recessiveControllerObject.transform.localRotation;
                rotationElement = Workspace.Instance.selectionParent.localRotation;
                action = TransformAction.ROTATION;

                scalingDown = false;
            }
            else if (dominantGripDown && !prevDominantGripDown)
            {
                // Translation
                if (miniObject != null)
                {
                    GameObject.Destroy(miniObject.gameObject, 0);
                    miniObject = null;
                    lineBtwnControllers.enabled = false;
                }
                action = TransformAction.TRANSLATION;

                scalingDown = false;
            }
            else if (idle)
            {
                // Idle
                Workspace.Instance.HideMenuAndTimeline(false);
                if (miniObject != null)
                {
                    GameObject.Destroy(miniObject.gameObject, 0);
                    miniObject = null;
                    lineBtwnControllers.enabled = false;
                }
                Workspace.Instance.UpdateSelectedElementStates();
                action = TransformAction.IDLE;

                scalingDown = false;
            }

            // If a selection is in progress, manage the user's transformations using bimanual techniques.
            if (selecting)
            {
                if (action == TransformAction.TRANSLATION)
                {
                    // Set selected object as child of dominant controller
                    Workspace.Instance.selectionParent.SetParent(dominantControllerObject.transform);
                }
                else if (action == TransformAction.ROTATION)
                {
                    // While holding the object with your right hand, it can mirror your left hand's rotation.
                    Quaternion controllerDiff = recessiveControllerObject.transform.localRotation * Quaternion.Inverse(rotationController);
                    Workspace.Instance.selectionParent.localRotation = controllerDiff * rotationElement;
                }
                else if (action == TransformAction.SCALING)
                {
                    // Set miniobject position
                    miniObject.position = ctrlMidpoint;
                    miniObject.rotation = RotationAlongControllerAxis();
                    miniObject.transform.localScale = 0.1f * Workspace.Instance.selectionParent.localScale;

                    // Set positions of Line Renderer
                    lineBtwnControllers.SetPosition(0, recessiveControllerObject.transform.position);
                    lineBtwnControllers.SetPosition(1, ctrlMidpoint);
                    lineBtwnControllers.SetPosition(2, dominantControllerObject.transform.position);

                    // Calculate the scale operation.

                    // Get the current distance between the two controllers and get the difference between the original distance.
                    Vector3 dominantInRecessiveSpace = recessiveControllerObject.transform.InverseTransformPoint(dominantControllerObject.transform.position);
                    float currentDistance = dominantInRecessiveSpace.magnitude;
                    float scaleValue = currentDistance - startingDistance;

                    // Determine what axis to apply the change to.
                    Vector3 scalingFactor = new Vector3();
                    if (scalingX)
                    {
                        scalingFactor.x = scaleValue * 30;
                    }
                    else if (scalingY)
                    {
                        scalingFactor.y = scaleValue * 30;
                    }
                    else
                    {
                        scalingFactor.z = scaleValue * 30;
                    }

                    // Clamp the scale to non-negative values.
                    Vector3 frameContribution = scalingElement + scalingFactor;
                    if (frameContribution.x < 0)
                    {
                        frameContribution.x = 0;
                    }
                    if (frameContribution.y < 0)
                    {
                        frameContribution.y = 0;
                    }
                    if (frameContribution.z < 0)
                    {
                        frameContribution.z = 0;
                    }

                    // Apply the scale contribution.
                    Workspace.Instance.selectionParent.localScale = frameContribution;
                }
                else if (action == TransformAction.IDLE)
                {

                }
            }

            Vector3 bounds = Workspace.Instance.workspaceBounds.bounds;
            List<VisualElement> outOfBoundsElements = new List<VisualElement>();
            bool allElementsInBounds = true;
            foreach (VisualElement element in Workspace.Instance.selVisualElements)
            {
                if (!Workspace.Instance.workspaceBounds.InBounds(element.transform))
                {
                    // out of bounds
                    // if selecting is off, delete, if selecting is on set boundaries on
                    Workspace.Instance.workspaceBounds.HideWalls(false);
                    allElementsInBounds = false;
                }
            }
            if (allElementsInBounds)
            {
                Workspace.Instance.workspaceBounds.HideWalls(true);
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

        private Vector3 ClipVec(Vector3 vec,
                                float clipMinX = 0, float clipMaxX = Mathf.Infinity,
                                float clipMinY = 0, float clipMaxY = Mathf.Infinity,
                                float clipMinZ = 0, float clipMaxZ = Mathf.Infinity)
        {
            return new Vector3(Mathf.Clamp(vec.x, clipMinX, clipMaxX),
                               Mathf.Clamp(vec.y, clipMinY, clipMaxY),
                               Mathf.Clamp(vec.z, clipMinZ, clipMaxZ));
        }

        // Finds the world space point between the dominant and recessive controllers.
        private Vector3 ControllerMidpoint()
        {
            Vector3 domToRec = recessiveControllerObject.transform.position - dominantControllerObject.transform.position;
            float domToRecMid = domToRec.magnitude / 2;
            Vector3 midpoint = dominantControllerObject.transform.position + domToRecMid * domToRec.normalized;
            return midpoint;
        }

        private Quaternion RotationAlongControllerAxis()
        {
            Vector3 forward = this.ControllerMidpoint() - this.rig.transform.position;
            forward.y = 0; // Cancel up direction component
            Vector3 upward = Vector3.up;
            return Quaternion.LookRotation(forward, upward);
        }

        private void CleanupOutOfBounds()
        {
            // Check if any object is outside bounds
            Vector3 bounds = Workspace.Instance.workspaceBounds.bounds;
            List<VisualElement> outOfBoundsElements = new List<VisualElement>();
            foreach (VisualElement element in Workspace.Instance.selVisualElements)
            {
                if (!Workspace.Instance.workspaceBounds.InBounds(element.transform))
                {
                    // out of bounds
                    // if selecting is off, delete, if selecting is on set boundaries on
                    outOfBoundsElements.Add(element);
                }
            }
            // remove out of bounds elements
            foreach (VisualElement element in outOfBoundsElements)
            {
                Workspace.Instance.DeleteElement(element);
            }

        }

        // This method gets called whenever the dominant controller selects an object.
        public void ElementSelected()
        {
            GameObject selectedElement = dominantInteractor.selectTarget.gameObject;
            Workspace.Instance.selectionParent.SetParent(null);
            VisualElement selVisElem = selectedElement.GetComponent<VisualElement>();
            if (selVisElem == null)
            {
                // remove all selection
                Workspace.Instance.ClearSelectedElements();
                selecting = false;
                Workspace.Instance.DetachMenu();
            }
            else if (Workspace.Instance.selVisualElements.Contains(selVisElem))
            {
                // remove from list
                CleanupOutOfBounds();
                Workspace.Instance.RemoveSelectedElement(selVisElem);
            }
            else
            {
                // add new element
                Workspace.Instance.AddSelectedElement(selVisElem);
                Workspace.Instance.AttachMenu(selVisElem);
                selecting = true;
            }
        }

        // These objects help with getting transform data.
        public GameObject dominantControllerObject;
        public GameObject recessiveControllerObject;
        public XRRayInteractor dominantInteractor;
        public XRInteractionManager xrInteractionManager;
        public LineRenderer lineBtwnControllers;

        private InputDevice dominantController;
        private InputDevice recessiveController;
        private XRRig rig;
        // These values help with the transformation actions.
        private bool selecting = false;
        private Vector3 scalingController;
        private Vector3 scalingElement;
        private Quaternion rotationController;
        private Quaternion rotationElement;
        private TransformAction action = TransformAction.IDLE;
        private bool dominantGripDown = false;
        private Transform miniObject = null;
        private bool xButtonPrev = false;
        private bool recessiveGripDown = false;
        // This value is true when the left controller loses connection.
        private bool reconnecting = true;

        // Values needed for David's scaling technique.
        private float startingDistance;
        private bool scalingDown = true;
        private bool scalingX = false;
        private bool scalingY = false;
        private bool scalingZ = false;
    }
}
