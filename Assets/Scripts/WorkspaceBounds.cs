using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualDemonstrator
{

    public class WorkspaceBounds : MonoBehaviour
    {
        public Vector3 bounds = new Vector3(1f, 1f, 1f);
        public GameObject boundWalls;

        public void HideWalls(bool hide)
        {
            boundWalls.SetActive(!hide);
        }

        public bool InBounds(Transform t)
        {
            return !(
                Mathf.Abs(t.position.x) > bounds.x ||
                t.position.y > bounds.y * 2 ||
                t.position.y < 0 ||
                Mathf.Abs(t.position.z) > bounds.z
            );
        }
        private void Start()
        {
            this.transform.localScale = bounds;
        }
    }
}
