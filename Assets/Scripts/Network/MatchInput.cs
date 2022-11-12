using System.Collections;
using System.Collections.Generic;
using GameNetwork;
using Unity.Netcode;
using UnityEngine;

public class MatchInput : NetworkBehaviour
{
    [SerializeField] private Field _field;
    private MatchSettings Settings;

    private NetworkVariable<ItemType> CurrentPlayerType = new NetworkVariable<ItemType>();

    public void Tap(FieldSlot slot)
    {
        if(Settings.PlayerTypes[CurrentPlayerType.Value] != NetworkManager.LocalClient.ClientId) return;

        (int, int) position = _field.GetSlotPosition(slot);
        SentTapServerRpc(position.Item1, position.Item2);
    }

    [ServerRpc]
    private void SentTapServerRpc(int row, int column, ServerRpcParams @params = default)
    {
        if(Settings.PlayerTypes[CurrentPlayerType.Value] != @params.Receive.SenderClientId) return;
        
        _field.Tap(row, column, CurrentPlayerType.Value);
        SetNextState();
    }
    
    private void SetNextState()
    {
        if (NetworkManager.IsServer)
        {
            Debug.LogError("You are not supposed to call SetNextState() from client side!");
            return;
        }
        
        CurrentPlayerType.Value = CurrentPlayerType.Value == ItemType.Circle ? ItemType.Cross : ItemType.Circle;
    }
}
