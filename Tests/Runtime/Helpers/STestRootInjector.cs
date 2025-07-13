using System;
using System.Collections.Generic;
using Overefactor.DI.Runtime.Behaviours;
using Overefactor.DI.Runtime.Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Overefactor.DI.Tests.Tests.Runtime.Helpers
{
    /// <summary>
    /// A helper class for creating a root injector for testing purposes.
    /// </summary>
    public class STestRootInjector : ISInjector, IDisposable
    {
        private readonly ISInjector _injector;
        private readonly GameObject _sceneRoot;
        
        private readonly SRootInjector _rootInjector;
        private bool _isSceneInjected;

        /// <summary>
        /// Creates a new instance of <see cref="STestRootInjector"/>.
        /// </summary>
        /// <param name="makePersistent">Whether the root injector should be persistent.</param>
        public STestRootInjector(bool makePersistent)
        {
            var g = new GameObject("Root Injector");
            _rootInjector = g.AddComponent<SRootInjector>();
            
            _rootInjector.MakePersistent = makePersistent;
            _injector = _rootInjector.Injector;
            
            _sceneRoot = new GameObject("Disabled Root");
            _sceneRoot.SetActive(false);
        }


        public T Resolve<T>() => _injector.Resolve<T>();

        public object Resolve(Type type) => _injector.Resolve(type);

        public bool TryResolve<T>(out T instance) => _injector.TryResolve(out instance);

        public bool TryResolve(Type type, out object instance) => _injector.TryResolve(type, out instance);
        
        public void ResolveAll(Type type, ISet<object> instances) => _injector.ResolveAll(type, instances);

        public bool IsRegistered<T>() => _injector.IsRegistered<T>();

        public bool IsRegistered(Type type) => _injector.IsRegistered(type);

        public void Register<T>(object instance) => _injector.Register<T>(instance);

        public void Register(Type type, object instance) => _injector.Register(type, instance);

        public bool TryRegister<T>(object instance) => _injector.TryRegister<T>(instance);

        public bool TryRegister(Type type, object instance) => _injector.TryRegister(type, instance);

        public void Unregister<T>(object instance) => _injector.Unregister<T>(instance);

        public void Unregister(Type type, object instance) => _injector.Unregister(type, instance);

        public void Inject(object instance) => _injector.Inject(instance);
        
        /// <summary>
        /// This method injects the scene and makes all objects alive.
        /// </summary>
        public void InjectAndEnableScene()
        {
            if (_isSceneInjected) return;
            
            _rootInjector.InjectScene(_sceneRoot.scene);
            _sceneRoot.SetActive(true);
            _isSceneInjected = true;
        }

        #region Create GameObject
        
        
        /// <summary>
        /// Creates a new game object with the specified name and components.
        /// </summary>
        /// <param name="name">Name of the game object.</param>
        /// <returns>The created game object.</returns>
        public GameObject CreateG(string name = null) => 
            CreateG(name, Type.EmptyTypes);

        /// <inheritdoc cref="CreateG(string)"/>
        public GameObject CreateG<T1>(string name = null) => 
            CreateG(name, typeof(T1));
        
        /// <inheritdoc cref="CreateG(string)"/>
        public GameObject CreateG<T1, T2>(string name = null) =>
            CreateG(name, typeof(T1), typeof(T2));
        
        /// <inheritdoc cref="CreateG(string)"/>
        public GameObject CreateG<T1, T2, T3>(string name = null) =>
            CreateG(name, typeof(T1), typeof(T2), typeof(T3));
        
        /// <inheritdoc cref="CreateG(string)"/>
        public GameObject CreateG<T1, T2, T3, T4>(string name = null) =>
            CreateG(name, typeof(T1), typeof(T2), typeof(T3), typeof(T4));

        /// <inheritdoc cref="CreateG(string)"/>
        public GameObject CreateG<T1, T2, T3, T4, T5>(string name = null) =>
            CreateG(name, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));

        /// <inheritdoc cref="CreateG(string)"/>
        public GameObject CreateG<T1, T2, T3, T4, T5, T6>(string name = null) => CreateG(name, typeof(T1), typeof(T2),
            typeof(T3), typeof(T4), typeof(T5), typeof(T6));

        /// <inheritdoc cref="CreateG(string)"/>
        public GameObject CreateG<T1, T2, T3, T4, T5, T6, T7>(string name = null) => CreateG(name, typeof(T1),
            typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));

        /// <inheritdoc cref="CreateG(string)"/>
        public GameObject CreateG<T1, T2, T3, T4, T5, T6, T7, T8>(string name = null) => CreateG(name, typeof(T1),
            typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8));
        
        /// <summary>
        /// Creates a new disabled game object with the specified name and components.
        /// </summary>
        /// <param name="name">Name of the game object.</param>
        /// <param name="components">Components to add to the game object.</param>
        /// <returns>The created game object.</returns>
        public GameObject CreateG(string name, params Type[] components)
        {
            var g = new GameObject(name);
            g.transform.SetParent(_sceneRoot.transform);

            foreach (var component in components) g.AddComponent(component);
            return g;
        }
        
        /// <summary>
        /// Creates a new game object with a component and returns the component.
        /// </summary>
        /// <param name="name">Name of the game object.</param>
        /// <typeparam name="T">Type of the component to add.</typeparam>
        /// <returns>The created component.</returns>
        public T CreateGWith<T>(string name = null) where T : MonoBehaviour
        {
            var g = CreateG(name);
            return g.AddComponent<T>();
        }

        #endregion

        /// <summary>
        /// Disposes the root injector and all game objects created by this helper.
        /// </summary>
        public void Dispose()
        {
            Object.DestroyImmediate(_rootInjector.gameObject);
            Object.DestroyImmediate(_sceneRoot);
        }
    }
}