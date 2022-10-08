using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Spartans
{ 
    public struct PlayerLobbyData : INetworkSerializable, System.IEquatable<PlayerLobbyData>
    {
         public ulong _id;
         public CharacterTypes _type;
         public bool _isReady;
         public LobbySync.Teams _team;

        public PlayerLobbyData(ulong id, LobbySync.Teams team, CharacterTypes type, bool isReady){
            _id = id;
            _type = type;
            _isReady = isReady;
            _team = team;
        }
        public PlayerLobbyData(PlayerLobbyData toClone){
            _id = toClone._id;
            _type = toClone._type;
            _isReady = toClone._isReady;
            _team = toClone._team;
        }

        void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            serializer.SerializeValue(ref _id);
            serializer.SerializeValue(ref _type);
            serializer.SerializeValue(ref _isReady);
            serializer.SerializeValue(ref _team);
        }

        public bool Equals(PlayerLobbyData other)
        {
            if(this._id == other._id && this._isReady == other._isReady &&
               this._team == other._team && this._type == other._type)
            {
                return true;
            }

            return false;
        }
    }
}
