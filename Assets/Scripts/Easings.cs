using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualDemonstrator {
    public static class Easings {
        public static float linear(float t) {
            return t;
        }
        public static float easeInOut(float t) {
            return t < 0.5 ? 4 * t * t *t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
        }
    }
}