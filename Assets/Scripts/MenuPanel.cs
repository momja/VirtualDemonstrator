using UnityEngine;

namespace VirtualDemonstrator {
    public class MenuPanel : MonoBehaviour { 

        private void Start()
        {
            cubeMaker_.menu_ = this;
            coneMaker_.menu_ = this;
            capsuleMaker_.menu_ = this; 
        }

        public void AddElement(VisualElement element)
        {
            workspace_.InsertNewElement(element);
        }

        public VisualElementInstantiator cubeMaker_;
        public VisualElementInstantiator coneMaker_;
        public VisualElementInstantiator capsuleMaker_;
        public Workspace workspace_;
    }
}