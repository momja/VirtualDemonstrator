using UnityEngine;

public struct VisualElementData {
    // transform information
    public Vector3 position;
    public Vector3 scale;
    public Quaternion rotation;
    public Color color;
    // used to access the file from Assets/Resources/VisualElements
    public string elementName;
}