using System;
using GameNetwork;
using GameUI.Toggle;
using Unity.Netcode;
using UnityEngine;

public class LobbyWindow : MonoBehaviour
{
    [SerializeField] private IndexedToggleGroup _timer;
    [SerializeField] private IndexedToggleGroup _booster;
    [SerializeField] private IndexedToggleGroup _timerAction;
    [SerializeField] private NetworkMatchSettings _settings;
    
    private void OnEnable()
    {
        _timer.OnToggleClick += OnTimerChanged;
        _booster.OnToggleClick += OnBoosterChanged;
        _timerAction.OnToggleClick += OnTimerActionChanged;

        if (!NetworkManager.Singleton.IsServer)
            _settings.OnUpdate += OnServerSideUpdate;
    }

    private void OnDisable()
    {
        _timer.OnToggleClick -= OnTimerChanged;
        _booster.OnToggleClick -= OnBoosterChanged;
        _timerAction.OnToggleClick -= OnTimerActionChanged;
        
        if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsServer)
            _settings.OnUpdate -= OnServerSideUpdate;
    }

    private void UpdateView()
    {
        _timer.TryForceToggle(_settings.TimerLength);
        _booster.TryForceToggle(_settings.IsBoosterAvailable?1:-1);
        _timerAction.TryForceToggle((int) _settings.TimerAction);
    }
    
    private void OnServerSideUpdate(SettingsUpdate data)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            return;
        }
        
        Debug.Log($"Updating value from server: {data.Type.ToString()} {data.Value}");
        switch (data.Type)
        {
            case SettingsUpdate.UpdateType.TimerLength:
                _timer.TryForceToggle(_settings.TimerLength);
                break;
            case SettingsUpdate.UpdateType.Booster:
                _booster.TryForceToggle(_settings.IsBoosterAvailable?1:-1);
                break;
            case SettingsUpdate.UpdateType.FinishAction:
                _timerAction.TryForceToggle((int) _settings.TimerAction);
                break;
            case SettingsUpdate.UpdateType.Full:
                UpdateView();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void OnTimerChanged(int choice)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            _timer.TryForceToggle(_settings.TimerLength);
            _timerAction.gameObject.SetActive(_settings.TimerLength != -1 && _booster.IsValueActive(1));
            return;
        }

        _settings.TimerLength = choice;
        _timerAction.gameObject.SetActive(_settings.TimerLength != -1 && _booster.IsValueActive(1));
        _settings.UpdateRequestClientRpc(new SettingsUpdate()
        {
            Type = SettingsUpdate.UpdateType.TimerLength,
            Value = choice
        });
    }
    
    private void OnBoosterChanged(int choice)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            _booster.TryForceToggle(_settings.IsBoosterAvailable ? 1 : -1);
            _timerAction.gameObject.SetActive(_settings.IsBoosterAvailable && !_timer.IsValueActive(0));
            return;
        }
        
        _settings.IsBoosterAvailable = choice != -1;
        _timerAction.gameObject.SetActive(choice != -1 && !_timer.IsValueActive(0));
        _settings.UpdateRequestClientRpc(new SettingsUpdate()
        {
            Type = SettingsUpdate.UpdateType.Booster,
            Value = choice
        });
    }
    
    private void OnTimerActionChanged(int choice)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            _timerAction.TryForceToggle((int)_settings.TimerAction);
            return;
        }
        
        _settings.TimerAction = (TimerFinishAction) choice;
        _settings.UpdateRequestClientRpc(new SettingsUpdate()
        {
            Type = SettingsUpdate.UpdateType.FinishAction,
            Value = choice
        });
    }
}
