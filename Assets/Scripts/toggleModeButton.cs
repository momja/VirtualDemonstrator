using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualDemonstrator {
    public class toggleModeButton : MonoBehaviour
    {       
        public Workspace workspace;
        public InteractionModes curMode = InteractionModes.Create;
        public Material playModeMat;
        public Material pauseModeMat;
        public Timeline delegator;
        // Start is called before the first frame update
        private void Start() {

        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.layer == 8) {
                print("Test");
                if (this.curMode == InteractionModes.Create) {
                    toggleMode(InteractionModes.Present);
                } else if (this.curMode == InteractionModes.Present) {
                    toggleMode(InteractionModes.Create);
                }
            }
        }

        void toggleMode(InteractionModes mode) {
            this.curMode = mode;
            delegator.toggleMode(mode);

            if (this.curMode == InteractionModes.Create) {
                this.gameObject.GetComponent<Renderer>().material = playModeMat;
            } else if (this.curMode == InteractionModes.Present) {
                this.gameObject.GetComponent<Renderer>().material = pauseModeMat;
            }
        }
    }
}