using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace NaderiteCustomScripts
{
    [ExecuteInEditMode]
    public class WavyVerticalLayoutGroup : VerticalLayoutGroup
    {
        [SerializeField] private bool useWave;
        public bool UseWave => useWave;
        [SerializeField] private AnimationCurve waveCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField, Min(1)] private float loopCount;
        [SerializeField] private bool applyWaveToAll;
        public bool ApplyWaveToAll => applyWaveToAll;
        [SerializeField] private int targetDistance;
        [SerializeField] private float intensity = 1;

        [FormerlySerializedAs("offset"), SerializeField]
        private Vector2 positionOffset = Vector2.zero;

        //[SerializeField, Range(0, 1f)] private float waveOffset;
        [SerializeField] private bool isMirror;
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

            if (applyWaveToAll)
            {
                var loopArea = rectChildren.Count / loopCount;
                for (var index = 0; index < rectChildren.Count; index++)
                {
                    var child = rectChildren[index];
                    if (child == null) continue;
                    
                    float curveTime;
                    if (isMirror)
                    {
                        curveTime = Mathf.Lerp(-1, 1, index / loopArea % 1);
                        curveTime = Mathf.Abs(curveTime);
                    }
                    else
                    {
                        curveTime = Mathf.Lerp(0, 1, index / loopArea % 1);
                    }

                    UpdatePositionFor(child, curveTime);
                }
            }
            else
            {
                foreach (var child in rectChildren)
                {
                    if (child == null) continue;
                    
                    var distance = (child.position - _center).y;
                    float curveTime;
                    float distanceLerpFactor = Mathf.Abs(distance / targetDistance);
                    if (isMirror)
                        curveTime = Mathf.Lerp(0, 1, distanceLerpFactor);
                    else
                    {
                        curveTime = distance >= 0
                            ? Mathf.Lerp(0.5f, 1, distanceLerpFactor)
                            : Mathf.Lerp(0.5f, 0f, distanceLerpFactor);
                    }

                    UpdatePositionFor(child, curveTime);
                    if (snap && Mathf.Abs(distance) < Mathf.Abs(_lastNearDistance))
                    {
                        _lastNearDistance = distance;
                        _nearestObject = child;
                    }
                }
            }
        }
        
        void UpdatePositionFor(Transform child, float curveTime)
        {
            var curveAmount = waveCurve.Evaluate(curveTime) * (intensity * 10);
            var newPos = child.position;
            newPos.x += curveAmount + positionOffset.x;
            newPos.y += positionOffset.y;
            child.position = newPos;
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