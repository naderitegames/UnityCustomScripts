using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace NaderiteCustomScripts
{
    [ExecuteInEditMode]
    public class CustomWaveVerticalLayoutGroup : VerticalLayoutGroup
    {
        [SerializeField] private AnimationCurve waveCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private float intensity = 1;
        [SerializeField] private Vector2 offset;
        [SerializeField] private int targetDistance;
        [SerializeField] private bool snap;
        [SerializeField] private float distanceForSnapping = 100;
        [SerializeField, Range(0.1f, 3)] private float snappingSpeed = 1;
        [SerializeField] private bool isMirror;
        [SerializeField] private Image centerTest;
        [SerializeField] private ScrollRect scrollRect;

        public bool IsSnap => snap;

        private Vector3 _center;
        private float nearestDistance;
        private float lastScrollValue = -1f;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (scrollRect != null)
            {
                scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (scrollRect != null)
            {
                scrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
            }
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

        private RectTransform nearestObject;
        private float lastNearDistance;

        private void ApplyWaveEffect()
        {
            if (rectChildren.Count == 0) return;
            _center = transform.parent ? transform.parent.position : transform.position;
            if (centerTest) centerTest.transform.position = _center;
            lastNearDistance = Mathf.Infinity;
            nearestObject = null;
            foreach (var child in rectChildren)
            {
                if (child == null) continue;
                var distance = (child.position - _center).y;
                float curveTime;
                if (isMirror)
                {
                    curveTime = Mathf.Lerp(0, 1, Mathf.Abs((distance / targetDistance)));
                }
                else
                {
                    curveTime = distance >= 0
                        ? Mathf.Lerp(0.5f, 1, Mathf.Abs(distance / targetDistance))
                        : Mathf.Lerp(0.5f, 0f, Mathf.Abs(distance / targetDistance));
                }

                var curveAmount = waveCurve.Evaluate(curveTime) * (intensity * 10);
                var newPos = child.position;
                newPos.x += curveAmount + offset.x;
                newPos.y += offset.y;
                child.position = newPos;
                if (snap && Mathf.Abs(distance) < Mathf.Abs(lastNearDistance))
                {
                    lastNearDistance = distance;
                    nearestObject = child;
                }
            }
        }

        protected override void Update()
        {
            base.Update();
            if (!snap) return;
            if (!scrollRect) return;
            if (Mathf.Abs(lastNearDistance) < distanceForSnapping) return;
            if (!nearestObject) return;
            if (Mathf.Abs(scrollRect.velocity.y) > 10f) return;
            var nearestDistance1 = Vector2.Distance(nearestObject.position, _center);
            if (nearestDistance1 <= 0.1f) return;
            var speed = (Time.deltaTime * Mathf.Lerp(0, 1, nearestDistance1 / distanceForSnapping)) * (snappingSpeed * .1f);
            if (_center.y > nearestObject.position.y)
                scrollRect.normalizedPosition += Vector2.down * speed;
            else if (_center.y < nearestObject.position.y)
            {
                scrollRect.normalizedPosition -= Vector2.down * speed;
            }
        }

        Vector2 GetCenterPointInCurve()
        {
            Vector2 center = transform.parent ? transform.parent.position : transform.position;
            var c = center + offset;
            c.x += waveCurve.Evaluate(isMirror ? 0 : 0.5f) * (intensity * 10);

            return c;
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if (Application.isPlaying) return;
            ApplyWaveEffect();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CustomWaveVerticalLayoutGroup))]
    public class CustomWaveVerticalLayoutGroupSimpleEditor : HorizontalOrVerticalLayoutGroupEditor
    {
        private SerializedProperty waveCurve;
        private SerializedProperty intensity;
        private SerializedProperty targetDistance;
        private SerializedProperty centerTest;
        private SerializedProperty offset;
        private SerializedProperty isMirror;
        private SerializedProperty snap;
        private SerializedProperty scrollRect;
        private SerializedProperty distanceForSnapping;
        private SerializedProperty snappingSpeed;
        private CustomWaveVerticalLayoutGroup _target;

        protected override void OnEnable()
        {
            base.OnEnable();
            waveCurve = serializedObject.FindProperty("waveCurve");
            intensity = serializedObject.FindProperty("intensity");
            targetDistance = serializedObject.FindProperty("targetDistance");
            centerTest = serializedObject.FindProperty("centerTest");
            offset = serializedObject.FindProperty("offset");
            isMirror = serializedObject.FindProperty("isMirror");
            snap = serializedObject.FindProperty("snap");
            scrollRect = serializedObject.FindProperty("scrollRect");
            distanceForSnapping = serializedObject.FindProperty("distanceForSnapping");
            snappingSpeed = serializedObject.FindProperty("snappingSpeed");
            _target = target as CustomWaveVerticalLayoutGroup;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Wave Effect", EditorStyles.boldLabel);

            serializedObject.Update();
            EditorGUILayout.PropertyField(waveCurve);
            EditorGUILayout.PropertyField(intensity);
            EditorGUILayout.PropertyField(targetDistance);
            EditorGUILayout.PropertyField(centerTest);
            EditorGUILayout.PropertyField(offset);
            EditorGUILayout.PropertyField(isMirror);
            EditorGUILayout.PropertyField(snap);
            if (_target.IsSnap)
            {
                EditorGUILayout.PropertyField(scrollRect);
                EditorGUILayout.PropertyField(distanceForSnapping);
                EditorGUILayout.PropertyField(snappingSpeed);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}