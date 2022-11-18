using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameNetwork
{
    public class MatchStarter : NetworkBehaviour
    {
        [SerializeField] private Lobby _lobby;
        [SerializeField] private NetworkMatchSettings _matchSettings;
        
        private double _startTime = -1;
        
        public double Countdown => (_startTime <0 ? Double.MaxValue: _startTime - NetworkManager.Singleton.ServerTime.Time);
        public double StartTime => _startTime;
        
        private Action OnStartTimeChange;
        
        public override void OnNetworkSpawn()
        {
            _lobby.PlayersReadiness.OnDictionaryChanged += OnReadyStatusChange;
            base.OnNetworkSpawn();
        }

        public override void OnNetworkDespawn()
        {
            _lobby.PlayersReadiness.OnDictionaryChanged -= OnReadyStatusChange;
            base.OnNetworkDespawn();
        }

        private void OnReadyStatusChange(NetworkDictionaryEvent<ulong, bool> _)
        {
            bool isEveryoneReady = GetLobbyReadyStatus();

            if (!isEveryoneReady)
            {
                _startTime = -1;
                SyncStartClientRpc(-1);
                return;
            }
            
            if(IsServer)
                StartDelayedStart();

            bool GetLobbyReadyStatus()
            {
                foreach (var item in _lobby.PlayersReadiness)
                {
                    if (!item.Value) return false;
                }

                return true;
            }
        }

        private void StartDelayedStart(double delay=5f)
        {
            _startTime = NetworkManager.Singleton.ServerTime.Time + delay;
            SyncStartClientRpc(_startTime);
            StartCoroutine(WaitAndStart());
        }

        [ClientRpc]
        private void SyncStartClientRpc(double startTime)
        {
            _startTime = startTime;
            OnStartTimeChange?.Invoke();
            SaveSettings();
        }

        private IEnumerator WaitAndStart()
        {
            while (Countdown > 0)
            {
                if (_startTime < 0) 
                    yield break;
                
                yield return null;
            }

            if (_startTime < 0) 
                yield break;

            NetworkManager.SceneManager.LoadScene("Match", LoadSceneMode.Single);
        }

        private void SaveSettings()
        {
            PlayerEntity playerEntity = NetworkManager.Singleton.
                LocalClient.PlayerObject.GetComponent<PlayerEntity>();

            if (playerEntity == null)
            {
                Debug.LogError("Can't save settings!");
                return;
            }

            playerEntity.UpdateSettings(_matchSettings.Settings);
        }
    }
}