using System.Collections.Generic;
using System.Text;
using Unity.Netcode;

namespace GameNetwork
{
    public struct MatchSettings  : INetworkSerializable
    {
        public Dictionary<ItemType, ulong> PlayerTypes;
        public int TimerLength;
        public bool IsBoosterAvailable;
        public TimerFinishAction FinishAction;

        public static MatchSettings Create(
            int timerLength = -1, 
            bool isBoosterAvailable = false, 
            TimerFinishAction action = TimerFinishAction.RandomChoice)
        {
            return new MatchSettings()
            {
                PlayerTypes = new Dictionary<ItemType, ulong>(2),
                TimerLength = timerLength,
                IsBoosterAvailable = isBoosterAvailable,
                FinishAction = action
            };
        }

        public ItemType? GetPlayerType(ulong playerId)
        {
            foreach (var pair in PlayerTypes)
            {
                if (pair.Value == playerId)
                    return pair.Key;
            }
            
            return null;
        }
        
        public ItemType? GetPlayerOpponentType(ulong playerId)
        {
            foreach (var pair in PlayerTypes)
            {
                if (pair.Value != playerId)
                    return pair.Key;
            }
            
            return null;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            int count = 0;
            if (serializer.IsWriter)
                count = PlayerTypes.Count;
            
            serializer.SerializeValue(ref count);

            if (serializer.IsReader)
                PlayerTypes = new Dictionary<ItemType, ulong>(count);

            ItemType itemType = ItemType.Circle;
            ulong player = 0;
            
            if (serializer.IsWriter)
            {
                foreach (var pair in PlayerTypes)
                {
                    itemType = pair.Key;
                    player = pair.Value;
                    
                    serializer.SerializeValue(ref itemType);
                    serializer.SerializeValue(ref player);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    serializer.SerializeValue(ref itemType);
                    serializer.SerializeValue(ref player);
                    
                    PlayerTypes.Add(itemType, player);
                }
            }
            
            serializer.SerializeValue(ref TimerLength);
            serializer.SerializeValue(ref IsBoosterAvailable);
            serializer.SerializeValue(ref FinishAction);
        }

        public string DebugString()
        {
            StringBuilder builder = new StringBuilder(64);
            builder.Append("MatchSettings:\n");
            builder.AppendFormat("Timer length is {0}\n", TimerLength);
            builder.AppendFormat("Booster availability is {0}\n", IsBoosterAvailable);
            builder.AppendFormat("Finish action is {0}\n", FinishAction.ToString());
            builder.Append("Players types:\n");
            foreach (var pair in PlayerTypes)
            {
                builder.AppendFormat("{0} - {1}\n", pair.Key.ToString(), pair.Value);
            }

            return builder.ToString();
        }
    }

    public enum TimerFinishAction
    {
        RandomChoice = 0,
        BoosterRemove = 1
    }

    public struct SettingsUpdate  : INetworkSerializable
    {
        public enum UpdateType
        {
            TimerLength = 0,
            Booster = 1,
            FinishAction = 2,
            Full = 3
        }

        public UpdateType Type;
        public int Value;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Type);
            serializer.SerializeValue(ref Value);
        }
    }
}