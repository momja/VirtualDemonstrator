﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualDemonstrator
{
    /// <summary>
    /// Main Driver class for the virtual demonstrator.
    /// Choreographs interactions between various components.
    /// </summary>
    public class Workspace : MonoBehaviour
    {
        public MenuPanel menuPanel;
        public Timeline timeline;
        private List<WorkspaceState> _stateHistory;
        private List<VisualElement> _visualElements;

        // Start is called before the first frame update
        private void Start()
        {
            this._stateHistory = new List<WorkspaceState>();
            this._visualElements = new List<VisualElement>();
        }

        // Update is called once per frame
        private void Update()
        {

        }

        public void insertNewState(WorkspaceState state, int index = -1)
        {
            if (index < 0)
            {
                // Set to end
                this._stateHistory.Add(state);
            }
            else
            {
                this._stateHistory.Insert(index, state);
            }
            this.timeline.stateCount += 1;
        }

        public bool deleteState(WorkspaceState state)
        {
            bool result = this._stateHistory.Remove(state);
            if (result)
            {
                this.timeline.stateCount -= 1;
            }
            return result;
        }

        public bool deleteState(int index)
        {
            if (index < 0 || index >= this._stateHistory.Count)
            {
                return false;
            }
            this._stateHistory.RemoveAt(index);
            return true;
        }

        public WorkspaceState getStateAtTime(float time)
        {
            int timeIndex = this.timeline.getTimeIndex(time);
            return this._stateHistory[timeIndex];
        }

    }
}