using System;
using Unity.Netcode;

namespace GameNetwork
{
    public class PlayerEntity : NetworkBehaviour
    {
        public NetworkVariable<ItemType> Type;
        public NetworkVariable<int> BoosterCount;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            // Request player state
        }
    }
}