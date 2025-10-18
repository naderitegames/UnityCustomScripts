using System;
using UnityEngine;

namespace NaderiteCustomScripts
{
    [Serializable]
    public class WaveSettings
    {
        public AnimationCurve waveCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public bool isMirror;
        [Min(1)] public float loopCount;
        public float intensity = 1;
    }
}