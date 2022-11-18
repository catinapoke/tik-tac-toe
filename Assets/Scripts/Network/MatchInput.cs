using GameNetwork;
using Unity.Netcode;
using UnityEngine;

public class MatchInput : NetworkBehaviour
{
    [SerializeField] private Field _field;
    private MatchSettings Settings => _localPlayer.Settings;

    private NetworkVariable<ItemType> CurrentPlayerType = new NetworkVariable<ItemType>();
    private PlayerEntity _localPlayer;

    public bool IsCurrentPlayerTurn => CurrentPlayerType.Value == _localPlayer.Type.Value;
    public bool IsInitialized => _localPlayer != null; 

    public event NetworkVariable<ItemType>.OnValueChangedDelegate OnTurnChanged
    {
        add => CurrentPlayerType.OnValueChanged += value;
        remove => CurrentPlayerType.OnValueChanged -= value;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _localPlayer = NetworkManager.LocalClient.PlayerObject.GetComponent<PlayerEntity>();

        if (IsServer)
        {
            CurrentPlayerType.Value = ItemType.Circle;
        }
    }

    public void Tap(FieldSlot slot)
    {
        if(!IsCurrentPlayerTurn || !slot.IsAvailable) return;

        (int, int) position = _field.GetSlotPosition(slot);
        TapAndBroadcast(position.Item1, position.Item2);
    }

    private void TapAndBroadcast(int row, int column)
    {
        _field.Tap(row, column, CurrentPlayerType.Value);
        
        if (IsServer)
        {
            ClientRpcParams clientRpcParams = new ClientRpcParams();
            clientRpcParams.SendOthers();

            FieldTapClientRpc(row, column, clientRpcParams);
            SetNextState();
        }
        else
        {
            SentTapServerRpc(row, column);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SentTapServerRpc(int row, int column, ServerRpcParams @params = default)
    {
        if (Settings.PlayerTypes[CurrentPlayerType.Value] != @params.Receive.SenderClientId)
        {
            Debug.LogWarning($"Stopped attempt: current type {CurrentPlayerType.Value} and player {Settings.PlayerTypes[CurrentPlayerType.Value]}, but sender {@params.Receive.SenderClientId} attempted");
            return;
        }
        
        _field.Tap(row, column, CurrentPlayerType.Value);
        SetNextState();
    }

    [ClientRpc]
    private void FieldTapClientRpc(int row, int column, ClientRpcParams @params = default)
    {
        _field.Tap(row, column, CurrentPlayerType.Value);
    }
    
    private void SetNextState()
    {
        if (!NetworkManager.IsServer)
        {
            Debug.LogError("You are not supposed to call SetNextState() from client side!");
            return;
        }

        ItemType prev = CurrentPlayerType.Value;
        CurrentPlayerType.Value = CurrentPlayerType.Value == ItemType.Circle ? ItemType.Cross : ItemType.Circle;
        Debug.Log($"Changed value from {prev} to {CurrentPlayerType.Value}");
    }
}
