using System.Collections.Generic;
using Overefactor.DI.Runtime.Attributes;
using Overefactor.DI.Runtime.Interfaces;
using UnityEngine;

namespace Overefactor.DI.Samples.CollectionInjection
{
    public class Controller : MonoBehaviour
    {
        [SInject] private ISInjector _injector;
        
        [SInject] private IEntity[] _entities;

        private void Start()
        {
            Debug.Log("Introducing entities injected as IEntity[] using <color=cyan>SInject</color> attribute.");
            foreach (var entity in _entities) entity.Introduce();

            Debug.Log("Introducing entities resolved as IEnumerable<IEntity> using <color=cyan>ISInjector</color>.");
            foreach (var entity in _injector.Resolve<IEnumerable<IEntity>>()) entity.Introduce();
        }
    }
}