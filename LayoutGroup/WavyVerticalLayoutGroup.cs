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

    [ExecuteInEditMode]
    public class WavyVerticalLayoutGroup : VerticalLayoutGroup
    {
        public WaveTargetPivot PivotType => targetPivot;
        public bool IsSnap => snap;
        [SerializeField] private WaveSettings waveSettings;
        [SerializeField] private Vector2 positionOffset = Vector2.zero;
        [SerializeField] WaveTargetPivot targetPivot = WaveTargetPivot.Parent;
        [SerializeField] private Transform targetPivotTransform;
        [SerializeField] private float targetDistance;
        [SerializeField] private bool snap;
        [SerializeField] private SnapSettings snapSettings;
        [SerializeField] private bool effectOutOfDistance = true;
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
            foreach (var child in rectChildren)
            {
                if (child == null) continue;

                var distance = (child.position - _center).y;
                var loopAmount = ((targetDistance) / waveSettings.LoopCount);
                var distanceLerpFactor = (distance / (targetDistance)) % loopAmount;
                if (distance <= targetDistance && distance >= 0)
                {
                    if (waveSettings.IsMirror)
                    {
                        _curveTime = Mathf.Lerp(-waveSettings.LoopCount, waveSettings.LoopCount, distanceLerpFactor);
                        _curveTime = Mathf.Abs(_curveTime);
                    }
                    else
                    {
                        _curveTime = Mathf.Lerp(waveSettings.LoopCount, 0, distanceLerpFactor);
                    }

                    UpdatePositionFor(child, (_curveTime + waveSettings.WaveOffset) % 1);
                }
                else if (effectOutOfDistance)
                {
                    UpdatePositionFor(child, (waveSettings.WaveOffset) % 1);
                }


                if (!snap || !(Mathf.Abs(distance) < Mathf.Abs(_lastNearDistance))) continue;
                _lastNearDistance = distance;
                _nearestObject = child;
            }
        }

        private void UpdatePositionFor(Transform child, float curveTime)
        {
            var newPos = child.position;
            newPos.x += waveSettings.Evaluate(curveTime) + positionOffset.x;
            newPos.y += positionOffset.y;
            child.position = newPos;
        }

        private void PrepareForWave()
        {
            _center = GetTargetPivot();
            _center.y -= targetDistance * 0.5f;
            _lastNearDistance = Mathf.Infinity;
            _nearestObject = null;
        }

        private Vector3 GetTargetPivot()
        {
            Vector3 center;
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

        void LateUpdate()
        {
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