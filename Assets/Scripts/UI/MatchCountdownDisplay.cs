using System;
using GameNetwork;
using TMPro;
using UnityEngine;

namespace GameUI
{
    [RequireComponent(typeof(TMP_Text))]
    public class MatchCountdownDisplay : MonoBehaviour
    {
        [SerializeField] private MatchStarter _matchStarter;

        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void OnGUI()
        {
            _text.text = _matchStarter.StartTime > 0 ? Math.Max(_matchStarter.Countdown, 0.0).ToString("f1") : "";
        }
    }
}