using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Spartans
{ 
    public struct PlayerGameData : INetworkSerializable, System.IEquatable<PlayerLobbyData>
    {
         public ulong Id;
         public CharacterTypes Type;
         public Teams Team;
         public static explicit operator PlayerGameData(PlayerLobbyData item){
            return new PlayerGameData(item.Id, item.Team, item.Type);
         }

        public PlayerGameData(ulong id, Teams team, CharacterTypes type){
            Id = id;
            Type = type;
            Team = team;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Id);
            serializer.SerializeValue(ref Type);
            serializer.SerializeValue(ref Team);
        }

        public bool Equals(PlayerLobbyData other)
        {
            if(this.Id == other.Id && this.Team == other.Team && this.Type == other.Type)
            {
                return true;
            }

            return false;
        }
        public override string ToString()
        {
            return $"Player{Id} is a {Type} on {Team} team";
        }
    }
}
