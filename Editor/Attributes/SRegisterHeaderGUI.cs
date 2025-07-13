using System;
using System.Collections.Generic;
using System.Linq;
using Overefactor.DI.Runtime.Common;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Task = System.Threading.Tasks.Task;

namespace Overefactor.DI.Editor.Editor.Attributes
{
    [InitializeOnLoad]
    internal class SRegisterHeaderGUI
    {
        private static Dictionary<Object, string[]> _registerTypes = new Dictionary<Object, string[]>();
        
        private static SReflectionCache _reflectionCache;
        private static GUIStyle _style;
        private static float _currentWidth;
        private static float _width = 1000;
        private static readonly GUIContent _tempContent = new GUIContent();
        private static bool _isReflectionCacheBuilding = true;

        private static SReflectionCache ReflectionCache
        {
            get
            {
                if (_reflectionCache != null) return _reflectionCache;

                _reflectionCache = new SReflectionCache();
                Task.Run(() =>
                {
                    try
                    {
                        _reflectionCache.Build();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }

                    _isReflectionCacheBuilding = false;

                });
                return _reflectionCache;
            }
        }


        static SRegisterHeaderGUI()
        {
            UnityEditor.Editor.finishedDefaultHeaderGUI += DrawInfo;
        }

        private static void DrawInfo(UnityEditor.Editor obj)
        {
            if (!obj.targets.SelectMany(GetRegisterTypes).Any()) return;
            
            _style ??= new GUIStyle(EditorStyles.helpBox) { richText = true };

            var width = EditorGUILayout.BeginHorizontal(GUILayout.ExpandHeight(false)).width;
            _currentWidth = 0;
            
            foreach (var type in obj.targets.SelectMany(GetRegisterTypes))
            {
                _tempContent.text = type;
                var x = _style.CalcSize(_tempContent).x;
                if (_currentWidth + x > _width)
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    _currentWidth = 0;
                }

                _currentWidth += x + 5;
                
                EditorGUILayout.LabelField(_tempContent, _style, GUILayout.ExpandWidth(false));
            }
            
            if (Event.current.type == EventType.Repaint) _width = width;

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        

        
        private static IEnumerable<string> GetRegisterTypes(Object target)
        {
            if (_isReflectionCacheBuilding)
            {
                _ = ReflectionCache;
                return Enumerable.Empty<string>();
            }
            
            if (target is GameObject g) return g.GetComponents<Component>().SelectMany(GetRegisterTypesFor);
            
            return GetRegisterTypesFor(target);
        }

        private static string[] GetRegisterTypesFor(Object target)
        {
            if (_registerTypes.TryGetValue(target, out var types)) return types;
            
            types = ReflectionCache.GetRegisterTypes(target.GetType()).Select(t => $"<color=#ff8000>{t.FullName}</color>").ToArray();
            _registerTypes[target] = types;
            return types;
        }

    }
}