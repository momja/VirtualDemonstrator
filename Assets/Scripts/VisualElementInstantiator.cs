using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualElementInstantiator : MonoBehaviour
{
    public GameObject visualElementPrefab;

    public void InstantiateVisualElement()
    {
        GameObject visualElement = Instantiate(visualElementPrefab, transform.position, transform.rotation);
        visualElement.transform.localScale = transform.localScale * 3f;
    }
}
