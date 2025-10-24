using System;
using UnityEngine;

namespace NaderiteCustomScripts
{
    public class GetTargetPivotTest : MonoBehaviour
    {
        [SerializeField] private WavyVerticalLayoutGroup targetWVLG;

        private void Update()
        {
            transform.position = targetWVLG.GetTargetPivot();
        }
    }
}