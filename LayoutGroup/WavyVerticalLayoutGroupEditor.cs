using UnityEditor;
using UnityEditor.UI;

namespace NaderiteCustomScripts
{
#if UNITY_EDITOR
    [CustomEditor(typeof(WavyVerticalLayoutGroup))]
    public class WavyVerticalLayoutGroupEditor : HorizontalOrVerticalLayoutGroupEditor
    {
        private SerializedProperty _useWave;
        private SerializedProperty _waveCurve;
        private SerializedProperty _loopCount;
        private SerializedProperty _applyWaveToAll;
        private SerializedProperty _targetDistance;
        private SerializedProperty _intensity;

        private SerializedProperty _positionOffset;

        //private SerializedProperty _waveOffset;
        private SerializedProperty _isMirror;
        private SerializedProperty _snap;
        private SerializedProperty _snapSettings;
        private WavyVerticalLayoutGroup _target;

        protected override void OnEnable()
        {
            base.OnEnable();
            _useWave = serializedObject.FindProperty("useWave");
            _waveCurve = serializedObject.FindProperty("waveCurve");
            _loopCount = serializedObject.FindProperty("loopCount");
            _applyWaveToAll = serializedObject.FindProperty("applyWaveToAll");
            _targetDistance = serializedObject.FindProperty("targetDistance");
            _intensity = serializedObject.FindProperty("intensity");
            _positionOffset = serializedObject.FindProperty("positionOffset");
            //_waveOffset = serializedObject.FindProperty("waveOffset");
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
            EditorGUILayout.PropertyField(_useWave);
            if (_target.UseWave)
            {
                EditorGUILayout.PropertyField(_waveCurve);
                EditorGUILayout.PropertyField(_intensity);
                EditorGUILayout.PropertyField(_positionOffset);
                //EditorGUILayout.PropertyField(_waveOffset);
                EditorGUILayout.PropertyField(_isMirror);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(_applyWaveToAll);
                if (!_target.ApplyWaveToAll)
                    EditorGUILayout.PropertyField(_targetDistance);
                EditorGUILayout.PropertyField(_loopCount);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(_snap);
                if (_target.IsSnap)
                {
                    EditorGUILayout.PropertyField(_snapSettings);
                }
            }


            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}