using UnityEditor;
using UnityEditor.UI;

namespace NaderiteCustomScripts
{
#if UNITY_EDITOR
    [CustomEditor(typeof(WavyVerticalLayoutGroup))]
    public class WavyVerticalLayoutGroupEditor : HorizontalOrVerticalLayoutGroupEditor
    {
        private SerializedProperty _waveCurve;
        private SerializedProperty _intensity;
        private SerializedProperty _targetDistance;
        private SerializedProperty _offset;
        private SerializedProperty _isMirror;
        private SerializedProperty _snap;
        private SerializedProperty _snapSettings;
        private WavyVerticalLayoutGroup _target;

        protected override void OnEnable()
        {
            base.OnEnable();
            _waveCurve = serializedObject.FindProperty("waveCurve");
            _intensity = serializedObject.FindProperty("intensity");
            _targetDistance = serializedObject.FindProperty("targetDistance");
            _offset = serializedObject.FindProperty("offset");
            _isMirror = serializedObject.FindProperty("isMirror");
            _snap = serializedObject.FindProperty("snap");
            _snapSettings = serializedObject.FindProperty("snapSettings");
            _target = target as WavyVerticalLayoutGroup;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Wave Effect", EditorStyles.boldLabel);

            serializedObject.Update();
            EditorGUILayout.PropertyField(_waveCurve);
            EditorGUILayout.PropertyField(_intensity);
            EditorGUILayout.PropertyField(_targetDistance);
            EditorGUILayout.PropertyField(_offset);
            EditorGUILayout.PropertyField(_isMirror);
            EditorGUILayout.PropertyField(_snap);
            if (_target.IsSnap)
            {
                EditorGUILayout.PropertyField(_snapSettings);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}