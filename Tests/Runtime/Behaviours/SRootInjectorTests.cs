using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    internal class SRootInjectorTests
    {
        private List<GameObject> _gameObjects = new();
        
        private SRootInjector CreateUninitializedInjector()
        {
            var g = new GameObject("Root Injector");
            g.SetActive(false);

            _gameObjects.Add(g);
            return g.AddComponent<SRootInjector>();
        }

        private SRootInjector CreateInjector() => new GameObject("Root Injector").AddComponent<SRootInjector>();

        private void AddAssetToRegister(SRootInjector injector, Object asset)
        {
            var field = injector.GetType().GetField("assetsToRegister", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null) throw new AssertionException("Field not found");
            
            var list = (List<Object>) field.GetValue(injector);
            list.Add(asset);
        }

        private T NewGWithComponent<T>() where T : Component => NewGWithComponent<T>(out _);

        private T NewGWithComponent<T>(out GameObject g) where T : Component
        {
            g = new GameObject();
            g.SetActive(false);
            _gameObjects.Add(g);
            return g.AddComponent<T>();
        }
        
        [SetUp]
        [TearDown]
        public void ClearHierarchy()
        {
            foreach (var g in Object.FindObjectsOfType<SRootInjector>()) 
                Object.DestroyImmediate(g.gameObject);

            foreach (var g in Object.FindObjectsOfType<SGameObjectInject>())
                Object.DestroyImmediate(g.gameObject);

            foreach (var g in _gameObjects)
            {
                if (g == null) continue;
                
                Object.DestroyImmediate(g);
            }
            
            _gameObjects.Clear();
        }
        

        #region set_MakePersistent

        [Test]
        public void set_MakePersistent_SetsTrue()
        {
            // Arrange
            var injector = CreateUninitializedInjector();
            
            // Act
            injector.MakePersistent = true;
            
            // Assert
            Assert.That(injector.MakePersistent, Is.True);
        }
        
        [Test]
        public void set_MakePersistent_SetsFalse()
        {
            // Arrange
            var injector = CreateUninitializedInjector();
            
            // Act
            injector.MakePersistent = false;
            
            // Assert
            Assert.That(injector.MakePersistent, Is.False);
        }
        
        [UnityTest]
        public IEnumerator set_MakePersistent_SetsTrueAndPersists()
        {
            // Arrange
            var injector = CreateUninitializedInjector();
            
            // Act
            injector.MakePersistent = true;
            injector.gameObject.SetActive(true);
            yield return null;
            
            // Assert
            Assert.That(injector.MakePersistent, Is.True);
            Assert.That(injector.gameObject.scene.buildIndex, Is.EqualTo(-1));
        }
        
        [UnityTest]
        public IEnumerator set_MakePersistent_SetsFalseAndDoesNotPersist()
        {
            // Arrange
            var injector = CreateUninitializedInjector();
            
            // Act
            injector.MakePersistent = false;
            injector.gameObject.SetActive(true);
            yield return null;
            
            // Assert
            Assert.That(injector.MakePersistent, Is.False);
            Assert.That(injector.gameObject.scene.buildIndex, Is.Not.EqualTo(-1));
        }

        #endregion

        #region get_MakePersistent

        [Test]
        public void get_MakePersistent_ReturnsFalse()
        {
            // Arrange
            var injector = CreateUninitializedInjector();
            injector.MakePersistent = false;
            
            // Act
            var result = injector.MakePersistent;
            
            // Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        public void get_MakePersistent_ReturnsTrue()
        {
            // Arrange
            var injector = CreateUninitializedInjector();
            injector.MakePersistent = true;
            
            // Act
            var result = injector.MakePersistent;
            
            // Assert
            Assert.That(result, Is.True);
        }

        #endregion

        #region get_Injector

        [Test]
        public void get_Injector_ReturnsInjector()
        {
            // Arrange
            var injector = CreateUninitializedInjector();
            
            // Act
            var result = injector.Injector;
            
            // Assert
            Assert.That(result, Is.Not.Null);
        }

        #endregion

        #region Awake & Initialize

        [Test]
        public void Awake_ShouldNotThrow()
        {
            // Arrange
            var g = new GameObject("Root Injector");
            
            // Act
            void Act() => g.AddComponent<SRootInjector>();
            
            // Assert
            Assert.That(Act, Throws.Nothing);
        }

        [Test]
        public void Awake_WhenMultipleRootInjectors_ShouldNotThrow()
        {
            // Arrange
            var g = new GameObject("Root Injector");
            CreateUninitializedInjector();
            
            // Act
            void Act() => g.AddComponent<SRootInjector>();
            
            // Assert
            Assert.That(Act, Throws.Nothing);
        }
        
        [UnityTest]
        public IEnumerator Awake_WhenMultipleRootInjectors_ShouldDestroyOther()
        {
            // Arrange
            var g = new GameObject("Root Injector");
            var other = CreateUninitializedInjector();
            
            // Act
            g.AddComponent<SRootInjector>();
            yield return null;
            
            // Assert
            Assert.That(other, Is.Destroyed);
        }

        [UnityTest]
        public IEnumerator Awake_WhenAlreadyInitializedOtherInjector_ShouldDestroy()
        {
            // Arrange
            var other = CreateUninitializedInjector();
            other.gameObject.SetActive(true);

            var g = new GameObject("Root Injector");
            
            // Act
            var injector = g.AddComponent<SRootInjector>();
            yield return null;
            
            // Assert
            Assert.That(injector, Is.Destroyed);
        }

        [UnityTest]
        public IEnumerator Initialize_WhenAlreadyInitialized_ShouldPass()
        {
            // Arrange
            var injector = CreateUninitializedInjector();
            injector.gameObject.SetActive(true);
            var scene = injector.gameObject.scene;
            yield return null;
            
            // Act
            injector.InjectScene(scene);
            yield return null;
            
            // Assert
            Assert.Pass();
        }
        
        [UnityTest]
        public IEnumerator Awake_WithAssetsToRegister_ShouldRegister()
        {
            // Arrange
            var injector = CreateUninitializedInjector();
            var service = ScriptableObject.CreateInstance<ScriptableServiceA>();
            AddAssetToRegister(injector, service);
            
            // Act
            injector.gameObject.SetActive(true);
            yield return null;
            
            // Assert
            Assert.That(injector.Injector.IsRegistered<IServiceA>(), Is.True);
            Assert.That(injector.Injector.Resolve<IServiceA>(), Is.EqualTo(service));
        }

        [UnityTest]
        public IEnumerator Awake_OnServices_ShouldCallEvents_WithCorrectOrder()
        {
            // Arrange
            var injector = CreateUninitializedInjector();
            var service = ScriptableObject.CreateInstance<ScriptableServiceAWithEvents>();
            AddAssetToRegister(injector, service);
            
            // Act
            injector.gameObject.SetActive(true);
            yield return null;
            
            // Assert
            Assert.That(service.Events, Is.EqualTo(new[] { "OnRegister", "OnInject" }));
        }
        
        [UnityTest]
        public IEnumerator Awake_WithNullAssetToRegister_ShouldPass()
        {
            // Arrange
            var injector = CreateUninitializedInjector();
            var service = ScriptableObject.CreateInstance<ScriptableServiceA>();
            AddAssetToRegister(injector, null);
            AddAssetToRegister(injector, service);
            
            // Act
            injector.gameObject.SetActive(true);
            yield return null;
            
            // Assert
            Assert.That(injector.Injector.IsRegistered<IServiceA>(), Is.True);
            Assert.That(injector.Injector.Resolve<IServiceA>(), Is.EqualTo(service));
        }

        [UnityTest]
        public IEnumerator Awake_WithNonServiceAssetToRegister_ShouldPass()
        {
            // Arrange
            var injector = CreateUninitializedInjector();
            var service = ScriptableObject.CreateInstance<ScriptableServiceA>();
            AddAssetToRegister(injector, new GameObject());
            AddAssetToRegister(injector, service);
            
            // Act
            injector.gameObject.SetActive(true);
            yield return null;
            
            // Assert
            Assert.That(injector.Injector.IsRegistered<IServiceA>(), Is.True);
            Assert.That(injector.Injector.Resolve<IServiceA>(), Is.EqualTo(service));
        }
        
        
        #endregion

        #region InjectGameObject

        [UnityTest]
        public IEnumerator InjectGameObject_ShouldRegisterServices()
        {
            // Arrange
            var injector = CreateInjector();
            var service = NewGWithComponent<ComponentServiceA>(out var go);

            // Act
            injector.InjectGameObject(go);
            yield return null;
            
            // Assert
            Assert.That(injector.Injector.IsRegistered<IServiceA>(), Is.True);
            Assert.That(injector.Injector.Resolve<IServiceA>(), Is.EqualTo(service));
        }
        
        [UnityTest]
        public IEnumerator InjectGameObject_WithNullGameObject_ShouldThrowException()
        {
            // Arrange
            var injector = CreateInjector();
            
            // Act
            void Act() => injector.InjectGameObject(null);
            yield return null;
            
            // Assert
            Assert.That(Act, Throws.ArgumentNullException);
        }

        [UnityTest]
        public IEnumerator InjectGameObject_ShouldInjectComponents()
        {
            // Arrange
            var injector = CreateInjector();
            
            var service = NewGWithComponent<ComponentServiceA>(out var go);
            var component = go.AddComponent<ComponentWithDependencyToA>();
            
            // Act
            injector.InjectGameObject(go);
            yield return null;
            
            // Assert
            Assert.That(component.ServiceA, Is.EqualTo(service));
        }
        
        [UnityTest]
        public IEnumerator InjectGameObject_ShouldCallEvents_WithCorrectOrder()
        {
            // Arrange
            var injector = CreateInjector();
            var component = NewGWithComponent<ComponentWithEvents>(out var go);
            
            // Act
            injector.InjectGameObject(go);
            yield return null;
            
            // Assert
            Assert.That(component.Events, Is.EqualTo(new[] { "OnRegister", "OnInject" }));
        }

        [UnityTest]
        public IEnumerator InjectGameObject_WithGameObjectInjectorInScene_ShouldInject()
        {
            // Arrange
            var injector = CreateInjector();
            
            var gInjector = NewGWithComponent<SGameObjectInject>(out var go);
            gInjector.CreateLocalInjector = true;
            
            var service = go.AddComponent<ComponentServiceA>();
            var component = go.AddComponent<ComponentWithDependencyToA>();
            
            // Act
            injector.InjectGameObject(go);
            yield return null;
            
            // Assert
            Assert.That(injector.Injector.IsRegistered<IServiceA>(), Is.False);
            Assert.That(component.ServiceA, Is.EqualTo(service));
        }
        

        #endregion

        #region InjectScene

        [UnityTest]
        public IEnumerator InjectScene_ShouldNotThrow()
        {
            // Arrange
            var injector = CreateInjector();
            var scene = SceneManager.GetActiveScene();
            
            // Act
            void Act() => injector.InjectScene(scene);
            yield return null;
            
            // Assert
            Assert.That(Act, Throws.Nothing);
        } 
        
        [UnityTest]
        public IEnumerator InjectScene_ShouldRegisterServices()
        {
            // Arrange
            var injector = CreateInjector();
            var scene = SceneManager.GetActiveScene();
            var service = NewGWithComponent<ComponentServiceA>();
            
            // Act
            injector.InjectScene(scene);
            yield return null;
            
            // Assert
            Assert.That(injector.Injector.IsRegistered<IServiceA>(), Is.True);
            Assert.That(injector.Injector.Resolve<IServiceA>(), Is.EqualTo(service));
        }
        
        [UnityTest]
        public IEnumerator InjectScene_ShouldInjectComponents()
        {
            // Arrange
            var injector = CreateInjector();
            var scene = SceneManager.GetActiveScene();
            var service = NewGWithComponent<ComponentServiceA>();
            var component = NewGWithComponent<ComponentWithDependencyToA>();
            
            // Act
            injector.InjectScene(scene);
            yield return null;
            
            // Assert
            Assert.That(component.ServiceA, Is.EqualTo(service));
        }
        
        [UnityTest]
        public IEnumerator InjectScene_ShouldCallEvents_WithCorrectOrder()
        {
            // Arrange
            var injector = CreateInjector();
            var scene = SceneManager.GetActiveScene();
            var component = NewGWithComponent<ComponentWithEvents>();
            
            // Act
            injector.InjectScene(scene);
            yield return null;
            
            // Assert
            Assert.That(component.Events, Is.EqualTo(new[] { "OnRegister", "OnInject" }));
        }
        
        [UnityTest]
        public IEnumerator InjectScene_WithGameObjectInjectorInScene_ShouldInject()
        {
            // Arrange
            var injector = CreateInjector();
            var scene = SceneManager.GetActiveScene();
            
            var gInjector = NewGWithComponent<SGameObjectInject>(out var g);
            gInjector.CreateLocalInjector = true;
            
            var service = g.AddComponent<ComponentServiceA>();
            var component = g.AddComponent<ComponentWithDependencyToA>();
            
            // Act
            injector.InjectScene(scene);
            yield return null;
            
            // Assert
            Assert.That(injector.Injector.IsRegistered<IServiceA>(), Is.False);
            Assert.That(component.ServiceA, Is.EqualTo(service));
        }

        [UnityTest]
        public IEnumerator InjectScene_WithServiceOnSelf_ShouldInject()
        {
            // Arrange
            var injector = CreateUninitializedInjector();
            var service = injector.gameObject.AddComponent<ComponentServiceA>();
            var scene = SceneManager.GetActiveScene();
            
            // Act
            injector.InjectScene(scene);
            injector.gameObject.SetActive(true);
            yield return null;
            
            // Assert
            Assert.That(injector.Injector.IsRegistered<IServiceA>(), Is.True);
            Assert.That(injector.Injector.Resolve<IServiceA>(), Is.EqualTo(service));
        }

        [UnityTest]
        public IEnumerator InjectScene_WithInjectableOnSelf_ShouldInject()
        {
            // Arrange
            var injector = CreateUninitializedInjector();
            var component = injector.gameObject.AddComponent<ComponentWithDependencyToA>();
            var service = injector.gameObject.AddComponent<ComponentServiceA>();
            var scene = SceneManager.GetActiveScene();
            
            // Act
            injector.InjectScene(scene);
            injector.gameObject.SetActive(true);
            yield return null;
            
            // Assert
            Assert.That(component.ServiceA, Is.EqualTo(service));
        }
        
        #endregion
    }
}