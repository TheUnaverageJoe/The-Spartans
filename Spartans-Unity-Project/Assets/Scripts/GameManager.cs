using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

using Spartans.Players;

namespace Spartans{
    public class GameManager : MonoBehaviour
    {
        private List<Player> _players = new List<Player>();
        private bool _pregame = true;
        private bool _inGame = false;
        [SerializeField] private GameObject connectionUI;
        private TMP_InputField _input;

        void Start(){

            if(NetworkManager.Singleton.IsServer) NetworkManager.Singleton.OnClientConnectedCallback += AddPlayer;

            _input = connectionUI.GetComponentInChildren<TMP_InputField>();
        }

        void FixedUpdate(){
            if(_pregame){
                connectionUI.SetActive(true);
            }else{
                connectionUI.SetActive(false);
            }
        }

        private void AddPlayer(ulong id){
            {
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
        }

        public void StartServer(){
            NetworkManager.Singleton.StartServer();
        }
        public void StartHost(){
            NetworkManager.Singleton.StartHost();
        }
        public void JoinGame(){
            
            NetworkManager.Singleton.StartClient();
        }
    }
}