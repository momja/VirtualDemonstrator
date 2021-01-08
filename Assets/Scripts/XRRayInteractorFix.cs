using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEngine.XR.Interaction.Toolkit {
    public class XRRayInteractorFix : XRRayInteractor {
        public override void GetValidTargets(List<XRBaseInteractable> validTargets)
        {
            base.GetValidTargets(validTargets);
            float minDistance = float.MaxValue;
            XRBaseInteractable minBaseInteractable = null; 
                        
            // Calculate distance squared to interactor's attach transform and add to validTargets (which is sorted before returning)
            foreach (var interactable in validTargets)
            {
                float dist = interactable.GetDistanceSqrToInteractor(this);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    minBaseInteractable = interactable;
                }
            }
            if (minBaseInteractable != null) {
                validTargets = new List<XRBaseInteractable>();
                validTargets.Add(minBaseInteractable);
            }
        }

        public override bool CanHover(XRBaseInteractable interactable)
        {
            return (validTargets.Count == 0 ||
                    validTargets[0] == interactable &&
                    base.CanHover(interactable));
        }
    }
}