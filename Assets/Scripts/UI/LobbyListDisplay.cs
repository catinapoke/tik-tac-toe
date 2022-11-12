using System.Text;
using GameNetwork;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace GameUI
{
    public class LobbyListDisplay : MonoBehaviour
    {
        [SerializeField] private Lobby _lobby;
        [SerializeField] private TMP_Text _text;

        private StringBuilder builder;

        private void Awake()
        {
            builder = new StringBuilder(64);

            // Clears after call so I don't unsubscribe
            _lobby.OnInitialized += Init;
        }

        private void Init()
        {
            _lobby.PlayersReadiness.OnDictionaryChanged += UpdateList;
            UpdateList(default);
        }

        private void OnDisable()
        {
            _lobby.PlayersReadiness.OnDictionaryChanged -= UpdateList;
        }

        private void UpdateList(NetworkDictionaryEvent<ulong, bool> _)
        {
            builder.Clear();
            builder.AppendLine("Current lobby list: ");
            foreach (var item in _lobby.PlayersReadiness)
            {
                builder.AppendFormat("{0} {1}\n", item.Key, item.Value);
            }

            _text.text = builder.ToString();
        }
    }
}