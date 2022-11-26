using Unity.Netcode;
using UnityEngine;

namespace GameUI
{
    public class MatchLeaver : MonoBehaviour
    {
        public void Leave()
        {
            LeaveStatic();
        }
        
        public static void LeaveStatic()
        {
            NetworkManager.Singleton.Shutdown();
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }
    }
}