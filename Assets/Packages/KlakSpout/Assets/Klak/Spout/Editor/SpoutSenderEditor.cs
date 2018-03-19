// KlakSpout - Spout realtime video sharing plugin for Unity
// https://github.com/keijiro/KlakSpout
using UnityEngine;
using UnityEditor;

namespace Klak.Spout
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SpoutSender))]
    public class SpoutSenderEditor : Editor
    {
        SerializedProperty _resolution;
        SerializedProperty _clearAlpha;
        SerializedProperty _sendOnly;

        void OnEnable()
        {
            _resolution = serializedObject.FindProperty("_resolution");
            _clearAlpha = serializedObject.FindProperty("_clearAlpha");
            _sendOnly = serializedObject.FindProperty("_sendOnly");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_resolution);
            EditorGUILayout.PropertyField(_clearAlpha);
            EditorGUILayout.PropertyField(_sendOnly);

            serializedObject.ApplyModifiedProperties();
        }
    }
}