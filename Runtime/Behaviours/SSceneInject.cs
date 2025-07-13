using System.ComponentModel;
using Overefactor.DI.Runtime.Interfaces;
using UnityEngine;

namespace Overefactor.DI.Runtime.Behaviours
{
    /// <summary>
    /// Scene Inject is a component that injects an entire scene during a scene load.
    /// </summary>
    [HelpURL("https://github.com/samopoul/Sapo.DI")]
    [DisplayName("Scene Inject")]
    [AddComponentMenu("Sapo/DI/Scene Inject")]
    [DisallowMultipleComponent]
    public sealed class SSceneInject : MonoBehaviour, ISInjectorRegisterHandler
    {
        private enum InjectionMethod
        {
            Awake = 0,
            OnEnable = 1,
            Script = 2
        }
        
        [SerializeField] private InjectionMethod injectOn = InjectionMethod.Awake;
        
        private bool _isInjected;
        
        void ISInjectorRegisterHandler.OnRegister(ISInjector injector) => _isInjected = true;
        
        private void Awake()
        {
            if (injectOn != InjectionMethod.Awake) return;
            
            InjectOnce();
        }
        
        private void OnEnable()
        {
            if (injectOn != InjectionMethod.OnEnable) return;
            
            InjectOnce();
        }
        
        public void Inject()
        {
            if (injectOn != InjectionMethod.Script) return;
            
            InjectOnce();
        }
        
        private void InjectOnce()
        {
            if (_isInjected)
            {
                Debug.LogError("[Sapo.DI] Scene already injected. Make sure you have only one SSceneInject per scene.");
                Destroy();
                return;
            }
            
            InjectAndDestroy();
        }
        
        private void InjectAndDestroy()
        {
            var injector = SRootInjector.FindOrCreateSingleton();;
            injector.InjectScene(gameObject.scene);
            Destroy();
        }

        private void Destroy()
        {
            if (GetComponent<SRootInjector>() == null) Destroy(gameObject);
            else Destroy(this);
        }
    }
}