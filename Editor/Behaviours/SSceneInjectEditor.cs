using Sapo.DI.Runtime.Behaviours;
using UnityEditor;
using UnityEngine;

namespace Sapo.DI.Editor.Behaviours
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SSceneInject))]
    internal class SSceneInjectEditor : UnityEditor.Editor
    {
        private readonly GUIContent _tempContent = new GUIContent();

        private SerializedProperty _sceneInjector;
        private SerializedProperty _injectOn;
        
        
        private void OnEnable()
        {
            _sceneInjector = serializedObject.FindProperty("sceneInjector");
            _injectOn = serializedObject.FindProperty("injectOn");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This component is used to automatically inject this scene.", MessageType.Info,
                true);

            serializedObject.UpdateIfRequiredOrScript();
            EditorGUILayout.PropertyField(_sceneInjector);
            EditorGUILayout.PropertyField(_injectOn);
            serializedObject.ApplyModifiedProperties();
            
            _tempContent.image ??= EditorGUIUtility.IconContent("console.warnicon.sml").image;
            _tempContent.text = "This gameObject will be destroyed after the injection process.";
            EditorGUILayout.LabelField(GUIContent.none, _tempContent, EditorStyles.helpBox);
        }
    }
}