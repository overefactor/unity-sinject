using Overefactor.DI.Runtime.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Overefactor.DI.Samples.SceneLoading
{
    public class ControlGUI : MonoBehaviour
    {
        [SerializeField] private string scene1 = "Scene_1";
        [SerializeField] private string scene2 = "Scene_2";
        [SerializeField] private string scene3 = "Scene_3";
        
        [SInject] private IGameManager _gameManager;
        
        private bool _sceneToggle;
        
        private void OnGUI()
        {
            if (GUILayout.Button("Load Scene"))
            {
                SceneManager.LoadScene(_sceneToggle ? scene1 : scene2, LoadSceneMode.Single);
                _sceneToggle = !_sceneToggle;
            }

            if (GUILayout.Button("Load Additive Scene")) SceneManager.LoadScene(scene3, LoadSceneMode.Additive);

            if (GUILayout.Button("Start Game")) _gameManager.StartGame();
        }
    }
}
