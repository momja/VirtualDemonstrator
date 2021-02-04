using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.EventSystems;
using UnityEditor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VirtualDemonstrator
{

    public enum WorkspaceModes
    {
        Create = 0,
        Present = 1
    }
    /// <summary>
    /// Main Driver class for the virtual demonstrator.
    /// Choreographs interactions between various components.
    /// </summary>
    public class Workspace : MonoBehaviour
    {

        private static Workspace _instance;
        public static Workspace Instance { get { return _instance; } }

        public MenuPanel menuPanel;
        public Timeline timeline;
        public RadialMenu radialMenu;
        public HashSet<VisualElement> selVisualElements
        {
            get
            {
                return selectedVisualElements_;
            }
        }
        public Dictionary<VisualElement, UnityEngine.Object> allElements;
        public WorkspaceBounds workspaceBounds;
        public GameObject rayInteractor;
        public Transform selectionParent;
        public Transform nonSelectionParent;
        public WorkspaceModes curMode = WorkspaceModes.Create;
        private List<WorkspaceState> stateHistory_;
        private HashSet<VisualElement> selectedVisualElements_;
        private float prevSliderValue;
        private int stateIndex;
        private WorkspaceState _prevState;
        private WorkspaceState _curState;
        private int goalStateFrame = -1;
        private int startStateFrame = -1;
        private bool isLerping = false;
        private float lerpT = 0;

        // Start is called before the first frame update
        private void Start()
        {
            this.stateHistory_ = new List<WorkspaceState>();
            this.selectedVisualElements_ = new HashSet<VisualElement>();
            this.allElements = new Dictionary<VisualElement, UnityEngine.Object>();
            this.selectionParent = GameObject.Find("SelectionParent").transform;
            this.nonSelectionParent = null;
            this.timeline.workspace = this;
            this.menuPanel.workspace_ = this;
            this.workspaceBounds = GameObject.Find("Workspace Bounds").GetComponent<WorkspaceBounds>();
            //InsertNewState();
            _prevState = _curState;
            LoadWorkspace();
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (goalStateFrame != startStateFrame)
            {
                this.lerpT += 0.02f * Mathf.Abs(goalStateFrame - startStateFrame);
                float t = Easings.easeInOut(this.lerpT);
                if (this.lerpT > 1)
                {
                    if (this.stateIndex != goalStateFrame)
                    {
                        this.stateIndex = stateIndex + (int)Mathf.Sign(goalStateFrame - stateIndex) * 1;
                        this._prevState = this._curState;
                        this._curState = GetStateAtTime(this.stateIndex);
                        this.lerpT = 0.0f;
                    }
                    else
                    {
                        startStateFrame = goalStateFrame;
                    }
                }
                else if (this._curState != null)
                {
                    this.UpdateCurrentState(t);
                }
            }
        }

        private void UpdateSelectionParentPosition()
        {
            if (this.selectedVisualElements_.Count == 0)
            {
                this.selectionParent.position = Vector3.zero;
                this.selectionParent.rotation = Quaternion.identity;
                this.radialMenu.Detach();
                return;
            }
            foreach (VisualElement selElement in this.selectedVisualElements_)
            {
                // temporarily reset parent
                selElement.transform.SetParent(null);
            }
            this.selectionParent.localScale = Vector3.one;
            // set parent to the midpoint of the objects
            Vector3 positionMidPt = Vector3.zero;
            Quaternion rotationMidPt = Quaternion.identity;
            int i = 0;
            foreach (VisualElement selElement in this.selectedVisualElements_)
            {
                positionMidPt += selElement.transform.position;
                if (i == 0)
                {
                    rotationMidPt = selElement.transform.rotation;
                }
                else
                {
                    float weight = 1.0f / (float)(i + 1);
                    rotationMidPt = Quaternion.Slerp(rotationMidPt, selElement.transform.rotation, weight);
                }
                i += 1;
            }
            positionMidPt = positionMidPt / this.selectedVisualElements_.Count;
            this.selectionParent.position = positionMidPt;
            this.selectionParent.rotation = rotationMidPt;
            foreach (VisualElement selElement in this.selectedVisualElements_)
            {
                selElement.transform.SetParent(this.selectionParent);
            }
        }

        private void RefreshSelection()
        {
            HashSet<VisualElement> newSelection = new HashSet<VisualElement>();
            foreach (VisualElement element in this.selectedVisualElements_)
            {
                if (this._curState.ElementExists(element))
                {
                    newSelection.Add(element);
                }
                else
                {
                    element.SelectExited();
                    element.transform.SetParent(this.nonSelectionParent);
                }
            }
            this.UpdateSelectionParentPosition();
            this.selectedVisualElements_ = newSelection;
        }

        public void ToggleModes()
        {
            if (curMode == WorkspaceModes.Create)
            {
                SaveWorkspace();
                curMode = WorkspaceModes.Present;
                SceneManager.LoadScene("PresenterMode");
            }
            else if (curMode == WorkspaceModes.Present)
            {
                curMode = WorkspaceModes.Create;
                SceneManager.LoadScene("VirtualDemonstrator");
            }
        }

        /// Parse a JSON file into Workspace states
        [MenuItem("Tools/Load Workspace")]
        static public void LoadWorkspace()
        {
            string filename = "./workspace_save.json";
            JObject json = JObject.Parse(File.ReadAllText(filename));
            JObject elements = (JObject)json["Elements"];

            Dictionary<string, VisualElement> id_elements = new Dictionary<string, VisualElement>();

            foreach (JProperty p in elements.Properties())
            {
                string name = p.Name;
                string value = (string)p.Value;
                GameObject prefab = Resources.Load<GameObject>($"VisualElements/{value}");
                // FIXME: without renaming object, there could potentially be repeats
                // because the UID does not hold for this new workspace
                GameObject visElObj = Instantiate(prefab);
                visElObj.name = name;
                visElObj.transform.localScale = Vector3.zero;
                VisualElement visEl = visElObj.GetComponent<VisualElement>();
                id_elements.Add(name, visEl);
                Instance.allElements.Add(visEl, prefab);
            }

            JArray states = (JArray)json["States"];

            Instance.stateHistory_ = new List<WorkspaceState>();
            Instance._curState = null;

            int index = 0;
            // WorkspaceStates (arrays)
            foreach (JArray a in states.Children<JArray>())
            {
                Instance.InsertNewState(index);
                WorkspaceState WSState = Instance.GetStateAtTime(index);
                // VisualElementStates (arrays)
                foreach (JObject o in a.Children<JObject>())
                {
                    // VisualElements (objects)
                    List<JProperty> properties = o.Properties().ToList<JProperty>();

                    // name
                    string name = (string)properties[0].Value;
                    VisualElement element = id_elements[name];

                    // position
                    Vector3 position = Vector3.zero;
                    string[] values_pos = ((string)properties[1].Value).Split(' ');
                    position.x = float.Parse(values_pos[0]);
                    position.y = float.Parse(values_pos[1]);
                    position.z = float.Parse(values_pos[2]);

                    // scale
                    Vector3 scale = Vector3.zero;
                    string[] values_sc = ((string)properties[2].Value).Split(' ');
                    scale.x = float.Parse(values_sc[0]);
                    scale.y = float.Parse(values_sc[1]);
                    scale.z = float.Parse(values_sc[2]);

                    // rotation
                    Quaternion rotation = Quaternion.identity;
                    string[] values_rot = ((string)properties[3].Value).Split(' ');
                    rotation.x = float.Parse(values_rot[0]);
                    rotation.y = float.Parse(values_rot[1]);
                    rotation.z = float.Parse(values_rot[2]);
                    rotation.w = float.Parse(values_rot[3]);

                    // material
                    Material material;
                    string material_name = (string)properties[4].Value;
                    material = Resources.Load<Material>($"Materials/{material_name}");

                    // text
                    string text = "";
                    if (properties.Count > 5)
                    {
                        text = (string)properties[5].Value;
                        TextElement te = (TextElement)element;
                        te.text.text = text;
                    }

                    // create new state
                    WSState.AddState(element);
                    // populate new state variables
                    VisualElementState VEState = WSState.elementStates[element];
                    VEState.SetStatePosition(position);
                    VEState.SetStateScale(scale);
                    VEState.SetStateRotation(rotation);
                    VEState.SetStateMaterial(material);
                    if (properties.Count > 5)
                    {

                    }
                }
                index++;
            }
            Instance.timeline.UpdateMarkers();
            Instance._curState = Instance.GetStateAtTime(0);
            Instance.stateIndex = 0;
            Instance._prevState = Instance._curState;
            Instance.UpdateCurrentState(1);
        }

        /// Saves workspace as a json file
        [MenuItem("Tools/Save Workspace")]
        static public void SaveWorkspace()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteStartObject();

                writer.WritePropertyName("Elements");
                writer.WriteStartObject();
                foreach (KeyValuePair<VisualElement, UnityEngine.Object> pair in Instance.allElements)
                {
                    writer.WritePropertyName(pair.Key.gameObject.name);
                    writer.WriteValue(pair.Key.prefabPath);
                }
                writer.WriteEndObject();

                writer.WritePropertyName("States");
                writer.WriteStartArray();
                foreach (WorkspaceState state in Instance.stateHistory_)
                {
                    writer.WriteStartArray();
                    foreach (VisualElementState vizState in state.elementStates.Values)
                    {
                        writer.WriteStartObject();

                        // Name
                        var name = vizState.GetStateElement().name;
                        writer.WritePropertyName("ElementName");
                        writer.WriteValue(name);

                        // Position
                        var position = vizState.GetStatePosition();
                        writer.WritePropertyName("Position");
                        writer.WriteValue($"{position.x} {position.y} {position.z}");

                        // Scale
                        var scale = vizState.GetStateScale();
                        writer.WritePropertyName("Scale");
                        writer.WriteValue($"{scale.x} {scale.y} {scale.z}");

                        // Rotation
                        var rotation = vizState.GetStateRotation();
                        writer.WritePropertyName("Rotation");
                        writer.WriteValue($"{rotation.x} {rotation.y} {rotation.z} {rotation.w}");

                        // Color
                        var color = vizState.GetStateMaterial();
                        writer.WritePropertyName("Material");
                        if (color != null) {
                            writer.WriteValue(color.name.Replace(" (Instance)", "")); // material name to access in resources
                        } else {
                            writer.WriteValue("White");
                        }

                        // TODO: checking for child count to get text elements is total hack
                        if (vizState.GetStateElement().transform.childCount > 0)
                        {
                            // Text element
                            var textElement = (TextElement)vizState.GetStateElement();
                            writer.WritePropertyName("Text");
                            writer.WriteValue(textElement.text.text);
                        }

                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                }
                writer.WriteEndArray();
                writer.WriteEndObject();
            }
            File.WriteAllText("./workspace_save.json", sb.ToString());
        }

        public void OnTimelineChanged(int index)
        {
            Debug.Log(index);
            if (index == stateIndex)
            {
                return;
            }
            this._prevState = this._curState;
            this.startStateFrame = this.stateIndex;
            this.goalStateFrame = index;
            this.stateIndex = stateIndex + (int)Mathf.Sign(index - stateIndex) * 1;
            this._curState = GetStateAtTime(stateIndex);
            this.lerpT = 0;
            // refresh selection, remove any elements that aren't in the current state
            RefreshSelection();
        }

        public void InsertNewElement(VisualElement element, UnityEngine.Object prefab)
        {
            this._curState.AddState(element);
            for (int i = this.stateIndex; i < this.stateHistory_.Count; i++)
            {
                this.stateHistory_[i].AddState(element);
            }
            this.allElements.Add(element, prefab);
        }

        /// Removes element from the current and following frames by updating the workspace states
        public void DeleteElement(VisualElement element, int index = -1)
        {
            index = index == -1 ? this.stateIndex : index;
            for (int i = index; i < this.stateHistory_.Count; i++)
            {
                this.stateHistory_[i].RemoveState(element);
            }
            element.gameObject.transform.localScale = Vector3.zero;
        }

        /// Adds element to collection of selected visual elements.
        /// Then updates parents of elements, to keep selected elements
        /// as child of one parent.
        public void AddSelectedElement(VisualElement element)
        {
            element.SelectEntered();
            this.selectedVisualElements_.Add(element);
            this.UpdateSelectionParentPosition();
            element.transform.SetParent(this.selectionParent);
        }

        /// Remove element from collection of selected visual element.
        /// Then update parents of elements, to keep selected elements
        /// as child of one parent.
        public void RemoveSelectedElement(VisualElement element)
        {
            element.SelectExited();
            this.selectedVisualElements_.Remove(element);
            this.UpdateSelectionParentPosition();
            element.transform.SetParent(this.nonSelectionParent);
        }

        public void DeleteSelectedElements(int index = -1)
        {
            foreach (VisualElement element in selectedVisualElements_)
            {
                DeleteElement(element, index);
            }
            ClearSelectedElements();
        }

        /// Updates the states of all selected elements by calling
        /// element.UpdateState()
        public void UpdateSelectedElementStates()
        {
            foreach (VisualElement element in this.selectedVisualElements_)
            {
                element.UpdateState();
            }
        }

        /// Removes all elements from collection and resets their parents.
        public void ClearSelectedElements()
        {
            foreach (VisualElement element in this.selectedVisualElements_)
            {
                element.SelectExited();
                element.transform.SetParent(this.nonSelectionParent);
            }
            this.selectedVisualElements_ = new HashSet<VisualElement>();
            this.selectionParent.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            this.selectionParent.localScale = Vector3.one;
        }

        public void CopySelectedForward()
        {
            foreach (WorkspaceState state in GetStatesAfterCurrent())
            {
                foreach (VisualElement element in this.selectedVisualElements_)
                {
                    state.AddState(element);
                }
            }
        }

        public void DuplicateSelected()
        {
            HashSet<VisualElement> duplicates = new HashSet<VisualElement>();
            foreach (VisualElement element in this.selectedVisualElements_)
            {
                element.SelectExited();
                element.transform.SetParent(this.nonSelectionParent);
                GameObject duplicate = Instantiate(element.gameObject, element.transform.position + Vector3.one * 0.1f, element.transform.rotation, nonSelectionParent);
                VisualElement duplicateElement = duplicate.GetComponent<VisualElement>();
                duplicates.Add(duplicateElement);
                InsertNewElement(duplicateElement, allElements[element]);
            }
            ClearSelectedElements();
            foreach (VisualElement duplicate in duplicates)
            {
                AddSelectedElement(duplicate);
            }
            UpdateSelectionParentPosition();
        }

        public void SetColorSelected(Material mat)
        {
            foreach (VisualElement element in this.selectedVisualElements_)
            {
                // set the material for state & the gameobject
                VisualElementState state = this._curState.elementStates[element];
                state.SetStateMaterial(mat);
                element.GetComponent<Renderer>().material.CopyPropertiesFromMaterial(mat);
            }
        }

        /// Adds a new state at the provided index. The default index (-1)
        /// will add a new state to the end of the state list.
        public void InsertNewState(int index = -1)
        {
            // Create the new state based on the current workspace conditions.
            WorkspaceState state;
            if (this._curState == null)
            {
                state = new WorkspaceState();
            }
            else
            {
                state = new WorkspaceState(this._curState);
            }
            this._prevState = this._curState;
            this._curState = state;


            if (index < 0)
            {
                // Set to end
                this.stateHistory_.Add(state);
            }
            else
            {
                this.stateHistory_.Insert(index, state);
            }
            // increment slider size
            timeline.setStateCount(this.stateHistory_.Count);

            print("Total States: " + this.stateHistory_.Count);
        }

        public bool DeleteState(int index)
        {
            if (index < 0 || index >= this.stateHistory_.Count)
            {
                return false;
            }
            this.stateHistory_.RemoveAt(index);
            if (index < this.stateIndex)
            {
                this.stateIndex -= 1;
            }
            else if (index == this.stateIndex)
            {
                if (index > 0)
                {
                    this.stateIndex -= 1;
                }
                this._curState = this.stateHistory_[stateIndex];
                this.goalStateFrame = stateIndex;
                this.startStateFrame = stateIndex;
                this.UpdateCurrentState(1);
            }
            return true;
        }

        public WorkspaceState GetStateAtTime(int stateIndex)
        {
            if (this.stateHistory_.Count == 0)
            {
                return null;
            }
            return this.stateHistory_[stateIndex];
        }

        public void RequestNextSlide()
        {
            if (goalStateFrame != startStateFrame || this.stateIndex == this.stateHistory_.Count - 1)
            {
                return;
            }
            int index = this.stateIndex + 1;
            this._prevState = this._curState;
            this.startStateFrame = this.stateIndex;
            this.goalStateFrame = index;
            this.stateIndex = stateIndex + (int)Mathf.Sign(index - stateIndex) * 1;
            this._curState = GetStateAtTime(stateIndex);
            this.lerpT = 0;
            timeline.SetSlider(index);
            
            // refresh selection, remove any elements that aren't in the current state
            try {
                RefreshSelection();
            }
            catch {

            }
        }

        public void ReqeuestPreviousSlide()
        {
            if (goalStateFrame != startStateFrame || this.stateIndex == 0)
            {
                return;
            }
            int index = this.stateIndex - 1;
            this._prevState = this._curState;
            this.startStateFrame = this.stateIndex;
            this.goalStateFrame = index;
            this.stateIndex = stateIndex + (int)Mathf.Sign(index - stateIndex) * 1;
            this._curState = GetStateAtTime(stateIndex);
            this.lerpT = 0;
            timeline.SetSlider(index);
            // refresh selection, remove any elements that aren't in the current state
            try {
                RefreshSelection();
            }
            catch {

            }
        }

        public void UpdateCurrentState(float t)
        {
            // set the transforms
            this._curState.updateAllStates(this._prevState, t);
        }

        public WorkspaceState GetCurrentState()
        {
            return this._curState;
        }

        public List<WorkspaceState> GetStatesAfterCurrent()
        {
            int cnt = this.stateHistory_.Count - this.stateIndex;
            return this.stateHistory_.GetRange(this.stateIndex, cnt);
        }

        public void HideMenuAndTimeline(bool hide)
        {
            if (curMode != WorkspaceModes.Present) {
                menuPanel.gameObject.SetActive(!hide);
                timeline.gameObject.SetActive(!hide);
            }
        }

        public void KeyboardMode(bool active)
        {
            HideMenuAndTimeline(active);
            rayInteractor.GetComponent<XRRayInteractor>().maxRaycastDistance = active ? 0f : 30f;
            rayInteractor.GetComponent<XRInteractorLineVisual>().enabled = !active;
        }

        public void AttachMenu(VisualElement element)
        {
            bool showEdit = element.GetType().Equals(typeof(TextElement));
            radialMenu.Attach(selectionParent, element.transform, showEdit);
        }

        public void DetachMenu()
        {
            radialMenu.Detach();
        }
    }
}