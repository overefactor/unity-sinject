using Overefactor.DI.Runtime.Attributes;
using UnityEngine;

namespace Overefactor.DI.Samples.GameObjectInjector
{
    [SRegister(typeof(IPlayerData))]
    public class PlayerData : MonoBehaviour, IPlayerData
    {
        [SerializeField] private string playerName;

        public string PlayerName => playerName;
    }
}
