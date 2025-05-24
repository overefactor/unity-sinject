using System.Collections.Generic;
using System.Linq;
using Sapo.DI.Runtime.Behaviours;
using Sapo.DI.Runtime.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sapo.DI.Runtime.Core
{
    internal class SInjectionPipeline
    {
        public SInjector Injector { get; }

        private class SInjectionContext
        {
            public SInjector Injector { get; }

            public List<GameObject> GameObjects { get; } = new();
            
            public Dictionary<GameObject, ISInjector> LocalInjectors { get; } = new();
            
            public List<(ISInjectorRegisterHandler, ISInjector)> RegisterHandlers { get; } = new();
            
            public List<(ISInjectorInjectHandler, ISInjector)> InjectHandlers { get; } = new();

            public SInjectionContext(SInjector injector) => Injector = injector;

            public ISInjector GetInjector(GameObject gameObject) =>
                LocalInjectors.GetValueOrDefault(gameObject, Injector);
        }

        public SInjectionPipeline(SInjector injector) => Injector = injector;

        public void Initialize(List<Object> assets, GameObject rootInjectorGameObject)
        {
            var reflectionCache = SInjector.ReflectionCache;
            
            foreach (var asset in assets)
            {
                if (!asset) continue;

                foreach (var registerType in reflectionCache.GetRegisterTypes(asset.GetType()))
                    Injector.Register(registerType, asset);
            }

            foreach (var handler in assets.OfType<ISInjectorRegisterHandler>()) handler.OnRegister(Injector);

            foreach (var asset in assets)
            {
                if (!asset) continue;
                
                Injector.Inject(asset);
            }

            foreach (var handler in assets.OfType<ISInjectorInjectHandler>()) handler.OnInject(Injector);
            
            
            InjectGameObject(rootInjectorGameObject);
        }
        
        

        public void InjectScene(Scene scene) => InjectGameObjects(scene.GetRootGameObjects());

        public void InjectGameObject(GameObject gameObject) => InjectGameObjects(new[] { gameObject });
        
        private void InjectGameObjects(GameObject[] gameObjects)
        {
            var context = new SInjectionContext(Injector);

            foreach (var gameObject in gameObjects) ScanGameObjectForContext(context, gameObject);
            
            ExecuteRegistration(context);
            
            ExecuteInjection(context);
        }
        
        private void ScanGameObjectForContext(SInjectionContext context, GameObject gameObject)
        {
            context.GameObjects.Add(gameObject);
            
            foreach (var injector in gameObject.GetComponentsInChildren<SGameObjectInject>(true))
            {
                if (!injector.CreateLocalInjector) continue;
                
                context.LocalInjectors.Add(injector.gameObject, new SInjector(Injector));
            }
            
            
            foreach (var handler in gameObject.GetComponentsInChildren(typeof(ISInjectorRegisterHandler), true))
            {
                context.RegisterHandlers.Add(((ISInjectorRegisterHandler)handler,
                    context.GetInjector(handler.gameObject)));
            }
            
            foreach (var handler in gameObject.GetComponentsInChildren(typeof(ISInjectorInjectHandler), true))
            {
                context.InjectHandlers.Add(((ISInjectorInjectHandler)handler,
                    context.GetInjector(handler.gameObject)));
            }
        }


        private void ExecuteRegistration(SInjectionContext context)
        {
            var reflectionCache = SInjector.ReflectionCache;
            
            foreach (var gameObject in context.GameObjects)
            foreach (var (componentT, registerT) in reflectionCache.RegistrableComponents)
            foreach (var component in gameObject.GetComponentsInChildren(componentT, true))
            foreach (var type in registerT) 
                context.GetInjector(component.gameObject).Register(type, component);
            
            foreach (var (handler, injector) in context.RegisterHandlers) handler.OnRegister(injector);
        }

        private void ExecuteInjection(SInjectionContext context)
        {
            var reflectionCache = SInjector.ReflectionCache;
            
            foreach (var gameObject in context.GameObjects)
            foreach (var injectableComponentT in reflectionCache.InjectableComponents)
            foreach (var component in gameObject.GetComponentsInChildren(injectableComponentT, true))
                context.GetInjector(component.gameObject).Inject(component);
            
            foreach (var (handler, injector) in context.InjectHandlers) handler.OnInject(injector);
        }
    }
}