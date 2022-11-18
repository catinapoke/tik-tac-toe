using Unity.Netcode;

namespace GameNetwork
{
    public static class NetworkClientsExtensions
    {
        public static void SendOthers(this ClientRpcParams @params)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                @params.Send = default(ClientRpcParams).Send;
                return;
            }

            ulong[] ids = new ulong[NetworkManager.Singleton.ConnectedClientsIds.Count - 1];

            int i = 0;
            foreach (var id in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if(id == NetworkManager.Singleton.LocalClientId) continue;
                ids[i++] = id;
            }

            @params.Send = new ClientRpcSendParams
            {
                TargetClientIds = ids
            };
        }
    }
}