using System;

namespace Overefactor.DI.Samples.SceneLoading
{
    public interface IGameManager
    {
        event Action OnGameStart;
        
        void StartGame();

    }
}
