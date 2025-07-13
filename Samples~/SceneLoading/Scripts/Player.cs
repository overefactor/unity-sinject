using Overefactor.DI.Runtime.Attributes;
using UnityEngine;

namespace Overefactor.DI.Samples.SceneLoading
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private string playerName;

        [SInject] private IGameManager _gameManager;


        private void OnEnable() => _gameManager.OnGameStart += OnGameStart;
        
        private void OnDisable() => _gameManager.OnGameStart -= OnGameStart;

        private void OnGameStart() => Debug.Log($"{playerName} is ready to play!");
    }
}
