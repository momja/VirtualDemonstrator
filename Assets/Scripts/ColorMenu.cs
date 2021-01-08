using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualDemonstrator {
    public class ColorMenu : RadialMenu {
        public List<MenuOptionInfo> menuOptions;

        protected override void Start() {
            this.SetMenuOptions(menuOptions);
            base.Start();
        }
    }
}
