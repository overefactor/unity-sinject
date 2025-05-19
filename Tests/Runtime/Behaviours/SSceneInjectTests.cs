using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Sapo.DI.Runtime.Behaviours;
using Sapo.DI.Runtime.Interfaces;
using Sapo.DI.Tests.Runtime.TestData;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Is = Sapo.DI.Tests.Runtime.Helpers.Is;

namespace Sapo.DI.Tests.Runtime.Behaviours
{
    [TestFixture]
    internal class SSceneInjectTests
    {
        private readonly List<GameObject> _gameObjects = new();
        
        [SetUp]
        [TearDown]
        public void ClearHierarchy()
        {
            foreach (var g in Object.FindObjectsOfType<SRootInjector>()) 
                Object.DestroyImmediate(g.gameObject);

            foreach (var g in Object.FindObjectsOfType<SSceneInject>())
                Object.DestroyImmediate(g.gameObject);
            
            foreach (var go in _gameObjects)
            {
                if (go == null) continue;

                Object.DestroyImmediate(go);
            }
            _gameObjects.Clear();
        }

        private GameObject NewG(bool active = true)
        {
            var g = new GameObject();
            g.SetActive(active);
            _gameObjects.Add(g);
            return g;
        }

        #region Awake
        
        [UnityTest]
        public IEnumerator Awake_WithNoRootInjector_ShouldDestroySelf()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;
            var g = NewG();
            
            // Act
            g.AddComponent<SSceneInject>();
            yield return null;
            
            // Assert
            Assert.That(g, Is.Destroyed);
        }
        
        [UnityTest]
        public IEnumerator Awake_WithRootInjector_ShouldInjectSceneAndDestroySelf()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;
            var injector = NewG().AddComponent<SRootInjector>();
            var g = NewG();
            var service = NewG(false).AddComponent<ComponentServiceA>();
            yield return null;
            
            // Act
            g.AddComponent<SSceneInject>();
            yield return null;
            
            // Assert
            Assert.That(g, Is.Destroyed);
            Assert.That(injector.Injector.IsRegistered<IServiceA>(), Is.True);
            Assert.That(injector.Injector.Resolve<IServiceA>(), Is.EqualTo(service));
        }
        
        [UnityTest]
        public IEnumerator Awake_WithAlreadyInjected_ShouldDestroySelf()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;
            var injector = NewG().AddComponent<SRootInjector>();
            var g = NewG(false);
            g.AddComponent<SSceneInject>();
            injector.InjectScene(SceneManager.GetActiveScene());
            
            yield return null;
            
            // Act
            g.SetActive(true);
            yield return null;
            
            // Assert
            Assert.That(g, Is.Destroyed);
        }
        
        #endregion

    }
}