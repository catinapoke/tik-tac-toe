﻿using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace GameNetwork
{
    public class NetworkMatchSettings : NetworkBehaviour
    {
        private MatchSettings _settings;

        public int TimerLength
        {
            get => _settings.TimerLength;
            set => _settings.TimerLength = value;
        }

        public bool IsBoosterAvailable
        {
            get => _settings.IsBoosterAvailable;
            set => _settings.IsBoosterAvailable = value;
        }

        public TimerFinishAction TimerAction
        {
            get => _settings.FinishAction;
            set => _settings.FinishAction = value;
        }

        public event Action<SettingsUpdate> OnUpdate;
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsServer)
            {
                Debug.Log("Sent settings request");
                RequestSettingsServerRpc();
                return;
            }

            _settings = new MatchSettings();
            ItemType type = UnityEngine.Random.Range(0, 2) == 0 ? ItemType.Circle : ItemType.Cross;
            _settings.PlayerTypes[type] = NetworkManager.LocalClient.ClientId;

            NetworkManager.Singleton.OnClientConnectedCallback += AddPlayer;
            NetworkManager.Singleton.OnClientDisconnectCallback += RemovePlayer;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= AddPlayer;
                NetworkManager.Singleton.OnClientDisconnectCallback -= RemovePlayer;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestSettingsServerRpc(ServerRpcParams sendParams = default)
        {
            Debug.Log($"Received settings request from {sendParams.Receive.SenderClientId}");
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[]{sendParams.Receive.SenderClientId}
                }
            };
            
            RequestSettingsClientRpc(_settings, clientRpcParams);
        }
        
        [ClientRpc]
        private void RequestSettingsClientRpc(MatchSettings settings, ClientRpcParams sendParams = default)
        {
            // TODO: Pass clients with new client (AddPlayer is called later then request)
            _settings = settings;
            Debug.Log($"Received settings: {settings.DebugString()}");
        }

        [ClientRpc]
        public void UpdateRequestClientRpc(SettingsUpdate update)
        {
            switch (update.Type)
            {
                case SettingsUpdate.UpdateType.TimerLength:
                    _settings.TimerLength = update.Value;
                    break;
                case SettingsUpdate.UpdateType.Booster:
                    _settings.IsBoosterAvailable = update.Value == 1;
                    break;
                case SettingsUpdate.UpdateType.FinishAction:
                    _settings.FinishAction = (TimerFinishAction) update.Value; 
                    break;
                case SettingsUpdate.UpdateType.Full:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            OnUpdate?.Invoke(update);
        }
        
        private void AddPlayer(ulong playerId)
        {
            if (_settings.PlayerTypes.ContainsKey(ItemType.Circle))
                _settings.PlayerTypes[ItemType.Cross] = playerId;
            else
                _settings.PlayerTypes[ItemType.Circle] = playerId;
        }

        private void RemovePlayer(ulong playerId)
        {
            ItemType type = _settings.PlayerTypes.First(x => x.Value == playerId).Key;
            _settings.PlayerTypes.Remove(type);
        }
    }
}