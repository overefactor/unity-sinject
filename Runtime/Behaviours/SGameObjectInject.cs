using System.ComponentModel;
using Sapo.DI.Runtime.Interfaces;
using UnityEngine;

namespace Sapo.DI.Runtime.Behaviours
{
    /// <summary>
    /// A GameObject Inject is a component that injects dependencies in the GameObject during game object instantiation.
    /// </summary>
    [HelpURL("https://github.com/samopoul/Sapo.DI")]
    [DisplayName("GameObject Inject")]
    [AddComponentMenu("Sapo/DI/GameObject Inject")]
    [DisallowMultipleComponent]
    public sealed class SGameObjectInject : MonoBehaviour, ISInjectorRegisterHandler
    {
        [SerializeField] private bool localInjector = false;
        internal bool CreateLocalInjector
        {
            get => localInjector;
            set => localInjector = value;
        }

        private bool _isInjected;
        
#if UNITY_EDITOR
        internal ISInjector LocalInjector { get; private set; }
#endif
        
        void ISInjectorRegisterHandler.OnRegister(ISInjector injector)
        {
            _isInjected = true;
#if UNITY_EDITOR
            LocalInjector = injector;
#endif
        }

        private void Awake()
        {
            if (_isInjected)
            {
#if !UNITY_EDITOR
                Destroy(this);
#endif
                return;
            }

            var injector = SRootInjector.FindOrCreateSingleton();
            injector.InjectGameObject(gameObject);
            
            
#if !UNITY_EDITOR
            Destroy(this);
#endif
        }

    }
}