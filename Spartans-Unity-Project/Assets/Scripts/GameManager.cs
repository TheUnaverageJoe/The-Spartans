using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UI;
using TMPro;

using Spartans.UI;
using Spartans.Players;

namespace Spartans{
    public class GameManager : MonoBehaviour
    {
        PlayerCanvasManager _playerCanvasManager;
        private List<Player> _players = new List<Player>();
        private bool _pregame = true;
        private bool _inGame = false;
        [SerializeField] private GameObject connectionUI;
        private TMP_InputField _input;
        private UnityTransport connection;
        //private PanelManager.ConnectionInfo info;

        void Start(){
            //NetworkManager.Singleton.OnClientConnectedCallback += AddPlayer;
            //NetworkManager.Singleton.OnClientConnectedCallback += RequestAddPlayerServerRPC;

            _playerCanvasManager = FindObjectOfType<PlayerCanvasManager>();
            _playerCanvasManager.Init();
            _input = connectionUI.GetComponentInChildren<TMP_InputField>();
            connection = NetworkManager.Singleton.GetComponent<UnityTransport>();
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
            print("add player called");
            if(NetworkManager.Singleton.IsServer){
                Player newPlayer;
                NetworkManager.Singleton.ConnectedClients[id].PlayerObject.TryGetComponent<Player>(out newPlayer);
                //if(NetworkManager.Singleton.IsServer){
                //    newPlayer.Start();
                //}
                _players.Add(newPlayer);
                ////info = new PanelManager.ConnectionInfo(newPlayer.name, 8, "Client");
                //print(_playerCanvasManager);
                //print(info.name + " " + info.ping + " " + info.connectedAs);
                ////PlayerCanvasManager.GetPanelManager().AddActiveConnection(info);
                //PanelManager.NewConnection.Invoke(info);
                print("----Players----- ");
                foreach(Player client in _players){
                    print("Player: " + client.playerName);
                }
                print("-----------------");
            }else if(!NetworkManager.Singleton.IsServer && NetworkManager.Singleton.IsClient){
                RequestAddPlayerServerRPC(id);
            }
        }
        [ServerRpc]
        private void RequestAddPlayerServerRPC(ulong id){
            Player newPlayer;
            NetworkManager.Singleton.ConnectedClients[id].PlayerObject.TryGetComponent<Player>(out newPlayer);
            PanelManager.ConnectionInfo info = new PanelManager.ConnectionInfo(newPlayer.name, 8, "Client");
            print("Got this far");

            ReplyAddPlayerClientRpc(info);
        }
        [ClientRpc]
        private void ReplyAddPlayerClientRpc(PanelManager.ConnectionInfo info){//Player player parameter 
            //_players.Add(player);  if we pass Player
            //PanelManager.NewConnection.Invoke(info);
            PlayerCanvasManager.GetPanelManager().AddActiveConnection(info);
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
                connection.ConnectionData.Address = "127.0.0.1";
            }else{
                connection.ConnectionData.Address = _input.text;
            }
            NetworkManager.Singleton.StartClient();
            _pregame = false;
            _inGame = true;
        }
    }
}