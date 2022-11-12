using Unity.Netcode;
using UnityEngine;

namespace GameNetwork
{
    public class HelloWorldPlayer : NetworkBehaviour
    {
        private NetworkVariable<Vector3> position = new NetworkVariable<Vector3>();

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsOwner) Move();
        }
    
        private void Update()
        {
            transform.position = position.Value;
        }

        public void Move()
        {
            if (IsServer)
            {
                position.Value = GetRandomPosition();
                // transform.position = position.Value;
            }
            else
            {
                RequestMoveServerRpc();
            }
        }

        [ServerRpc]
        private void RequestMoveServerRpc(ServerRpcParams rpcParams = default)
        {
            position.Value = GetRandomPosition();
        }

        private Vector3 GetRandomPosition()
        {
            return new Vector3(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-2f, 2f), 0);
        }
    }
}
