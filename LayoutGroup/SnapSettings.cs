using System;
using UnityEngine;
using UnityEngine.UI;

namespace NaderiteCustomScripts
{
    [Serializable]
    public class SnapSettings
    {
        public ScrollRect scrollRect;
        [Min(0.1f)] public float distanceForSnapping = 100;
        [Min(0.1f)] public float snapSensitivity = 10;
        [Range(0.1f, 3)] public float snappingSpeed = 1;

        public bool HasScrollRect()
        {
            if (scrollRect)
            {
                return true;
            }

            Debug.LogWarning("Scroll rect is null. Please set an reference in inspector to use snapping!");
            return false;
        }
    }
}