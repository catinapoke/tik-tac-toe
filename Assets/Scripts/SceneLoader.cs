using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private bool _loadSceneAtStart;
        [SerializeField] private string sceneName;

        private void Start()
        {
            if(_loadSceneAtStart)
                LoadScene(sceneName);
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}