using System;
using UnityEngine;
using UnityEngine.UI;

namespace NaderiteCustomScripts
{
    [Serializable]
    public class SnapSettings
    {
        public ScrollRect scrollRect;
        public bool isMirror;
        public float distanceForSnapping = 100;
        [Range(0.1f, 3)] public float snappingSpeed = 1;
    }
}