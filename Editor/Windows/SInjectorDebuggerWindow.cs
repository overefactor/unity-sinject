using System.Collections.Generic;
using Overefactor.DI.Editor.Editor.Common;
using Overefactor.DI.Runtime.Behaviours;
using Overefactor.DI.Runtime.Common;
using Overefactor.DI.Runtime.Core;
using Overefactor.DI.Runtime.Interfaces;
using UnityEditor;
using UnityEngine;

namespace Overefactor.DI.Editor.Editor.Windows
{
    public class SInjectorDebuggerWindow : EditorWindow
    {
        [MenuItem("Window/Sapo/DI/Context Debugger")]
        public static void ShowWindow()
        {
            var window = GetWindow<SInjectorDebuggerWindow>();
            window.titleContent = new GUIContent("SInject Context Debugger");
            window.Show();
        }

        private SRootInjector _injector;
        private List<InjectorInfo> _injectors = new();
        private int _selectedInjector = 0;
        private bool _dirty;
        private Vector2 _scrollPosition;

        private class InjectorInfo
        {
            public string Name { get; }
            public ISInjector Injector { get; }

            public InjectorRuntimeInfo Gui;

            public InjectorInfo(string name, ISInjector injector)
            {
                Name = name;
                Injector = injector;
                Gui = new InjectorRuntimeInfo((SInjector)injector);
            }
        }

        private void OnEnable()
        {
            Selection.selectionChanged += RefreshContext;
            _dirty = true;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= RefreshContext;
        }
        
        
        private void RefreshContext()
        {
            _dirty = false;
            
            if (!_injector) return;
            
            _injectors = LoadInjectorsFromContext();
            _selectedInjector = _injectors.Count - 1;
            
            Repaint();
        }
        
        private List<InjectorInfo> LoadInjectorsFromContext()
        {
            var result = new List<InjectorInfo>(3);
            var context = Selection.activeGameObject;
            if (!context) return result;

            result.Add(new InjectorInfo("Root", _injector.Injector));

            if (!context.TryGetComponent<SGameObjectInject>(out var gInject)) return result;
            if (!gInject.CreateLocalInjector) return result;
            
            result.Add(new InjectorInfo("GameObject", gInject.LocalInjector));
            return result;
        }


        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Enter play mode to debug the DI context", MessageType.Info, true);
                return;
            }
            
            if (!_injector) _injector = SRootInjector.FindSingleton();

            if (!_injector)
            {
                EditorGUILayout.HelpBox("No root injector found", MessageType.Info, true);
                return;
            }

            if (_dirty) RefreshContext();

            if (_injectors.IsEmpty())
            {
                EditorGUILayout.HelpBox("No injectors found", MessageType.Info, true);
                return;
            }

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            for (var index = 0; index < _injectors.Count; index++)
            {
                if (index != 0) GUI.Label(EditorGUILayout.GetControlRect(GUILayout.Width(10)), ">");

                if (index == _selectedInjector) EditorGUI.BeginDisabledGroup(true);
                var clicked = GUILayout.Button(_injectors[index].Name);
                if (index == _selectedInjector) EditorGUI.EndDisabledGroup();

                if (clicked) _selectedInjector = index;
            }

            EditorGUILayout.EndHorizontal();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            _injectors[_selectedInjector].Gui.OnGUI();
            EditorGUILayout.EndScrollView();
            
        }
    }
}