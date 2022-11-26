using System;
using UI;
using UnityEngine;
using UnityEngine.Assertions;

public class MatchFinisher : MonoBehaviour
{
    [SerializeField] private Field _field;
    [SerializeField] private ResultWindow _window;
    [SerializeField] private FieldTaper _taper;

    private void OnValidate()
    {
        Assert.IsNotNull(_field);
        Assert.IsNotNull(_window);
    }
    
    private void OnEnable()
    {
        _field.OnGameFinish += OnGameResult;
    }

    private void OnDisable()
    {
        _field.OnGameFinish -= OnGameResult;
    }

    private void OnGameResult(ItemType? result)
    {
        _window.gameObject.SetActive(true);
        _taper.enabled = false;
        
        GameResult gameResult;
        switch (result)
        {
            case ItemType.Circle:
                gameResult = GameResult.WinCircle;
                break;
            case ItemType.Cross:
                gameResult = GameResult.WinCross;
                break;
            case null:
                gameResult = GameResult.FullField;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(result), result, null);
        }
        
        _window.SetResult(gameResult);
    }
}
