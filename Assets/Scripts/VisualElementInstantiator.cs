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
            GameObject visualElement = Instantiate(visualElementPrefab, transform.position, transform.rotation);
            menu_.AddElement(visualElement.GetComponent<VisualElement>());
        }

        public MenuPanel menu_;
    }
}
