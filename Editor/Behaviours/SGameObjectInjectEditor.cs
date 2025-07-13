using Overefactor.DI.Runtime.Behaviours;
using UnityEditor;
using UnityEngine;

namespace Overefactor.DI.Editor.Editor.Behaviours
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SGameObjectInject))]
    internal class SGameObjectInjectEditor : UnityEditor.Editor
    {
        private SerializedProperty _createLocalInjector;

        private void OnEnable()
        {
            _createLocalInjector = serializedObject.FindProperty("localInjector");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This component is used to automatically inject this gameObject.", MessageType.Info,
                true);

            serializedObject.UpdateIfRequiredOrScript();
            EditorGUILayout.PropertyField(_createLocalInjector, new GUIContent("Local Injector"));
            serializedObject.ApplyModifiedProperties();

            if (!_createLocalInjector.boolValue) return;

            EditorGUILayout.HelpBox("This will create a local injector for this gameObject. This means " +
                                    "that registration of components attached to this gameObject will only be " +
                                    "available to this gameObject.", MessageType.None, true);

        }
    }
}