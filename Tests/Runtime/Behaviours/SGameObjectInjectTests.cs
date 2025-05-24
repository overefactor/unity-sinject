using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Sapo.DI.Runtime.Behaviours;
using Sapo.DI.Runtime.Interfaces;
using Sapo.DI.Tests.Runtime.Helpers;
using Sapo.DI.Tests.Runtime.TestData;
using UnityEngine;
using UnityEngine.TestTools;
using Is = Sapo.DI.Tests.Runtime.Helpers.Is;

namespace Sapo.DI.Tests.Runtime.Behaviours
{
    [TestFixture]
    internal class SGameObjectInjectTests
    {
        private readonly List<GameObject> _gameObjects = new();
        
        [TearDown]
        public void TearDown()
        {
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
            if (Application.isEditor) Assert.Ignore("This test only runs in Player builds.");
            
            // Arrange
            LogAssert.ignoreFailingMessages = true;
            var g = NewG(false);
            var inject = g.AddComponent<SGameObjectInject>();
            
            // Act
            g.SetActive(true);
            yield return null;
            
            // Assert
            Assert.That(inject, Is.Destroyed);
            Assert.That(g, Is.Not.Destroyed());
        }

        [UnityTest]
        public IEnumerator Awake_WithRootInjector_ShouldInjectAndDestroySelf()
        {
            if (Application.isEditor) Assert.Ignore("This test only runs in Player builds.");

            // Arrange
            var injector = NewG().AddComponent<SRootInjector>();
            var g = NewG(false);
            var inject = g.AddComponent<SGameObjectInject>();
            var service = g.AddComponent<ComponentServiceA>();
            
            // Act
            g.SetActive(true);
            yield return null;
            
            // Assert
            Assert.That(inject, Is.Destroyed);
            Assert.That(g, Is.Not.Destroyed());
            Assert.That(injector.Injector.IsRegistered<IServiceA>(), Is.True);
            Assert.That(injector.Injector.Resolve<IServiceA>(), Is.EqualTo(service));
        }

        [UnityTest]
        public IEnumerator Awake_WithLocalInjector_ShouldInjectAndDestroySelf()
        {
            if (Application.isEditor) Assert.Ignore("This test only runs in Player builds.");

            // Arrange
            var g = NewG(false);
            var inject = g.AddComponent<SGameObjectInject>();
            inject.CreateLocalInjector = true;
            
            var service = g.AddComponent<ComponentServiceA>();
            var component = g.AddComponent<ComponentWithDependencyToA>();

            // Act
            g.SetActive(true);
            yield return null;
            
            // Assert
            Assert.That(inject, Is.Destroyed);
            Assert.That(g, Is.Not.Destroyed());
            Assert.That(component.ServiceA, Is.EqualTo(service));
        }

        [UnityTest]
        public IEnumerator Awake_WithLocalInjector_WithGameObjectInjector_ShouldCallEvents_WithCorrectOrder()
        {
            if (Application.isEditor) Assert.Ignore("This test only runs in Player builds.");

            // Arrange
            var g = NewG(false);
            var inject = g.AddComponent<SGameObjectInject>();
            inject.CreateLocalInjector = true;
            var component = g.AddComponent<ComponentWithEvents>();
            
            // Act
            g.SetActive(true);
            yield return null;
            
            // Assert
            Assert.That(inject, Is.Destroyed);
            Assert.That(component.Events, Is.EqualTo(new[] { "OnRegister", "OnInject" }));
        }
        
        [UnityTest]
        public IEnumerator Awake_WithAlreadyInjected_ShouldDestroySelf()
        {
            if (Application.isEditor) Assert.Ignore("This test only runs in Player builds.");

            // Arrange
            var g = NewG(false);
            var inject = g.AddComponent<SGameObjectInject>();
            inject.CreateLocalInjector = true;
            
            NewG().AddComponent<SRootInjector>();
            yield return null;

            // Act
            g.SetActive(true);
            yield return null;
            
            // Assert
            Assert.That(inject, Is.Destroyed);
            Assert.That(g, Is.Not.Destroyed());
        }

        #endregion
        
    }
}