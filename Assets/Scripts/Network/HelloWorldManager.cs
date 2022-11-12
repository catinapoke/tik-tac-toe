using Unity.Netcode;
using UnityEngine;

namespace GameNetwork
{
    public class HelloWorldManager : MonoBehaviour
    {
        private NetworkManager _manager;

        private void Start()
        {
            _manager = NetworkManager.Singleton;
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            // Game isn't started in any of modes or Server disconnected
            if (!_manager.IsServer && !_manager.IsClient)
            {
                Debug.Log($"Not server and not client: IsHost = {_manager.IsHost}");
                DrawStartButtons();
            }
            else
            {
                DrawStatus();
                DrawMoveLocalPlayerButton();
            }

            GUILayout.EndArea();
        }

        private void DrawStartButtons()
        {
            if (GUILayout.Button("Host")) _manager.StartHost();
            if (GUILayout.Button("Server")) _manager.StartServer();
            if (GUILayout.Button("Client")) _manager.StartClient();
        }

        private void DrawStatus()
        {
            string mode = _manager.IsHost ? "Host" :
                _manager.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " + _manager.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
        }

        private void DrawMoveLocalPlayerButton()
        {
            if (_manager.IsServer && GUILayout.Button("Move all players"))
            {
                var clients = _manager.ConnectedClients;
                foreach (var client in clients)
                {
                    client.Value.PlayerObject.GetComponent<HelloWorldPlayer>().Move();
                }
            }

            if (_manager.IsClient && GUILayout.Button("Move local player"))
            {
                NetworkObject localPlayer = _manager.SpawnManager.GetLocalPlayerObject();
                HelloWorldPlayer player = localPlayer.GetComponent<HelloWorldPlayer>();
                player.Move();
            }
        }
    }
}