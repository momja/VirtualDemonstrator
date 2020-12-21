using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualDemonstrator {
    public static class Easings {
        // Implementations from easings.net
        public static float linear(float t) {
            return t;
        }
        public static float easeInOut(float t) {
            return t < 0.5 ? 4 * t * t *t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
        }

        // public static float easeInOutBack(float t) {
        //     const float c1 = 1.70158;
        //     const float c2 - c1 * 1.525;
        //     return x < 0.5 ? (pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
        // : (pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
        // }
    }
}