using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Transports.UTP;

using Spartans.UI;

namespace Spartans{
    public class LobbyManager : NetworkBehaviour
    {
        [SerializeField] private GameObject _gameManagerPrefab;
        [SerializeField] private TMP_InputField _input;
        private UnityTransport connection;

        // Start is called before the first frame update
        void Start()
        {
            print("Lobby scene loaded");
            connection = NetworkManager.Singleton.GetComponent<UnityTransport>();
            NetworkManager.OnClientConnectedCallback += NotifyClientConnected;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            print("Lobby spawned into " + NetworkManager.Singleton.ConnectedHostname);

            if(!IsServer) return;
            GameObject spawn = Instantiate(_gameManagerPrefab);
            //GameManager.Instance.GetComponent<NetworkObject>().Spawn();
            spawn.GetComponent<NetworkObject>().Spawn();
        }

        public void StartServer(){
                NetworkManager.Singleton.StartServer();
                //joinedGame.Invoke();
        }
        public void StartHost(){
            NetworkManager.Singleton.StartHost();
            //joinedGame.Invoke();
        }
        public void JoinGame(){
            if(_input.text.Length < 7 || _input.text == null){
                connection.ConnectionData.Address = "127.0.0.1";
            }else{
                connection.ConnectionData.Address = _input.text;
            }
            NetworkManager.Singleton.StartClient();
            //joinedGame.Invoke();
        }

        public void SelectCharacter(int type){
            GameManager.Instance.requestAssignCharacter(NetworkManager.LocalClientId, (CharacterTypes)type);

        }
        private void NotifyClientConnected(ulong clientID){
            print($"Client {clientID} connected");
        }
        public void LeaveLobby(){
            if(GameManager.Instance != null)
                GameManager.Instance.StopConnection();
        }
    }
}