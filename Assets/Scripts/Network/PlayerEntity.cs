using Unity.Netcode;
using UnityEngine;

namespace GameNetwork
{
    public class PlayerEntity : NetworkBehaviour
    {
        private MatchSettings _settings;
        
        public NetworkVariable<ItemType> Type = new NetworkVariable<ItemType>(ItemType.Circle, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> BoosterCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public MatchSettings Settings => _settings;

        public void UpdateSettings(MatchSettings settings)
        {
            _settings = settings;
            BoosterCount.Value = settings.IsBoosterAvailable ? 1 : 0;

            var type = settings.GetPlayerType(OwnerClientId);
            if (type == null)
            {
                Debug.LogError("Can't get type of player!");
                Type.Value = ItemType.Circle;
            }
            else
            {
                Type.Value = type.Value;
            }
            
            Debug.Log($"UpdateSettings({OwnerClientId}), Type: {Type.Value}, Boosters: {BoosterCount.Value}");
            gameObject.name = $"Player_{Type.Value}";
        }
    }
}