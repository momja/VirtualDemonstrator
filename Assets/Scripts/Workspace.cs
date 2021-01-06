using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.EventSystems;

namespace VirtualDemonstrator
{

    public enum InteractionModes {
        Create,
        Present
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
        public HashSet<VisualElement> selVisualElements {
            get {
                return selectedVisualElements_;
            }
        }
        public WorkspaceBounds workspaceBounds;
        public GameObject rayInteractor;
        public Transform selectionParent;
        public Transform nonSelectionParent;
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
            this.selectionParent = GameObject.Find("SelectionParent").transform;
            this.nonSelectionParent = null;
            this.timeline.workspace = this;
            this.menuPanel.workspace_ = this;
            this.workspaceBounds = GameObject.Find("Workspace Bounds").GetComponent<WorkspaceBounds>();
            InsertNewState();
        }

        private void Awake() {
            if (_instance != null && _instance != this) {
                Destroy(this.gameObject);
            } else {
                _instance = this;
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (goalStateFrame != startStateFrame)
            {
                this.lerpT += 0.01f * Mathf.Abs(goalStateFrame - startStateFrame);
                float t = Easings.easeInOut(this.lerpT);
                if (this.lerpT > 1)
                {
                    if (this.stateIndex != goalStateFrame) {
                        this.stateIndex = stateIndex + (int)Mathf.Sign(goalStateFrame-stateIndex)*1;
                        this._prevState = this._curState;
                        this._curState = GetStateAtTime(this.stateIndex);
                        this.lerpT = 0.0f;
                    }
                }
                else if (this._curState != null)
                {
                    this.UpdateCurrentState(t);
                }
            }
        }

        private void UpdateSelectionParentPosition() {
            if (this.selectedVisualElements_.Count == 0) {
                this.selectionParent.position = Vector3.zero;
                this.selectionParent.rotation = Quaternion.identity;
                return;
            }
            foreach(VisualElement selElement in this.selectedVisualElements_) {
                // temporarily reset parent
                selElement.transform.SetParent(null);
            }
            this.selectionParent.localScale = Vector3.one;
            // set parent to the midpoint of the objects
            Vector3 positionMidPt = Vector3.zero;
            Quaternion rotationMidPt = Quaternion.identity;
            int i = 0;
            foreach(VisualElement selElement in this.selectedVisualElements_) {
                positionMidPt += selElement.transform.position;
                if (i == 0) {
                    rotationMidPt = selElement.transform.rotation;
                } else {
                    float weight = 1.0f / (float)(i+1);
                    rotationMidPt = Quaternion.Slerp(rotationMidPt, selElement.transform.rotation, weight);
                }
                i += 1;
            }
            positionMidPt = positionMidPt / this.selectedVisualElements_.Count;
            this.selectionParent.position = positionMidPt;
            this.selectionParent.rotation = rotationMidPt;
            foreach(VisualElement selElement in this.selectedVisualElements_) {
                selElement.transform.SetParent(this.selectionParent);
            }
        }

        private void RefreshSelection() {
            HashSet<VisualElement> newSelection = new HashSet<VisualElement>();
            foreach(VisualElement element in this.selectedVisualElements_) {
                if (this._curState.ElementExists(element)) {
                    newSelection.Add(element);
                }
                else {
                    element.SelectExited();
                    element.transform.SetParent(this.nonSelectionParent);
                }
            }
            this.UpdateSelectionParentPosition();
            this.selectedVisualElements_ = newSelection;
        }

        public void OnTimelineChanged(int index)
        {
            Debug.Log(index);
            if (index == stateIndex) {
                return;
            }
            this._prevState = this._curState;
            this.startStateFrame = this.stateIndex;
            this.goalStateFrame = index;
            this.stateIndex = stateIndex + (int)Mathf.Sign(index-stateIndex)*1;
            this._curState = GetStateAtTime(stateIndex);
            this.lerpT = 0;
            // refresh selection, remove any elements that arent in the current state
            RefreshSelection();
        }

        public void InsertNewElement(VisualElement element) {
            this._curState.AddState(element);
            for (int i = this.stateIndex; i < this.stateHistory_.Count; i++) {
                this.stateHistory_[i].AddState(element);
            }
        }

        /// Removes element from the current and following frames by updating the workspace states
        public void DeleteElement(VisualElement element, int index=-1) {
            index = index == -1 ? this.stateIndex : index;
            for (int i = index; i < this.stateHistory_.Count; i++) {
                this.stateHistory_[i].RemoveState(element);
            }
            element.gameObject.transform.localScale = Vector3.zero;
        }

        /// Adds element to collection of selected visual elements.
        /// Then updates parents of elements, to keep selected elements
        /// as child of one parent.
        public void AddSelectedElement(VisualElement element) {
            element.SelectEntered();
            this.selectedVisualElements_.Add(element);
            this.UpdateSelectionParentPosition();
            element.transform.SetParent(this.selectionParent);
        }
        
        /// Remove element from collection of selected visual element.
        /// Then update parents of elements, to keep selected elements
        /// as child of one parent.
        public void RemoveSelectedElement(VisualElement element) {
            element.SelectExited();
            this.selectedVisualElements_.Remove(element);
            this.UpdateSelectionParentPosition();
            element.transform.SetParent(this.nonSelectionParent);
        }

        public void DeleteSelectedElements(int index=-1) {
            foreach(VisualElement element in selectedVisualElements_) {
                DeleteElement(element, index);
            }
            ClearSelectedElements();
        }

        /// Updates the states of all selected elements by calling
        /// element.UpdateState()
        public void UpdateSelectedElementStates() {
            foreach(VisualElement element in this.selectedVisualElements_) {
                element.UpdateState();
            }
        }

        /// Removes all elements from collection and resets their parents.
        public void ClearSelectedElements() {
            foreach(VisualElement element in this.selectedVisualElements_) {
                element.SelectExited();
                element.transform.SetParent(this.nonSelectionParent);
            }
            this.selectedVisualElements_ = new HashSet<VisualElement>();
            this.selectionParent.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            this.selectionParent.localScale = Vector3.one;
        }

        public void CopySelectedForward() {
            foreach(WorkspaceState state in GetStatesAfterCurrent()) {
                foreach(VisualElement element in this.selectedVisualElements_) {
                    state.AddState(element);
                }
            }
        }

        public void DuplicateSelected() {
            HashSet<VisualElement> duplicates = new HashSet<VisualElement>();
            foreach(VisualElement element in this.selectedVisualElements_) {
                element.SelectExited();
                element.transform.SetParent(this.nonSelectionParent);
                GameObject duplicate = Instantiate(element.gameObject, element.transform.position + Vector3.one*0.1f, element.transform.rotation, nonSelectionParent);
                VisualElement duplicateElement = duplicate.GetComponent<VisualElement>();
                duplicates.Add(duplicateElement);
                InsertNewElement(duplicateElement);
            }
            ClearSelectedElements();
            foreach(VisualElement duplicate in duplicates) {
                AddSelectedElement(duplicate);
            }
            UpdateSelectionParentPosition();
        }

        /// Adds a new state at the provided index. The default index (-1)
        /// will add a new state to the end of the state list.
        public void InsertNewState(int index = -1)
        {
            // Create the new state based on the current workspace conditions.
            WorkspaceState state;
            if (this._curState == null) {
                state = new WorkspaceState();
            }
            else {
                state = new WorkspaceState(this._curState);
            }
            this._prevState = this._curState;
            this._curState = state;
            
            // increment slider size
            timeline.setStateCount(this.stateHistory_.Count);

            if (index < 0)
            {
                // Set to end
                this.stateHistory_.Add(state);
            }
            else
            {
                this.stateHistory_.Insert(index, state);
            }
            print("Total States: " + this.stateHistory_.Count);
        }

        public bool DeleteState(WorkspaceState state)
        {
            bool result = this.stateHistory_.Remove(state);
            if (result)
            {
                this.timeline.stateCount -= 1;
            }
            return result;
        }

        public bool DeleteState(int index)
        {
            if (index < 0 || index >= this.stateHistory_.Count)
            {
                return false;
            }
            this.stateHistory_.RemoveAt(index);
            return true;
        }

        public WorkspaceState GetStateAtTime(int stateIndex)
        {
            if (this.stateHistory_.Count == 0) {
                return null;
            }
            return this.stateHistory_[stateIndex];
        }

        public void UpdateCurrentState(float t) {
            // set the transforms
            this._curState.updateAllStates(this._prevState, t);
        }

        public void toggleMode(InteractionModes mode) {
            // Handle change in mode
            print("toggling");
        }

        public WorkspaceState GetCurrentState()
        {
            return this._curState;
        }

        public List<WorkspaceState> GetStatesAfterCurrent() {
            int cnt = this.stateHistory_.Count - this.stateIndex;
            return this.stateHistory_.GetRange(this.stateIndex, cnt);
        }

        public void HideMenuAndTimeline(bool hide) {
            menuPanel.gameObject.SetActive(!hide);
            timeline.gameObject.SetActive(!hide);
        }

        public void KeyboardMode(bool active) {
            HideMenuAndTimeline(active);
            rayInteractor.GetComponent<XRRayInteractor>().maxRaycastDistance = active ? 0f : 30f;
            rayInteractor.GetComponent<XRInteractorLineVisual>().enabled = !active;
        }

        public void AttachMenu(VisualElement element) {
            radialMenu.Attach(selectionParent);
        }

        public void DetachMenu() {
            radialMenu.Detach();
        }
    }
}