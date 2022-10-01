using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;
using TMPro;

using Spartans.UI;
using Spartans.Players;

namespace Spartans{
    public class GameManager : NetworkBehaviour
    {
        private const string MENU_SCENE_NAME = "MainMenu";
        public PlayerCanvasManager _playerCanvasManager{get; private set;}
        //private List<Player> _players = new List<Player>();
        [SerializeField] private List<GameObject> _playerPrefabs;
        [SerializeField] private TMP_InputField _input;
        [SerializeField] private Camera _mainCamera;
        private UnityTransport connection;
        public static States activeState{get; private set;}
        //private PanelManager.ConnectionInfo info;
        public static System.Action stateChanged;
        public static System.Action joinedGame;
        public static System.Action leftGame;
        //public System.Action onClickBack;
        public enum States{
            ModeSelect,
            Connected,
            InGame,
            PostGame
        }

        void Start(){
            joinedGame += JoinGameCallback;
            //onClickBack += OnClickBackCallback;

            _playerCanvasManager = FindObjectOfType<PlayerCanvasManager>();
            _playerCanvasManager.Init();

            connection = NetworkManager.Singleton.GetComponent<UnityTransport>();

            activeState = States.ModeSelect;
            stateChanged?.Invoke();
            
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
            SceneManager.LoadScene(MENU_SCENE_NAME);
        }
        public void StopConnection(){
            NetworkManager.Singleton.Shutdown();
            activeState = States.ModeSelect;
            //stateChanged?.Invoke();
            leftGame?.Invoke();
        }

        private void JoinGameCallback(){
            activeState = States.Connected;
            stateChanged?.Invoke();
        }
        private void OnClickBackCallback(){
            SceneManager.LoadScene(MENU_SCENE_NAME);
        }

        private void OnDisable(){
            if(NetworkManager.Singleton != null){
                Destroy(NetworkManager.Singleton.gameObject);
            }
            joinedGame -= JoinGameCallback;
            //onClickBack -= OnClickBackCallback;
        }

        [ServerRpc(RequireOwnership = false)]
        private void requestCharacterServerRpc(ulong clientID, int classIndex){
            if(classIndex > _playerPrefabs.Count-1)
            {
                print("Client sender error: invalid classIndex");
            }
            //print("assigning player for client " + clientID);
            GameObject spawningPlayer = Instantiate(_playerPrefabs[classIndex], Vector3.up*2, Quaternion.identity);
            spawningPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID);
            
        }
        public void requestCharacter(int classIndex){
            requestCharacterServerRpc(NetworkManager.Singleton.LocalClientId, classIndex);
            activeState = States.InGame;
            stateChanged?.Invoke();
            _playerCanvasManager.ToggleHudOnOff();
        }
     
    }
}