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
        public PlayerCanvasManager _playerCanvasManager{get; private set;}
        //private List<Player> _players = new List<Player>();
        [SerializeField] private GameObject connectionUI;
        [SerializeField] private GameObject _backButton;
        private TMP_InputField _input;
        private UnityTransport connection;
        public static States activeState{get; private set;}
        //private PanelManager.ConnectionInfo info;
        public System.Action joinedGame;
        public System.Action onClickBack;
        public enum States{
            ModeSelect,
            Connected,
            InGame,
            PostGame
        }

        void Start(){
            joinedGame += JoinGameCallback;
            onClickBack += onClickBackCallback;

            _playerCanvasManager = FindObjectOfType<PlayerCanvasManager>();
            _playerCanvasManager.Init();
            
            _input = connectionUI.GetComponentInChildren<TMP_InputField>();
            connection = NetworkManager.Singleton.GetComponent<UnityTransport>();

            activeState = States.ModeSelect;
            
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
        public void StopConnection(){
            NetworkManager.Singleton.Shutdown();
            activeState = States.ModeSelect;
            //_playerCanvasManager.ToggleConnectionButtonsActive(true);

        }

        private void JoinGameCallback(){
            //_playerCanvasManager.ToggleConnectionButtonsActive(false);
            activeState = States.Connected;
            //_playerCanvasManager.ToggleBackButtonActive(false);
            //_playerCanvasManager.OnJoinGame();
        }
        private void onClickBackCallback(){
            SceneManager.LoadScene(MENU_SCENE_NAME);
        }

        private void OnDisable(){
            if(NetworkManager.Singleton != null){
                Destroy(NetworkManager.Singleton.gameObject);
            }
            joinedGame -= JoinGameCallback;
            onClickBack -= onClickBackCallback;
        }
    }
}