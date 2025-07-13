using System;
using Overefactor.DI.Runtime.Attributes;
using UnityEngine;

namespace Overefactor.DI.Samples.SceneLoading
{
    [CreateAssetMenu(menuName = "Sapo/DI/Samples/Scene Loading/New Game Manager")]
    [SRegister(typeof(IGameManager))]
    public class GameManager : ScriptableObject, IGameManager
    {
        public event Action OnGameStart;

        public void StartGame() => OnGameStart?.Invoke();
    }
}
