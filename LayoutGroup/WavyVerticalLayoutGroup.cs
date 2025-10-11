using UnityEngine;
using UnityEngine.UI;

namespace NaderiteCustomScripts
{
    [ExecuteInEditMode]
    public class WavyVerticalLayoutGroup : VerticalLayoutGroup
    {
        [SerializeField] private AnimationCurve waveCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private float intensity = 1;
        [SerializeField] private Vector2 offset;
        [SerializeField] private int targetDistance;
        public bool IsSnap => snap;
        [SerializeField] private bool snap;
        [SerializeField] private SnapSettings snapSettings;
        private RectTransform _nearestObject;
        private float _lastNearDistance;
        private Vector3 _center;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (!snap) return;
            if (snapSettings.scrollRect)
                snapSettings.scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (!snap) return;
            if (snapSettings.scrollRect)
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
                float curveTime;
                float distanceLerpFactor = Mathf.Abs(distance / targetDistance);
                if (snapSettings.isMirror)
                    curveTime = Mathf.Lerp(0, 1, distanceLerpFactor);
                else
                {
                    curveTime = distance >= 0
                        ? Mathf.Lerp(0.5f, 1, distanceLerpFactor)
                        : Mathf.Lerp(0.5f, 0f, distanceLerpFactor);
                }

                var curveAmount = waveCurve.Evaluate(curveTime) * (intensity * 10);
                var newPos = child.position;
                newPos.x += curveAmount + offset.x;
                newPos.y += offset.y;
                child.position = newPos;
                if (snap && Mathf.Abs(distance) < Mathf.Abs(_lastNearDistance))
                {
                    _lastNearDistance = distance;
                    _nearestObject = child;
                }
            }
        }

        void PrepareForWave()
        {
            _center = transform.parent ? transform.parent.position : transform.position;
            _lastNearDistance = Mathf.Infinity;
            _nearestObject = null;
        }

        protected override void Update()
        {
            base.Update();
            if (!snap) return;
            if (!snapSettings.scrollRect) return;
            if (!_nearestObject) return;
            if (Mathf.Abs(_lastNearDistance) < snapSettings.distanceForSnapping) return;
            if (Mathf.Abs(snapSettings.scrollRect.velocity.y) > 10f) return;
            var nearestDistance1 = Vector2.Distance(_nearestObject.position, _center);
            if (nearestDistance1 <= 0.1f) return;
            var speed = (Time.deltaTime * Mathf.Lerp(0, 1, nearestDistance1 / snapSettings.distanceForSnapping)) *
                        (snapSettings.snappingSpeed * .1f);
            if (_center.y > _nearestObject.position.y)
                snapSettings.scrollRect.normalizedPosition += Vector2.down * speed;
            else if (_center.y < _nearestObject.position.y)
            {
                snapSettings.scrollRect.normalizedPosition -= Vector2.down * speed;
            }
        }

        Vector2 GetCenterPointInCurve()
        {
            Vector2 center = transform.parent ? transform.parent.position : transform.position;
            var c = center + offset;
            c.x += waveCurve.Evaluate(snapSettings.isMirror ? 0 : 0.5f) * (intensity * 10);

            return c;
        }
    }
}