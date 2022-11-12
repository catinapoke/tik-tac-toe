using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameUI
{
    public class MenuWindow : MonoBehaviour
    {
        [SerializeField] private TMP_Text _port;
        [SerializeField] private TMP_Text _ip;

        private NetworkManager _manager;
        private UnityTransport _transport;

        private void Start()
        {
            _manager = NetworkManager.Singleton;
            NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
            _transport = _manager.NetworkConfig.NetworkTransport as UnityTransport;

            if (_manager == null) Debug.LogError("[MenuWindow] Manager is null!");
            if (_transport == null) Debug.LogError("[MenuWindow] Transport is null!");
        }

        public void StartServer()
        {
            int port;
            // TextMeshPro adds empty symbol with id 8203 that gives a FormatException
            if (!int.TryParse(_port.text.TrimEnd((char) 8203), out port))
            {
                Debug.LogError("Can't parse port");
                return;
            }

            _transport.SetConnectionData(
                "127.0.0.1", // The IP address is a string
                (ushort) port, // The port number is an unsigned short
                "0.0.0.0" // The server listen address is a string.
            );

            if (_manager.StartHost())
            {
                // Start controlling callbacks of client load
                // Load lobby scene
                Debug.Log($"Started as host with port {port}");
                NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
            }
            else
            {
                Debug.LogError("Can't host server!");
            }
        }

        public void JoinServer()
        {
            UnityTransport transport = _manager.NetworkConfig.NetworkTransport as UnityTransport;

            if (transport == null)
            {
                Debug.LogError("Wrong transport type - can't set connection data!");
                return;
            }

            string[] ip_port = _ip.text.TrimEnd((char) 8203).Split(":");

            if (ip_port.Length != 2)
            {
                Debug.LogError("Can't parse ip - wrong length!");
                return;
            }

            transport.SetConnectionData(ip_port[0], ushort.Parse(ip_port[1]));

            if (_manager.StartClient())
            {
                // Load lobby scene
                Debug.Log($"Connected as client to {ip_port[0]}:{ip_port[1]}");
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Lobby");
            }
            else
            {
                Debug.LogError("Can't join server!");
            }
        }

        public void Exit()
        {
            Debug.Log("Clicked exit button");
            Application.Quit();
        }
        
        private static void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            if (NetworkManager.Singleton.ConnectedClients.Count >= 2)
            {
                response.Approved = false;
                Debug.Log($"Client {request.ClientNetworkId} was disapproved as count is {NetworkManager.Singleton.ConnectedClients.Count}");
                return;
            }

            // The client identifier to be authenticated
            var clientId = request.ClientNetworkId;

            // Additional connection data defined by user code
            var connectionData = request.Payload;

            // Your approval logic determines the following values
            response.Approved = true;
            response.CreatePlayerObject = true;

            // The prefab hash value of the NetworkPrefab, if null the default NetworkManager player prefab is used
            response.PlayerPrefabHash = null;

            // Position to spawn the player object (if null it uses default of Vector3.zero)
            response.Position = Vector3.zero;

            // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
            response.Rotation = Quaternion.identity;

            // If additional approval steps are needed, set this to true until the additional steps are complete
            // once it transitions from true to false the connection approval response will be processed.
            response.Pending = false;

            Debug.Log($"Client {request.ClientNetworkId} was approved");
        }
    }
}