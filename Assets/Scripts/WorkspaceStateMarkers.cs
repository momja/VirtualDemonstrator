using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkspaceStateMarkers : MonoBehaviour
{
    public GameObject WorkspaceStateMarkerPrefab;
    public Slider slider;
    public Transform panelStart;
    public Transform panelEnd;

    // Where to parent the markers
    public Transform markersParent;

    int numStates;
    float panelLength, distanceBetweenMarkers;
    Vector3 panelDir;

    void Awake()
    {
        panelLength = Vector3.Distance(panelEnd.position, panelStart.position);
        // slider.onValueChanged.AddListener(delegate {UpdateMarkers(); });
    }

    public void UpdateMarkers()
    {
        ClearChildren(markersParent);
        CreateNewMarkers();
    }

    void CreateNewMarkers()
    {
        numStates = (int) slider.maxValue + 1;
        if (numStates > 1) {
            distanceBetweenMarkers = panelLength / (numStates - 1);
        }
        else {
            distanceBetweenMarkers = 0;
        }
        panelDir = (panelEnd.position - panelStart.position).normalized;
        for (int i = 0; i < numStates; i++)
        {
            Vector3 markerPos = panelStart.position + panelDir * distanceBetweenMarkers * i;
            GameObject marker = Instantiate(WorkspaceStateMarkerPrefab, markersParent);
            marker.transform.position = markerPos;
        }
    }

    public void ClearChildren(Transform parent)
    {
        int i = 0;

        //Array to hold all child obj
        GameObject[] allChildren = new GameObject[parent.childCount];

        //Find all child obj and store to that array
        foreach (Transform child in parent)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }

        //Now destroy them
        foreach (GameObject child in allChildren)
        {
            DestroyImmediate(child.gameObject);
        }
    }
}
