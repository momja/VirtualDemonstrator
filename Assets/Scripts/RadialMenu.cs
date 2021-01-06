using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

namespace VirtualDemonstrator
{
    public class RadialMenu : MonoBehaviour
    {
        private List<GameObject> MenuOptionObjects = new List<GameObject>();
        private InputDevice CameraNode;
        private Transform attached;
        public List<MenuOptionInfo> MenuOptions;
        public GameObject defaultOptionObject;

        public float degreesPerOption = 10f;
        public float degreesSpacing = 11f;
        public float innerRadius = 0.05f;
        public float outerRadius = 0.2f;
        public Vector3 centerOffset = new Vector3(0, 0, 0);
        public Material optionMaterial;

        private void Start()
        {
            RefreshMenu();
            HideMenu(true);
        }

        private void Update()
        {
            if (attached)
            {
                // RefreshMenu();
                UpdatePosition(attached);
            }
        }

        public void Attach(Transform element)
        {
            if (CameraNode == null || CameraNode.characteristics == InputDeviceCharacteristics.None)
            {
                this.CameraNode = XRExtenders.XRHelpers.GetCameraNode();
            }
            this.gameObject.SetActive(true);
            UpdatePosition(element);
            attached = element;
        }

        public void Detach()
        {
            attached = null;
            this.gameObject.SetActive(false);
        }

        /// Certain elements will require different menu elements
        /// RefreshMenu can be called to re-generate the meshes
        private void RefreshMenu()
        {
            foreach (Transform child in transform)
            {
                // remove all existing elements
                GameObject.Destroy(child.gameObject);
            }
            Mesh mesh = GenerateMesh();
            MenuOptionObjects = new List<GameObject>();
            int i = 0;
            foreach (MenuOptionInfo option in MenuOptions)
            {
                GameObject newOption = Instantiate(defaultOptionObject, Vector3.zero, Quaternion.identity, transform);
                MenuOptionObjects.Add(newOption);
                newOption.GetComponent<MeshRenderer>().material = optionMaterial;
                newOption.GetComponent<MeshFilter>().mesh = mesh;
                newOption.GetComponent<MeshCollider>().sharedMesh = mesh;
                newOption.GetComponent<MenuOption>().SetInfo(option);
                newOption.transform.GetComponentInChildren<TextMeshPro>().text = option.name;
                newOption.transform.Rotate(Vector3.forward, ((float)MenuOptions.Count / 2 - (float)i) * degreesSpacing, Space.Self);
                i += 1;
            }
        }

        private void UpdatePosition(Transform element)
        {
            Vector3 cameraPosition;
            CameraNode.TryGetFeatureValue(CommonUsages.centerEyePosition, out cameraPosition);

            Bounds bounds;
            if (element.childCount == 1)
            {
                bounds = element.GetComponentInChildren<VisualElement>().GetBounds();
            }
            else
            {
                bounds = new Bounds(element.position, Vector3.one * 0.001f);
                foreach (Transform child in element)
                {
                    bounds.Encapsulate(child.GetComponent<VisualElement>().GetBounds());
                }
            }

            Vector3 objectCenter = bounds.center;
            float objectRadius = Mathf.Max(bounds.extents.x, bounds.extents.z);
            transform.position = objectCenter;
            Vector3 lookPos = objectCenter - cameraPosition;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = rotation;
            transform.Rotate(Vector3.up, 15, Space.World);
            transform.Translate(Vector3.right * objectRadius + centerOffset, Space.Self);
        }

        /// Generates a mesh for option given the current option cound
        private Mesh GenerateMesh()
        {
            Mesh mesh = new Mesh();

            List<Vector3> newVertices = new List<Vector3>();
            List<int> newTriangles = new List<int>();

            float degrees = degreesPerOption / 2f;

            newVertices.Add(new Vector3(innerRadius * Mathf.Cos(-degrees * Mathf.Deg2Rad), innerRadius * Mathf.Sin(-degrees * Mathf.Deg2Rad), 0));
            newVertices.Add(new Vector3(innerRadius * Mathf.Cos(degrees * Mathf.Deg2Rad), innerRadius * Mathf.Sin(degrees * Mathf.Deg2Rad), 0));
            newVertices.Add(new Vector3(outerRadius * Mathf.Cos(degrees * Mathf.Deg2Rad), outerRadius * Mathf.Sin(degrees * Mathf.Deg2Rad), 0));
            newVertices.Add(new Vector3(outerRadius * Mathf.Cos(-degrees * Mathf.Deg2Rad), outerRadius * Mathf.Sin(-degrees * Mathf.Deg2Rad), 0));

            newTriangles.Add(0);
            newTriangles.Add(1);
            newTriangles.Add(2);
            newTriangles.Add(3);
            newTriangles.Add(0);
            newTriangles.Add(2);

            mesh.Clear();
            mesh.vertices = newVertices.ToArray();
            mesh.triangles = newTriangles.ToArray();
            mesh.Optimize();
            mesh.RecalculateNormals();

            return mesh;
        }

        public void SetMenuOptions(List<MenuOptionInfo> options)
        {
            this.MenuOptions = options;
        }

        public void HideMenu(bool hide)
        {
            this.gameObject.SetActive(!hide);
        }
    }
}