using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using UnityEngine.UI;
using TMPro;

using Spartans.UI;
using Spartans.Players;

namespace Spartans{
    public class GameManager : NetworkBehaviour
    {
        private List<Player> _players = new List<Player>();
        private bool _pregame = true;
        private bool _inGame = false;
        [SerializeField] private GameObject connectionUI;
        private TMP_InputField _input;
        private UNetTransport connection;

        void Start(){

            if(NetworkManager.Singleton.IsServer){
                NetworkManager.Singleton.OnClientConnectedCallback += AddPlayer;
            }else if(!NetworkManager.Singleton.IsServer && NetworkManager.Singleton.IsClient){
                NetworkManager.Singleton.OnClientConnectedCallback += RequestAddPlayerServerRPC;
            }

            _input = connectionUI.GetComponentInChildren<TMP_InputField>();
            connection = NetworkManager.Singleton.GetComponent<UNetTransport>();
        }

        void FixedUpdate(){
            if(_pregame){
                connectionUI.SetActive(true);
            }
            if(_inGame){
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
                PanelManager.ConnectionInfo info = new PanelManager.ConnectionInfo(newPlayer.name, 8, "Client");
                PanelManager.NewConnection.Invoke(info);
                print("----Players----- ");
                foreach(Player client in _players){
                    print("Player: " + client.playerName);
                }
                print("-----------------");
            }
        }
        [ServerRpc]
        private void RequestAddPlayerServerRPC(ulong id){
            Player newPlayer;
            NetworkManager.Singleton.ConnectedClients[id].PlayerObject.TryGetComponent<Player>(out newPlayer);
            PanelManager.ConnectionInfo info = new PanelManager.ConnectionInfo(newPlayer.name, 8, "Client");

            ReplyAddPlayerClientRpc(info);
        }
        [ClientRpc]
        private void ReplyAddPlayerClientRpc(PanelManager.ConnectionInfo info){//Player player parameter 
            //_players.Add(player);  if we pass Player
            PanelManager.NewConnection.Invoke(info);
        }
        public void StartServer(){
            NetworkManager.Singleton.StartServer();
            _pregame = false;
            _inGame = true;
        }
        public void StartHost(){
            NetworkManager.Singleton.StartHost();
            _pregame = false;
            _inGame = true;
        }
        public void JoinGame(){
            if(_input.text.Length < 7 || _input.text == null){
                connection.ConnectAddress = "127.0.0.1";
            }else{
                connection.ConnectAddress = _input.text;
            }
            NetworkManager.Singleton.StartClient();
            _pregame = false;
            _inGame = true;
        }
    }
}