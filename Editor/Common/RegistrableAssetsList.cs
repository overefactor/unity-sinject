using System;
using System.Collections.Generic;
using System.Linq;
using Overefactor.DI.Runtime.Attributes;
using Overefactor.DI.Runtime.Common;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Overefactor.DI.Editor.Editor.Common
{
    internal class RegistrableAssetsList
    {
        private readonly SerializedProperty _property;
        private readonly ReorderableList _list;

        public RegistrableAssetsList(SerializedProperty property)
        {
            _property = property;
            
            _list = new ReorderableList(property.serializedObject, property, true, true, true, true);
            _list.drawHeaderCallback = rect => GUI.Label(rect, "Assets to register");
            _list.drawElementCallback = (rect, index, _, _) =>
            {
                rect.y += 2;
                rect.height -= 4;
                var element = _property.GetArrayElementAtIndex(index);

                var prevValue = element.objectReferenceValue;
                EditorGUI.BeginChangeCheck();
                EditorGUI.ObjectField(rect, element, GUIContent.none);
                if (!EditorGUI.EndChangeCheck()) return;

                var obj = element.objectReferenceValue;
                element.objectReferenceValue = null;
                element.objectReferenceValue = CanAddAssetToRegister(obj) ? obj : prevValue;
            };
            _list.onAddCallback = _ =>
            {
                _property.arraySize++;
                _property.GetArrayElementAtIndex(_property.arraySize - 1).objectReferenceValue = null;
            };
            _list.multiSelect = true;
            _list.elementHeight = 20;
        }
        
        private bool CanAddAssetToRegister(Object obj)
        {
            if (obj == null) return true;
            if (!obj.GetType().IsDefinedWithAttribute<SRegister>(out var attr)) return false;

            return GetAllRegisteredAssets().All(registeredAsset =>
                registeredAsset.registerType != attr.Type && registeredAsset.obj != obj);
        }

        private IEnumerable<(Object obj, Type registerType)> GetAllRegisteredAssets()
        {
            foreach (SerializedProperty asset in _property)
            {
                var obj = asset.objectReferenceValue;
                if (obj == null) continue;
                if (!obj.GetType().IsDefinedWithAttribute<SRegister>(out var attr)) continue;

                yield return (obj, attr.Type);
            }
        }

        private void AddAssetsToRegister(Object[] objs)
        {
            foreach (var obj in objs)
            {
                if (obj == null) continue;
                if (!CanAddAssetToRegister(obj)) continue;
                
                _property.arraySize++;
                _property.GetArrayElementAtIndex(_property.arraySize - 1).objectReferenceValue = obj;
            }
        }


        public void OnGUI()
        {
            var evt = Event.current;
            var rect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, _list.GetHeight()));
            _list.DoList(rect);
            
            if (evt.type == EventType.DragUpdated && rect.Contains(evt.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                evt.Use();
            }
            if (evt.type == EventType.DragPerform && rect.Contains(evt.mousePosition))
            {
                DragAndDrop.AcceptDrag();
                AddAssetsToRegister(DragAndDrop.objectReferences);
                GUI.changed = true;
                evt.Use();
            }

        }
    }
}