using Overefactor.DI.Runtime.Attributes;
using Overefactor.DI.Runtime.Common;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Overefactor.DI.Editor.Editor.Attributes
{
    /// <summary>
    /// A GUI drawer for <see cref="SRegister"/> attribute.
    /// </summary>
    public static class SRegisterGUI
    {
        private static GUIStyle _style;
        
        
        /// <summary>
        /// Use to draw the info of the component that is registered with <see cref="SRegister"/> attribute.
        /// </summary>
        /// <remarks>
        /// Here is an example of how to use this method in your custom inspector:
        /// <code>
        /// public override void OnInspectorGUI()
        /// {
        ///     SRegisterGUI.DrawInfo(target);
        ///  
        ///     // Your custom inspector code here
        /// }
        /// </code>
        /// </remarks>
        /// <param name="target">The target object that you want to draw the info.</param>
        public static void DrawInfo(Object target)
        {
            if (!target.GetType().IsDefinedWithAttribute<SRegister>(out var attribute)) return;
            
            _style ??= new GUIStyle(EditorStyles.helpBox) { richText = true };
            var type = attribute.Type ?? target.GetType();
            EditorGUILayout.LabelField(
                target is Component
                    ? $"Component registered as <color=#FF8000>{type}</color>"
                    : $"Object can be registered as <color=#FF8000>{type}</color>", _style);
        }


        /// <summary>
        /// Use to draw the info of the component that is not registered with <see cref="SRegister"/> attribute.
        /// </summary>
        /// <remarks>
        /// Here is an example of how to use this method in your custom inspector:
        /// <code>
        /// public override void OnInspectorGUI()
        /// {
        ///     SRegisterGUI.DrawInfo&lt;IMyObject&gt;(target);
        ///  
        ///     // Your custom inspector code here
        /// }
        /// </code>
        /// </remarks>
        /// <param name="target">The target object that you want to draw the info.</param>
        public static void DrawInfo<T>(Object target)
        {
            _style ??= new GUIStyle(EditorStyles.helpBox) { richText = true };
            var type = typeof(T);

            EditorGUILayout.LabelField(
                target is Component
                    ? $"Component registered as <color=#FF8000>{type.Name}</color>"
                    : $"Object can be registered as <color=#FF8000>{type.Name}</color>", _style);
        }
    }
}