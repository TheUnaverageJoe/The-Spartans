using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

using Spartans.Players;

namespace Spartans{
    public class GameManager : MonoBehaviour
    {
        private List<Player> _players = new List<Player>();

        void Start(){
            
            NetworkManager.Singleton.OnClientConnectedCallback += (id) => {
                if(NetworkManager.Singleton.IsServer){
                    Player newPlayer;
                    NetworkManager.Singleton.ConnectedClients[id].PlayerObject.TryGetComponent<Player>(out newPlayer);
                    //if(NetworkManager.Singleton.IsServer){
                    //    newPlayer.Start();
                    //}
                    _players.Add(newPlayer);
                    print("----Players----- ");
                    foreach(Player client in _players){
                        print("Player: " + client.playerName);
                    }
                    print("-----------------");

                }
            };
            
        }

        void FixedUpdate(){
            if(!NetworkManager.Singleton.IsServer) return;
            
            if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient )
            {
                
            }
            else
            {
                //Debug.Log("Hey look I made it");
                
                //NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
                //PlayerMovement player = playerObject.GetComponent<PlayerMovement>();
                //player.Move();
            }
        }
    }
}