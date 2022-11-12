using Unity.Netcode;
using UnityEngine;

namespace GameUI
{
    public class MatchLeaver : MonoBehaviour
    {
        public void Leave()
        {
            NetworkManager.Singleton.Shutdown();
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }
    }
}