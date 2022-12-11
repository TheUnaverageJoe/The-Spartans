using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spartans
{
    public static class SavedData
    {
        private static List<PlayerGameData> _playerDataFromLobby = new List<PlayerGameData>();


        public static void SavePlayerLobbyData(PlayerLobbyData[] _playersData){
            if(_playerDataFromLobby.Count > 0){
                _playerDataFromLobby.Clear();
                Debug.LogWarning("emptied saved data from playerDataFromLobby variable");
            }
            foreach(var item in _playersData){
                _playerDataFromLobby.Add((PlayerGameData)item);
            }
        }

        public static List<PlayerGameData> LoadDataFromPlayerLobby(){
            return _playerDataFromLobby;
        }


    }
}
