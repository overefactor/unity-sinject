using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Sapo.DI.Runtime.Attributes;
using Sapo.DI.Runtime.Common;
using Sapo.DI.Runtime.Core;
using Sapo.DI.Runtime.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;
using Component = UnityEngine.Component;
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
        [SerializeField] private bool makePersistent = true;
        internal bool MakePersistent
        {
            get => makePersistent;
            set => makePersistent = value;
        }
        
        [SerializeField] private List<Object> assetsToRegister = new();

        private static SRootInjector _instance;
        private bool _registerSelfComponents;
        
        private readonly SInjector _injector = new();
        
        internal SInjector Injector => _injector;

        private void Awake()
        {
            Initialize();
        }

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
            
            foreach (var asset in assetsToRegister)
            {
                if (asset == null) continue;
                if (!asset.GetType().IsDefinedWithAttribute<SRegister>(out var sRegister)) continue;

                _injector.Register(sRegister.Type ?? asset.GetType(), asset);
                if (!sRegister.RegisterAllInterfaces) continue;
                
                foreach (var interfaceType in asset.GetType().GetInterfaces())
                    _injector.Register(interfaceType, asset);
            }

            foreach (var handler in assetsToRegister.OfType<ISInjectorRegisterHandler>()) handler.OnRegister(_injector);
            
            _injector.PerformSelfInjection();

            foreach (var handler in assetsToRegister.OfType<ISInjectorInjectHandler>()) handler.OnInject(_injector);

            _registerSelfComponents = true;
        }
        
        internal void InjectScene(Scene scene)
        {
            Initialize();
            
            var reflectionCache = SInjector.ReflectionCache;

            var localInjectors = new Dictionary<GameObject, ISInjector>();
            var registerHandlers = new List<(GameObject, ISInjectorRegisterHandler)>();
            var injectHandlers = new List<(GameObject, ISInjectorInjectHandler)>();
            var roots = scene.GetRootGameObjects();
            
            foreach (var root in roots)
            {
                foreach (var injector in root.GetComponentsInChildren<SGameObjectInject>(true)
                             .Where(i => i.CreateLocalInjector))
                    localInjectors.Add(injector.gameObject, new SInjector(_injector));
                
                foreach (var (componentT, registerT) in reflectionCache.RegistrableComponents)
                foreach (var component in root.GetComponentsInChildren(componentT, true))
                {
                    var injector = localInjectors.GetValueOrDefault(component.gameObject, _injector);
                    foreach (var type in registerT) injector.Register(type, component);
                }

                foreach (var gTransform in root.GetComponentsInChildren<Transform>(true))
                {
                    registerHandlers.AddRange(gTransform.GetComponents<ISInjectorRegisterHandler>()
                        .Select(handler => (gTransform.gameObject, handler)));

                    injectHandlers.AddRange(gTransform.GetComponents<ISInjectorInjectHandler>()
                        .Select(handler => (gTransform.gameObject, handler)));
                }
            }


            if (_registerSelfComponents)
            {
                foreach (var (componentT, registerT) in reflectionCache.RegistrableComponents)
                foreach (var component in GetComponents(componentT))
                foreach (var type in registerT)
                    _injector.Register(type, component);
                
                registerHandlers.AddRange(GetComponents<ISInjectorRegisterHandler>().Select(c => (gameObject, c)));
                injectHandlers.AddRange(GetComponents<ISInjectorInjectHandler>().Select(c => (gameObject, c)));
            }

            foreach (var (gameObj, handler) in registerHandlers)
                handler.OnRegister(localInjectors.GetValueOrDefault(gameObj, _injector));
            
            if (_registerSelfComponents)
            {
                foreach (var injectableComponentT in reflectionCache.InjectableComponents)
                foreach (var component in GetComponents(injectableComponentT))
                    _injector.Inject(component);    
            }
            
            foreach (var root in roots)
            foreach (var injectableComponentT in reflectionCache.InjectableComponents)
            foreach (var component in root.GetComponentsInChildren(injectableComponentT, true))
                localInjectors.GetValueOrDefault(component.gameObject, _injector).Inject(component);
            
            foreach (var (gameObj, handler) in injectHandlers) 
                handler.OnInject(localInjectors.GetValueOrDefault(gameObj, _injector));

            _registerSelfComponents = false;
        }
        
        internal void InjectGameObject(GameObject obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            
            Initialize();
            var reflectionCache = SInjector.ReflectionCache;

            var localInjectors = obj.GetComponentsInChildren<SGameObjectInject>(true)
                .Where(s => s.CreateLocalInjector)
                .ToDictionary(i => i.gameObject, _ => (ISInjector)new SInjector(Injector));

            var registerHandlers = obj.GetComponentsInChildren<ISInjectorRegisterHandler>(true);
            var injectHandlers = obj.GetComponentsInChildren<ISInjectorInjectHandler>(true);
            
            foreach (var (componentT, registerT) in reflectionCache.RegistrableComponents)
            foreach (var component in obj.GetComponentsInChildren(componentT, true))
            {
                var injector = localInjectors.GetValueOrDefault(component.gameObject, _injector);
                foreach (var type in registerT) injector.Register(type, component);
            }

            foreach (var handler in registerHandlers) handler.OnRegister(_injector);
            
            foreach (var injectableComponentT in reflectionCache.InjectableComponents)
            foreach (var component in obj.GetComponentsInChildren(injectableComponentT, true))
                localInjectors.GetValueOrDefault(component.gameObject, _injector).Inject(component);
            
            foreach (var handler in injectHandlers) handler.OnInject(_injector);
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