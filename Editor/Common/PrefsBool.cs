using UnityEditor;

namespace Overefactor.DI.Editor.Editor.Common
{
    internal class PrefsBool
    {
        public string Key { get; }

        private bool _value;
        public bool Value
        {
            get => _value;
            set
            {
                if (_value == value) return;
                
                _value = value;
                EditorPrefs.SetBool(Key, value);
            }
        }

        public PrefsBool(string key) => Key = key;
        
        public void Load() => _value = EditorPrefs.GetBool(Key);
    }
}