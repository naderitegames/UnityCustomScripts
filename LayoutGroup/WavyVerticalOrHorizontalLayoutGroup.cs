using System;
using UnityEngine;
using UnityEngine.UI;

namespace NaderiteCustomScripts
{
    public abstract class WavyVerticalOrHorizontalLayoutGroup : HorizontalOrVerticalLayoutGroup
    {
        public WaveTargetPivot PivotType => targetPivot;
        public bool IsSnap => snap;
        [SerializeField] private WaveSettings waveSettings;
        [SerializeField] private Vector2 positionOffset = Vector2.zero;
        [SerializeField] WaveTargetPivot targetPivot = WaveTargetPivot.Parent;
        [SerializeField] private Transform targetPivotTransform;
        [SerializeField] private float targetDistance;
        [SerializeField] ScrollRect scrollRect;
        [SerializeField] private bool snap;
        [SerializeField] private SnapSettings snapSettings;
        [SerializeField] private bool affectOutOfDistance = true;
        private RectTransform _nearestObject;
        private float _lastNearDistance;
        private Vector3 _center;
        private float _curveTime;

        private ScrollRect ScrollRect
        {
            get
            {
                if (!scrollRect)
                    Debug.LogWarning("Scroll rect is null. Please set an reference in inspector to use snapping!");
                return scrollRect;
            }
        }

        private void OnScrollValueChanged(Vector2 value)
        {
            SetLayoutHorizontal();
            SetLayoutVertical();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (ScrollRect)
                scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (ScrollRect)
                scrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
        }

        // protected override void OnRectTransformDimensionsChange()
        // {
        //     base.OnRectTransformDimensionsChange();
        //     if (Application.isPlaying) return;
        //     ApplyWaveEffect();
        // }

        protected void ApplyWaveEffect(bool isVertical)
        {
            if (rectChildren.Count == 0) return;
            PrepareForWave();
            // if (!isVertical)
            // {
            //     var temp = _center;
            //     _center.x = temp.y;
            //     _center.y = temp.x;
            // }
            var targetAxis = (isVertical ? _center.y : _center.x);
            float editedCenter = targetAxis - targetDistance * 0.5f;
            foreach (var child in rectChildren)
            {
                if (child == null) continue;
                var targetChildPosAxis = (isVertical ? child.position.y : child.position.x);
                var distance = (targetChildPosAxis - targetAxis);
                var editedDistance = (targetChildPosAxis - editedCenter);
                var loopAmount = ((targetDistance) / waveSettings.LoopCount);
                var distanceLerpFactor = (editedDistance / (targetDistance)) % loopAmount;
                if (editedDistance <= targetDistance && editedDistance >= 0)
                {
                    if (waveSettings.IsMirror)
                    {
                        _curveTime = Mathf.Lerp(waveSettings.LoopCount, -waveSettings.LoopCount, distanceLerpFactor);
                        _curveTime = Mathf.Abs(_curveTime);
                    }
                    else
                    {
                        _curveTime = Mathf.Lerp(waveSettings.LoopCount, 0, distanceLerpFactor);
                    }

                    UpdatePositionFor(child, (_curveTime + waveSettings.WaveOffset) % 1, isVertical);
                }
                else if (affectOutOfDistance)
                {
                    UpdatePositionFor(child, (waveSettings.WaveOffset) % 1, isVertical);
                }

                if (snap && (Mathf.Abs(distance) < Mathf.Abs(_lastNearDistance) + snapSettings.distanceForSnapping))
                {
                    _lastNearDistance = distance;
                    _nearestObject = child;
                }
            }
        }

        private void PrepareForWave()
        {
            _center = GetTargetPivot();
            _lastNearDistance = Mathf.Infinity;
            _nearestObject = null;
        }

        private void UpdatePositionFor(Transform child, float curveTime, bool isVertical)
        {
            var newPos = child.position;
            if (isVertical)
            {
                newPos.x += waveSettings.Evaluate(curveTime) + positionOffset.x;
                newPos.y += positionOffset.y;
            }
            else
            {
                newPos.y += waveSettings.Evaluate(curveTime) + positionOffset.y;
                newPos.x += positionOffset.x;
            }

            child.position = newPos;
        }

        public Vector3 GetTargetPivot()
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

        protected virtual void LateUpdate()
        {
            if (!snap) return;
            if (!scrollRect) return;
            if (!_nearestObject) return;
            if (Mathf.Abs(_lastNearDistance) < snapSettings.distanceForSnapping) return;
            if (Mathf.Abs(scrollRect.velocity.y) > snapSettings.snapSensitivity) return;
            var nearestDistance1 = Vector2.Distance(_nearestObject.position, _center);
            if (nearestDistance1 <= 0.1f) return;
            var speed = Time.deltaTime * Mathf.Lerp(0, 1, nearestDistance1 / snapSettings.distanceForSnapping) *
                        (snapSettings.snappingSpeed * .1f);
            if (_center.y - snapSettings.distanceForSnapping > _nearestObject.position.y)
                scrollRect.normalizedPosition += Vector2.down * speed;
            else if (_center.y + snapSettings.distanceForSnapping < _nearestObject.position.y)
                scrollRect.normalizedPosition -= Vector2.down * speed;
        }
    }

    [Serializable]
    public class SnapSettings
    {
        [Min(0.2f)] public float distanceForSnapping = 5;
        [Min(0.1f)] public float snapSensitivity = 10;
        [Range(0.1f, 2)] public float snappingSpeed = 1;
    }

    public enum WaveTargetPivot
    {
        Parent,
        Custom
    }
}