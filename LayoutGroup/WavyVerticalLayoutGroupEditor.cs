#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;

namespace NaderiteCustomScripts
{
    [CustomEditor(typeof(WavyVerticalLayoutGroup))]
    public class WavyVerticalLayoutGroupEditor : HorizontalOrVerticalLayoutGroupEditor
    {
        private SerializedProperty _waveSettings;
        private SerializedProperty _targetPivot;
        private SerializedProperty _targetPivotTransform;
        private SerializedProperty _targetDistance;

        private SerializedProperty _positionOffset;

        //private SerializedProperty _waveOffset;
        private SerializedProperty _snap;
        private SerializedProperty _snapSettings;
        private WavyVerticalLayoutGroup _target;

        protected override void OnEnable()
        {
            base.OnEnable();
            _waveSettings = serializedObject.FindProperty("waveSettings");
            _targetPivot = serializedObject.FindProperty("targetPivot");
            _targetPivotTransform = serializedObject.FindProperty("targetPivotTransform");
            _targetDistance = serializedObject.FindProperty("targetDistance");
            _positionOffset = serializedObject.FindProperty("positionOffset");
            _snap = serializedObject.FindProperty("snap");
            _snapSettings = serializedObject.FindProperty("snapSettings");
            _target = target as WavyVerticalLayoutGroup;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            serializedObject.Update();
            EditorGUILayout.PropertyField(_waveSettings);
            EditorGUILayout.PropertyField(_targetDistance);
            EditorGUILayout.PropertyField(_targetPivot);
            if (_target.PivotType == WaveTargetPivot.Custom)
                EditorGUILayout.PropertyField(_targetPivotTransform);

            EditorGUILayout.PropertyField(_positionOffset);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_snap);
            if (_target.IsSnap)
            {
                EditorGUILayout.PropertyField(_snapSettings);
            }


            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif