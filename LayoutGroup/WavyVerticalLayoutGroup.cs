using System;
using UnityEngine;
using UnityEngine.UI;

namespace NaderiteCustomScripts
{
    [AddComponentMenu("Layout/Wavy Vertical Layout Group")]
    public class WavyVerticalLayoutGroup : WavyVerticalOrHorizontalLayoutGroup
    {
        /// <summary>
        /// Called by the layout system. Also see ILayoutElement
        /// </summary>
        public override void CalculateLayoutInputVertical()
        {
            CalcAlongAxis(1, true);
            ApplyWaveEffect(true);
        }

        /// <summary>
        /// Called by the layout system. Also see ILayoutElement
        /// </summary>
        public override void SetLayoutHorizontal()
        {
            SetChildrenAlongAxis(0, true);
            ApplyWaveEffect(true);
        }

        /// <summary>
        /// Called by the layout system. Also see ILayoutElement
        /// </summary>
        public override void SetLayoutVertical()
        {
            SetChildrenAlongAxis(1, true);
            ApplyWaveEffect(true);
        }
    }
}