using System;
using UnityEditor;
using UnityEngine;

namespace NaderiteCustomScripts
{
    [Serializable]
    public class WaveSettings
    {
        [SerializeField] private AnimationCurve waveCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [Min(1)] [SerializeField] float loopCount;
        [Min(1)] public float LoopCount => loopCount;
        [SerializeField] private float intensity = 1;
        public bool IsMirror => isMirror;
        [SerializeField] bool isMirror;
        public float WaveOffset => waveOffset;
        [SerializeField, Range(0, 1)] private float waveOffset;

        public float Evaluate(float t)
        {
            return waveCurve.Evaluate(t) * (intensity * 10);
        }
    }
}