using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Spartans
{ 
    public struct PlayerLobbyData : INetworkSerializable, System.IEquatable<PlayerLobbyData>
    {
         public ulong Id;
         public CharacterTypes Type;
         public bool IsReady;
         public LobbySync.Teams Team;

        public PlayerLobbyData(ulong id, LobbySync.Teams team, CharacterTypes type, bool isReady){
            Id = id;
            Type = type;
            IsReady = isReady;
            Team = team;
        }
        public PlayerLobbyData(PlayerLobbyData toClone){
            Id = toClone.Id;
            Type = toClone.Type;
            IsReady = toClone.IsReady;
            Team = toClone.Team;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Id);
            serializer.SerializeValue(ref Type);
            serializer.SerializeValue(ref IsReady);
            serializer.SerializeValue(ref Team);
        }

        public bool Equals(PlayerLobbyData other)
        {
            if(this.Id == other.Id && this.IsReady == other.IsReady &&
               this.Team == other.Team && this.Type == other.Type)
            {
                return true;
            }

            return false;
        }
        public override string ToString()
        {
            return $"{Id} is a {Type} and is ready: {IsReady} on team {Team}";
        }
    }
}
