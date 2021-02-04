//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//
//namespace VirtualDemonstrator {
//    public class PlayPauseButton : MonoBehaviour
//    {       
//        public InteractionModes currentMode = InteractionModes.Create;
//        public Timeline timeline;
//        public Sprite playButtonSprite;
//        public Sprite pauseButtonSprite;
//
//        Image buttonImage;
//
//        void Awake()
//        {
//            buttonImage = GetComponent<Image>();
//        }
//
//        public void OnButtonClick() 
//        {
//            if (this.currentMode == InteractionModes.Create) 
//            {
//                toggleMode(InteractionModes.Present);
//            } 
//            else if (this.currentMode == InteractionModes.Present) 
//            {
//                toggleMode(InteractionModes.Create);
//            }            
//        }
//
//        void toggleMode(InteractionModes mode) 
//        {
//            print(mode);
//
//            this.currentMode = mode;
//            timeline.toggleMode(mode);
//
//            if (this.currentMode == InteractionModes.Create) 
//            {
//                buttonImage.sprite = playButtonSprite;
//            } 
//            else if (this.currentMode == InteractionModes.Present) 
//            {
//                buttonImage.sprite = pauseButtonSprite;
//            }
//        }
//    }
//}