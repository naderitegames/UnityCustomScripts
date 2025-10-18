using System;
using UnityEngine;
using UnityEngine.UI;

namespace NaderiteCustomScripts
{
    public enum WaveTargetPivot
    {
        Parent,
        Custom
    }

    public enum WaveApplyMode
    {
        Solid,
        ByDistance
    }

    [ExecuteInEditMode]
    public class WavyVerticalLayoutGroup : VerticalLayoutGroup
    {
        public WaveApplyMode WaveApplyMode => waveApplyMode;
        public WaveTargetPivot PivotType => targetPivot;

        public bool IsSnap => snap;

        //[SerializeField, Range(0, 1f)] private float waveOffset;
        [SerializeField] private WaveSettings waveSettings;
        [SerializeField] private Vector2 positionOffset = Vector2.zero;
        [SerializeField] WaveTargetPivot targetPivot = WaveTargetPivot.Parent;
        [SerializeField] private Transform targetPivotTransform;
        [SerializeField] WaveApplyMode waveApplyMode = WaveApplyMode.ByDistance;
        [SerializeField] private int targetDistance;
        [SerializeField] private bool snap;
        [SerializeField] private SnapSettings snapSettings;
        private RectTransform _nearestObject;
        private float _lastNearDistance;
        private Vector3 _center;
        private float _curveTime;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (!snap) return;
            if (snapSettings.HasScrollRect())
                snapSettings.scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (!snap) return;
            if (snapSettings.HasScrollRect())
                snapSettings.scrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if (Application.isPlaying) return;
            ApplyWaveEffect();
        }

        private void OnScrollValueChanged(Vector2 value)
        {
            SetLayoutHorizontal();
            SetLayoutVertical();
        }

        public override void SetLayoutVertical()
        {
            base.SetLayoutVertical();
            ApplyWaveEffect();
        }

        public override void SetLayoutHorizontal()
        {
            base.SetLayoutHorizontal();
            ApplyWaveEffect();
        }

        private void ApplyWaveEffect()
        {
            if (rectChildren.Count == 0) return;
            PrepareForWave();
            switch (WaveApplyMode)
            {
                case WaveApplyMode.Solid:
                    var loopArea = rectChildren.Count / waveSettings.loopCount;
                    for (var index = 0; index < rectChildren.Count; index++)
                    {
                        var child = rectChildren[index];
                        if (child == null) continue;
                        if (waveSettings.isMirror)
                        {
                            _curveTime = Mathf.Lerp(-1, 1, index / loopArea % 1);
                            _curveTime = Mathf.Abs(_curveTime);
                        }
                        else
                            _curveTime = Mathf.Lerp(0, 1, index / loopArea % 1);

                        UpdatePositionFor(child, _curveTime);
                    }

                    break;
                case WaveApplyMode.ByDistance:
                    foreach (var child in rectChildren)
                    {
                        if (child == null) continue;

                        var distance = (child.position - _center).y;

                        var distanceLerpFactor = Mathf.Abs(distance / targetDistance);
                        if (waveSettings.isMirror)
                        {
                            _curveTime = Mathf.Lerp(1, 0, distanceLerpFactor);
                        }
                        else
                        {
                            _curveTime = distance >= 0
                                ? Mathf.Lerp(0.5f, 1, distanceLerpFactor)
                                : Mathf.Lerp(0.5f, 0f, distanceLerpFactor);
                        }


                        UpdatePositionFor(child, _curveTime);
                        if (!snap || !(Mathf.Abs(distance) < Mathf.Abs(_lastNearDistance))) continue;
                        _lastNearDistance = distance;
                        _nearestObject = child;
                    }

                    break;
            }
        }

        void UpdatePositionFor(Transform child, float curveTime)
        {
            var curveAmount = waveSettings.waveCurve.Evaluate(curveTime) * (waveSettings.intensity * 10);
            var newPos = child.position;
            newPos.x += curveAmount + positionOffset.x;
            newPos.y += positionOffset.y;
            child.position = newPos;
        }

        void PrepareForWave()
        {
            _center = GetTargetPivot();
            _lastNearDistance = Mathf.Infinity;
            _nearestObject = null;
        }

        Vector3 GetTargetPivot()
        {
            switch (targetPivot)
            {
                case WaveTargetPivot.Parent when transform.parent:
                    return transform.parent.position;
                case WaveTargetPivot.Parent:
                    Debug.LogWarning("No parents found", this);
                    return transform.position;
                case WaveTargetPivot.Custom when targetPivotTransform:
                    return targetPivotTransform.position;
                default:
                case WaveTargetPivot.Custom:
                    Debug.LogWarning("Target pivot game object is null", this);
                    return transform.position;
            }
        }

        protected override void Update()
        {
            base.Update();
            if (!snap) return;
            if (!snapSettings.HasScrollRect()) return;
            if (!_nearestObject) return;
            if (Mathf.Abs(_lastNearDistance) < snapSettings.distanceForSnapping) return;
            if (Mathf.Abs(snapSettings.scrollRect.velocity.y) > snapSettings.snapSensitivity) return;
            var nearestDistance1 = Vector2.Distance(_nearestObject.position, _center);
            if (nearestDistance1 <= 0.1f) return;
            var speed = Time.deltaTime * Mathf.Lerp(0, 1, nearestDistance1 / snapSettings.distanceForSnapping) *
                        (snapSettings.snappingSpeed * .1f);
            if (_center.y > _nearestObject.position.y)
                snapSettings.scrollRect.normalizedPosition += Vector2.down * speed;
            else if (_center.y < _nearestObject.position.y)
                snapSettings.scrollRect.normalizedPosition -= Vector2.down * speed;
        }
    }
}