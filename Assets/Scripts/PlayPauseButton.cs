using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace VirtualDemonstrator {
    public class PlayPauseButton : MonoBehaviour
    {       
        public InteractionModes currentMode = InteractionModes.Create;
        public Timeline timeline;
        public Texture playButtonSprite;
        public Texture pauseButtonSprite;

        Image buttonImage;

        void Awake()
        {
            // This doesn't work, and idk how to fix it
            // buttonImage = GetComponent<Image>();
        }

        public void OnButtonClick() 
        {
            if (this.currentMode == InteractionModes.Create) 
            {
                toggleMode(InteractionModes.Present);
            } 
            else if (this.currentMode == InteractionModes.Present) 
            {
                toggleMode(InteractionModes.Create);
            }            
        }

        void toggleMode(InteractionModes mode) 
        {
            print(mode);

            this.currentMode = mode;
            timeline.toggleMode(mode);

            // if (this.currentMode == InteractionModes.Create) 
            // {
            //     buttonImage.image = playButtonSprite;
            // } 
            // else if (this.currentMode == InteractionModes.Present) 
            // {
            //     buttonImage.image = pauseButtonSprite;
            // }
        }
    }
}