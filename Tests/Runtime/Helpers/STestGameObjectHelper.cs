using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Overefactor.DI.Tests.Tests.Runtime.Helpers
{
    /// <summary>
    /// A helper class for creating game objects for testing purposes.
    /// </summary>
    public class STestGameObjectHelper : IDisposable
    {
        private readonly GameObject _root;
        private readonly List<GameObject> _gameObjects = new();

        /// <summary>
        /// Creates a new instance of <see cref="STestGameObjectHelper"/>.
        /// </summary>
        public STestGameObjectHelper()
        {
            _root = new GameObject("Disabled Prefab Root");
            _root.SetActive(false);
        }
        
        #region Create Prefab
        
        /// <summary>
        /// Creates a new gameObject which can be used as a prefab. Or by changing parent automatically becomes
        /// active.
        /// </summary>
        /// <param name="name">Name of the game object.</param>
        /// <returns>The created game object.</returns>
        public GameObject CreateP(string name = null) => 
            CreateP(name, Type.EmptyTypes);

        /// <inheritdoc cref="CreateP(string)"/>
        public GameObject CreateP<T1>(string name = null) =>
            CreateP(name, typeof(T1));

        /// <inheritdoc cref="CreateP(string)"/>
        public GameObject CreateP<T1, T2>(string name = null) =>
            CreateP(name, typeof(T1), typeof(T2));
        
        /// <inheritdoc cref="CreateP(string)"/>
        public GameObject CreateP<T1, T2, T3>(string name = null) =>
            CreateP(name, typeof(T1), typeof(T2), typeof(T3));
        
        /// <inheritdoc cref="CreateP(string)"/>
        public GameObject CreateP<T1, T2, T3, T4>(string name = null) =>
            CreateP(name, typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        
        /// <inheritdoc cref="CreateP(string)"/>
        public GameObject CreateP<T1, T2, T3, T4, T5>(string name = null) =>
            CreateP(name, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
        
        /// <inheritdoc cref="CreateP(string)"/>
        public GameObject CreateP<T1, T2, T3, T4, T5, T6>(string name = null) => CreateP(name, typeof(T1), typeof(T2),
            typeof(T3), typeof(T4), typeof(T5), typeof(T6));

        /// <inheritdoc cref="CreateP(string)"/>
        public GameObject CreateP<T1, T2, T3, T4, T5, T6, T7>(string name = null) => CreateP(name, typeof(T1),
            typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));

        /// <inheritdoc cref="CreateP(string)"/>
        public GameObject CreateP<T1, T2, T3, T4, T5, T6, T7, T8>(string name = null) => CreateP(name, typeof(T1),
            typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8));
        
        
        /// <summary>
        /// Creates a new disabled gameObject which can be used as a prefab. Or by changing parent automatically becomes
        /// active.
        /// </summary>
        /// <param name="name">Name of the game object.</param>
        /// <param name="components">Components to add to the game object.</param>
        /// <returns>The created game object.</returns>
        public GameObject CreateP(string name = null, params Type[] components)
        {
            var g = new GameObject(name);
            g.transform.SetParent(_root.transform);
            _gameObjects.Add(g);
            
            foreach (var component in components) g.AddComponent(component);
            return g;
        }
        
        #endregion

        #region Create GameObject

        /// <summary>
        /// Creates a new disabled game object with the specified name and components.
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
        /// Creates a new game object with the specified name and components.
        /// </summary>
        /// <param name="name">Name of the game object.</param>
        /// <param name="components">Components to add to the game object.</param>
        /// <returns>The created game object.</returns>
        public GameObject CreateG(string name, params Type[] components)
        {
            var g = new GameObject(name);
            g.SetActive(false);
            _gameObjects.Add(g);

            foreach (var component in components) g.AddComponent(component);
            return g;
        }
        
        /// <summary>
        /// Creates a new game object with component and returns the component.
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
        /// Destroys all the gameObjects created by this helper.
        /// </summary>
        public void Dispose()
        {
            foreach (var gameObject in _gameObjects)
            {
                if (gameObject == null) return;

                Object.DestroyImmediate(gameObject);
            }

            Object.DestroyImmediate(_root);
        }
    }
}