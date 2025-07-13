using Overefactor.DI.Runtime.Attributes;
using UnityEngine;

namespace Overefactor.DI.Samples.Testing.Runtime
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicController : MonoBehaviour
    {
        [SInject] private IGameSoundSettings _settings;
        
        private AudioSource _source;

        private void Awake() => _source = GetComponent<AudioSource>();

        private void Start() => _source.volume = _settings.MusicVolume;

        private void OnEnable() => _settings.OnChanged += OnSettingsChanged;
        
        private void OnDisable() => _settings.OnChanged -= OnSettingsChanged;

        private void OnSettingsChanged()
        {
            _source.volume = _settings.MusicVolume;
        }
    }
}