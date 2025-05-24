using System;
using System.Collections.Generic;
using System.Reflection;
using Sapo.DI.Runtime.Behaviours;
using Sapo.DI.Runtime.Common;
using Sapo.DI.Runtime.Core;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sapo.DI.Editor.Common
{
    public class InjectorRuntimeInfo
    {
        private readonly Dictionary<Type, SInstanceCollection> _instances;
        private readonly Dictionary<Type, SInstanceCollection> _parentInstances;

        public InjectorRuntimeInfo(SInjector injector)
        {
            var t = injector.GetType();
            
            var instancesField = t.GetField("_instances", BindingFlags.NonPublic | BindingFlags.Instance);

            _instances = (Dictionary<Type, SInstanceCollection>)instancesField!.GetValue(injector);

            if (t.GetField("_parent", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(injector) is not
                SInjector parent) return;

            _parentInstances = (Dictionary<Type, SInstanceCollection>)instancesField.GetValue(parent);
        }
        
        public void OnGUI()
        {
            EditorGUI.indentLevel++;

            var hasInstances = DrawInstances(_parentInstances, new Color(1, 0.5f, 0f));
            hasInstances |= DrawInstances(_instances, new Color(0.5f, 1, 0f));

            if (!hasInstances)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField("No instances registered.", EditorStyles.miniBoldLabel);
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUI.indentLevel--;
        }

        private bool DrawInstances(Dictionary<Type, SInstanceCollection> instances, Color color)
        {
            if (instances == null) return false;
            
            var hasInstances = false;
            
            foreach (var (type, collection) in instances)
            {
                var first = true;
                
                foreach (var instance in collection.Instances)
                {
                    if (!instance.IsAlive()) continue;
                    
                    var c = GUI.color;
                    GUI.color = color;
                    GUI.Box(EditorGUI.IndentedRect(EditorGUILayout.BeginVertical()), "", EditorStyles.helpBox);
                    GUI.color = c;
                    GUILayout.Space(2);


                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(4);
                
                    EditorGUILayout.LabelField(type.Name, EditorStyles.boldLabel);

                    EditorGUI.BeginDisabledGroup(true);
                    if (instance is Object uo) EditorGUILayout.ObjectField(uo, uo.GetType(), true);
                    else EditorGUILayout.LabelField(GetInstanceInfo(instance));
                    EditorGUI.EndDisabledGroup();
                
                    GUILayout.Space(2);
                    EditorGUILayout.EndHorizontal();

                    GUILayout.Space(2);
                    EditorGUILayout.EndVertical();

                    if (first) EditorGUI.indentLevel++;
                    first = false;
                }
                
                if (!first) EditorGUI.indentLevel--;

                hasInstances |= !first;
            }

            return hasInstances;
        }


        
        private string GetInstanceInfo(object instance)
        {
            var str = instance.ToString();
            return str == instance.GetType().ToString() ? $"{str} [{instance.GetShortHashCode()}]" : str;
        }
    }
}