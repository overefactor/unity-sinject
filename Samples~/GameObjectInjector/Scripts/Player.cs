using Overefactor.DI.Runtime.Attributes;
using UnityEngine;

namespace Overefactor.DI.Samples.GameObjectInjector
{
    public class Player : MonoBehaviour
    {
        [SInject] private IPlayerData _playerData;
        
        private void Start()
        {
            name = $"Player ({_playerData.PlayerName})";
        }
    }
}
