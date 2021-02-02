using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualDemonstrator
{
    public class VisualElementInstantiator : MonoBehaviour
    {
        public GameObject visualElementPrefab;

        public void InstantiateVisualElement()
        {
            GameObject visElObj = Instantiate(visualElementPrefab, transform.position, transform.rotation);
            // Provide a unique ID used for serialization
            visElObj.name = $"element_{visElObj.GetInstanceID()}";
            menu_.AddElement(visElObj.GetComponent<VisualElement>());
        }
        public MenuPanel menu_;
    }
}