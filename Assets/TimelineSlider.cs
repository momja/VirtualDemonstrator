using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace VirtualDemonstrator {
    public class TimelineSlider : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        public Timeline timelineDelegate;
        public void OnPointerDown(PointerEventData eventData) {
            Debug.Log("Pointer Down on Timeline");
            timelineDelegate.pointerUp();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("Pointer Up on Timeline");
            timelineDelegate.pointerUp();
        }
    }

}