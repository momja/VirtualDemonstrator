using UnityEngine;

namespace VirtualDemonstrator {
    public class MenuPanel : MonoBehaviour { 

        private void Start()
        {
            cubeMaker_.menu_ = this;
        }

        public void AddElement(VisualElement element, UnityEngine.Object prefab)
        {
            workspace_.InsertNewElement(element, prefab);
        }

        public VisualElementInstantiator cubeMaker_;
        public Workspace workspace_;
    }
}