using Overefactor.DI.Runtime.Attributes;
using UnityEngine;

namespace Overefactor.DI.Samples.Testing.Runtime
{
    public class Player : MonoBehaviour
    {
        [SInject] private IHealth _health;
        
        public void TakeDamage(int damage)
        {
            _health.Value -= damage;
        }

    }
}