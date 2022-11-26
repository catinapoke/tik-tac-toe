using System;
using GameUI;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ResultWindow : MonoBehaviour
    {
        [SerializeField] private TMP_Text _resultText;

        public void SetResult(GameResult result)
        {
            _resultText.text = GetResultString(result);
        }

        public void GoToMenu()
        {
            MatchLeaver.LeaveStatic();
        }

        private string GetResultString(GameResult result)
        {
            switch (result)
            {
                case GameResult.WinCircle:
                    return "Circle player won!";
                case GameResult.WinCross:
                    return "Cross player won!";
                case GameResult.FullField:
                    return "It's a draw!";
                case GameResult.PlayerDisconnected:
                    return "Opponent disconnected!";
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        }
    }

    public enum GameResult
    {
        WinCircle,
        WinCross,
        FullField,
        PlayerDisconnected
    }
}
