using System;
using System.Collections;
using NUnit.Framework;
using Overefactor.DI.Tests.Tests.Runtime.Helpers;
using UnityEngine;
using UnityEngine.TestTools;

namespace Overefactor.DI.Samples.Testing.Runtime.Tests
{
    [TestFixture]
    public class MusicControllerTests
    {
        private class TestSettings : IGameSoundSettings
        {
            public float MusicVolume { get; set; }
            
            public event Action OnChanged;
            
            public void InvokeOnChanged() => OnChanged?.Invoke();
        }
        
        private STestRootInjector _injector;

        [SetUp]
        public void SetUp()
        {
            _injector = new STestRootInjector(true);
        }
        
        [TearDown]
        public void TearDown()
        {
            _injector.Dispose();
        }
        
        [UnityTest]
        public IEnumerator OnSettingsChanged_WithMusicVolumeChanged_ShouldChangeVolume()
        {
            // Arrange
            var musicController = _injector.CreateGWith<MusicController>();
            var audioSource = musicController.GetComponent<AudioSource>();
            var musicVolume = 0.5f;

            var settings = new TestSettings { MusicVolume = 1f };
            _injector.Register<IGameSoundSettings>(settings);
            
            _injector.InjectAndEnableScene();
            yield return null;
            
            // Act
            settings.MusicVolume = musicVolume;
            settings.InvokeOnChanged();
            
            // Assert
            Assert.That(audioSource.volume, Is.EqualTo(musicVolume));
        }
        
        [UnityTest]
        public IEnumerator Start_WithMusicVolumeSet_ShouldSetVolume()
        {
            // Arrange
            var musicController = _injector.CreateGWith<MusicController>();
            var audioSource = musicController.GetComponent<AudioSource>();
            var musicVolume = 0.7f;

            var settings = new TestSettings { MusicVolume = musicVolume };
            _injector.Register<IGameSoundSettings>(settings);
            _injector.InjectAndEnableScene();

            // Act
            yield return null;
            
            // Assert
            Assert.That(audioSource.volume, Is.EqualTo(musicVolume));
        }
    }
}