using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

using Spartans.UI;
using Spartans.Players;

namespace Spartans{
    public class GameManager : MonoBehaviour
    {
        PlayerCanvasManager _playerCanvasManager;
        //private List<Player> _players = new List<Player>();
        [SerializeField] private GameObject connectionUI;
        private TMP_InputField _input;
        private UnityTransport connection;
        //private PanelManager.ConnectionInfo info;
        public UnityEvent _joinedGame;

        void Start(){
            //NetworkManager.Singleton.OnClientConnectedCallback += AddPlayer;
            //NetworkManager.Singleton.OnClientConnectedCallback += RequestAddPlayerServerRPC;

            _joinedGame = new UnityEvent();
            _joinedGame.AddListener(JoinGameCallback);

            _playerCanvasManager = FindObjectOfType<PlayerCanvasManager>();
            _playerCanvasManager.Init();
            
            _input = connectionUI.GetComponentInChildren<TMP_InputField>();
            connection = NetworkManager.Singleton.GetComponent<UnityTransport>();
        }

        public void StartServer(){
            NetworkManager.Singleton.StartServer();
            _joinedGame.Invoke();
        }
        public void StartHost(){
            NetworkManager.Singleton.StartHost();
            _joinedGame.Invoke();
        }
        public void JoinGame(){
            if(_input.text.Length < 7 || _input.text == null){
                connection.ConnectionData.Address = "127.0.0.1";
            }else{
                connection.ConnectionData.Address = _input.text;
            }
            NetworkManager.Singleton.StartClient();
            _joinedGame.Invoke();

        }

        private void JoinGameCallback(){
            connectionUI.gameObject.SetActive(false);
            _playerCanvasManager.OnJoinGame();
        }
    }
}