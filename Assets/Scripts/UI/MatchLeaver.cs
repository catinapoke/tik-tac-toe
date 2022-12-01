using System.Threading.Tasks;
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
        
        public async static void LeaveStatic()
        {
            NetworkManager.Singleton.Shutdown(true);
            while (NetworkManager.Singleton.ShutdownInProgress)
                await Task.Delay(100);
            
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }
    }
}