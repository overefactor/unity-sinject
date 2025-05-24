using System;
using System.Collections.Generic;
using System.ComponentModel;
using Sapo.DI.Runtime.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Sapo.DI.Runtime.Behaviours
{
    /// <summary>
    /// A root injector that initializes the dependency injection system and injects dependencies in the scene.
    /// </summary>
    [HelpURL("https://github.com/samopoul/Sapo.DI")]
    [DisplayName("Root Injector")]
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public sealed class SRootInjector : MonoBehaviour
    {
        private static SRootInjector _instance;

        private SInjectionPipeline _injectionPipeline = new SInjectionPipeline(new SInjector());
        
        [SerializeField] private bool makePersistent = true;
        [SerializeField] private List<Object> assetsToRegister = new();
        
        internal SInjector Injector => _injectionPipeline.Injector;

        internal bool MakePersistent
        {
            get => makePersistent;
            set => makePersistent = value;
        }

        private void Awake() => Initialize();

        private void Initialize()
        {
            if (_instance != null)
            {
                if (_instance == this) return;
                
                Destroy(gameObject);
                return;
            }
            
            if (makePersistent) DontDestroyOnLoad(gameObject);
            _instance = this;
            
            foreach (var rootInjector in FindObjectsOfType<SRootInjector>(true))
            {
                if (rootInjector == this) continue;
                
                Destroy(rootInjector);
            }

            _injectionPipeline.Initialize(assetsToRegister, gameObject);
        }
        
        internal void InjectScene(Scene scene)
        {
            Initialize();
            
            _injectionPipeline.InjectScene(scene);
        }
        
        internal void InjectGameObject(GameObject obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            
            Initialize();
            
            _injectionPipeline.InjectGameObject(obj);
        }

        internal static SRootInjector FindOrCreateSingleton()
        {
            var i = FindObjectOfType<SRootInjector>();
            if (i != null) return i;

            var g = new GameObject("Root Injector");
            DontDestroyOnLoad(g);
            return g.AddComponent<SRootInjector>();
        }
        
        internal static SRootInjector FindSingleton() => FindObjectOfType<SRootInjector>();
    }
}