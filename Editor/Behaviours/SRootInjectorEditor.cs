using Overefactor.DI.Editor.Editor.Common;
using Overefactor.DI.Runtime.Behaviours;
using UnityEditor;
using UnityEngine;

namespace Overefactor.DI.Editor.Editor.Behaviours
{
    [CustomEditor(typeof(SRootInjector))]
    internal class SRootInjectorEditor : UnityEditor.Editor
    {
        private SRootInjector _injector;
        private SerializedProperty _makePersistent;
        private RegistrableAssetsList _assetsToRegister;

        private readonly PrefsBool _settingsFoldout = new("Sapo.DI.Editor.Behaviours.SRootInjectorEditor._settingsFoldout");
        private readonly PrefsBool _runtimeInfoFoldout = new("Sapo.DI.Editor.Behaviours.SRootInjectorEditor._runtimeInfoFoldout");
        private InjectorRuntimeInfo _runtimeInfo;

        private void OnEnable()
        {
            _injector = (SRootInjector)target;

            _makePersistent = serializedObject.FindProperty("makePersistent");
            _assetsToRegister = new RegistrableAssetsList(serializedObject.FindProperty("assetsToRegister"));
            
            _settingsFoldout.Load();
            _runtimeInfoFoldout.Load();
        }
        


        public override bool RequiresConstantRepaint() => true;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(
                "This component is a root injector that will be used to register and inject dependencies.",
                MessageType.Info, true);
            
            serializedObject.UpdateIfRequiredOrScript();

            DrawSettingsGUI();
            DrawRuntimeGUI();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSettingsGUI()
        {
            _settingsFoldout.Value = EditorGUILayout.Foldout(_settingsFoldout.Value, "Settings", true);
            if (!_settingsFoldout.Value) return;

            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(_makePersistent);
            _assetsToRegister.OnGUI();
            
            EditorGUI.indentLevel--;

        }

        private void DrawRuntimeGUI()
        {
            var isPlaying = Application.IsPlaying(_injector);

            if (!isPlaying)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Foldout(false, "Runtime info", true);
                EditorGUI.EndDisabledGroup();
                return;
            }
            
            
            _runtimeInfoFoldout.Value = EditorGUILayout.Foldout(_runtimeInfoFoldout.Value, "Runtime info", true);
            if (!_runtimeInfoFoldout.Value) return;
            
            _runtimeInfo ??= new InjectorRuntimeInfo(_injector.Injector);
            
            _runtimeInfo.OnGUI();
        }
    }
}