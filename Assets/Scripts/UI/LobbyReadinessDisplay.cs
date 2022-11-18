using GameNetwork;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    [RequireComponent(typeof(Button))]
    public class LobbyReadinessDisplay : MonoBehaviour
    {
        [SerializeField] private Lobby _lobby;

        [Header("Appearance")] [SerializeField] [ColorUsage(false)]
        private Color _readyColor;

        [SerializeField] [ColorUsage(false)] private Color _notReadyColor;
        [SerializeField] private string _bothReadyText = "Ready";
        [SerializeField] private string _localPlayerReadyText = "Waiting opponent";
        [SerializeField] private string _opponentReadyText = "Opponent waits you";
        [SerializeField] private string _nobodyReadyText = "Not ready";

        private Button _button;
        private TMP_Text _text;

        private bool IsLocalPlayerReady => _lobby.LocalPlayerReadiness;
        private bool IsOtherPlayerReady => _lobby.OpponentReadiness;
        
        private void Start()
        {
            _button = GetComponent<Button>();
            _text = GetComponentInChildren<TMP_Text>();

            UpdateState(false, false);
        }

        private void OnEnable()
        {
            _lobby.OnOpponentChangedState += OnOpponentChangedReadiness;
        }

        private void OnDisable()
        {
            _lobby.OnOpponentChangedState -= OnOpponentChangedReadiness;
        }

        public void ChangeReadiness()
        {
            if(NetworkManager.Singleton.IsServer && NetworkManager.Singleton.ConnectedClientsIds.Count < 2) return;
            
            bool currentState = _lobby.LocalPlayerReadiness;
            _lobby.RequestSwitchReadyStatus(!currentState);
            UpdateState(!currentState, IsOtherPlayerReady);
        }

        private void OnOpponentChangedReadiness()
        {
            UpdateState(IsLocalPlayerReady, IsOtherPlayerReady);
        }

        private void UpdateState(bool isLocalPlayerReady, bool isOtherPlayerReady)
        {
            _button.image.color = isLocalPlayerReady ? _readyColor : _notReadyColor;

            if (_text != null)
            {
                switch (isOtherPlayerReady)
                {
                    case true:
                        _text.text = isLocalPlayerReady ? _bothReadyText : _opponentReadyText;
                        break;
                    case false:
                        _text.text = isLocalPlayerReady ? _localPlayerReadyText : _nobodyReadyText;
                        break;
                }
            }
        }
    }
}