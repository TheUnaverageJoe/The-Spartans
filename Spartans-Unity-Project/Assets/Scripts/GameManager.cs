using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Events;

using Spartans.UI;
using Spartans.Players;
using System;

namespace Spartans{
    public class GameManager : MonoBehaviour
    {
        private const string MENU_SCENE_NAME = "MainMenu";
        PlayerCanvasManager _playerCanvasManager;
        //private List<Player> _players = new List<Player>();
        [SerializeField] private GameObject connectionUI;
        [SerializeField] private GameObject _backButton;
        private TMP_InputField _input;
        private UnityTransport connection;
        //private PanelManager.ConnectionInfo info;
        public UnityEvent joinedGame;
        public System.Action onClickBack;
        enum States{
            Select,
            InSession,
            PostGame
        }
        bool connected = false;
        States activeState = States.Select;

        void Start(){
            //if(NetworkManager.Singleton.IsServer)
                //NetworkManager.Singleton.OnClientConnectedCallback += RandomizeSpawn;
            //NetworkManager.Singleton.OnClientConnectedCallback += RequestAddPlayerServerRPC;

            joinedGame = new UnityEvent();
            joinedGame.AddListener(JoinGameCallback);
            onClickBack += onClickBackCallback;

            //_playerCanvasManager = FindObjectOfType<PlayerCanvasManager>();
            //_playerCanvasManager.Init();
            
            _input = connectionUI.GetComponentInChildren<TMP_InputField>();
            connection = NetworkManager.Singleton.GetComponent<UnityTransport>();

            
            Physics.gravity = new Vector3(0, -20f, 0);
        }


        public void StartServer(){
            NetworkManager.Singleton.StartServer();
            joinedGame.Invoke();
        }
        public void StartHost(){
            NetworkManager.Singleton.StartHost();
            joinedGame.Invoke();
        }
        public void JoinGame(){
            if(_input.text.Length < 7 || _input.text == null){
                connection.ConnectionData.Address = "127.0.0.1";
            }else{
                connection.ConnectionData.Address = _input.text;
            }
            NetworkManager.Singleton.StartClient();
            joinedGame.Invoke();

        }
        public void BackButtonPressed(){
            onClickBack?.Invoke();
        }

        private void JoinGameCallback(){
            connectionUI.gameObject.SetActive(false);
            connected = true;
            activeState = States.InSession;
            _backButton.SetActive(false);
            //_playerCanvasManager.OnJoinGame();
        }
        private void onClickBackCallback(){
            SceneManager.LoadScene(MENU_SCENE_NAME);
        }

        private void OnDestroy(){
            if(NetworkManager.Singleton.gameObject != null){
                Destroy(NetworkManager.Singleton.gameObject);
            }
            joinedGame.RemoveListener(JoinGameCallback);
            onClickBack -= onClickBackCallback;
        }
    }
}