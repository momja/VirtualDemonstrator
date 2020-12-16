using System.Collections;
using System.Collections.Generic;

namespace VirtualDemonstrator {
    public class WorkspaceState {
        public List<VisualElementState> elementStates;

        public WorkspaceState() {
            this.elementStates = new List<VisualElementState>();
        }

        public void updateAllStates(float t) {
            // t is used as the interpolation parameter.
            foreach(VisualElementState vizState in this.elementStates) {
                // set new transform for visual element
                vizState.setVizElementToState(t);
            }
        }
    }
}