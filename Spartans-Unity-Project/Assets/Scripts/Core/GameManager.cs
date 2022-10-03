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
    //This class is a singleton
    public class GameManager : NetworkBehaviour
    {

        public static GameManager Instance;
        private const string MENU_SCENE_NAME = "MainMenu";
        public PlayerCanvasManager _playerCanvasManager{get; private set;}
        //private List<Player> _players = new List<Player>();
        [SerializeField] private List<GameObject> _playerPrefabs;
        
        [SerializeField] private Camera _mainCamera;

        private GameMode.GameModeBase _gameMode;
        private GameObject[] _objectives;
        private GameObject selectedObjective;
        public static States activeState{get; private set;}

        private Dictionary<ulong, CharacterTypes> playerCharacterSelections;

        public System.Action stateChanged;
        public System.Action joinedGame;
        public System.Action leftGame;
        //public System.Action onClickBack;
        public enum States{
            ModeSelect,
            Connected,
            InGame,
            GameOver,
            PostGame
        }

        void Awake(){
            if(Instance == null){
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }else{
                Destroy(this.gameObject);
            }
        }

        void Start(){
            joinedGame += JoinGameCallback;
            //onClickBack += OnClickBackCallback;

            _playerCanvasManager = FindObjectOfType<PlayerCanvasManager>();
            _playerCanvasManager.Init();

            activeState = States.ModeSelect;
            stateChanged?.Invoke();

            playerCharacterSelections = new Dictionary<ulong, CharacterTypes>();
            
            Physics.gravity = new Vector3(0, -20f, 0);
        }

        public void BackButtonPressed(){
            SceneManager.LoadScene(MENU_SCENE_NAME);
        }
        public void StopConnection(){
            NetworkManager.Singleton.Shutdown();
            activeState = States.ModeSelect;
            Instance = null;
            Destroy(this.gameObject);
            BackButtonPressed();
        }

        private void JoinGameCallback(){
            activeState = States.Connected;
            stateChanged?.Invoke();
        }
        private void OnClickBackCallback(){
            SceneManager.LoadScene(MENU_SCENE_NAME);
        }

        //**SUMMARY**
        ///<summary>
        ///OnDisable Method destroyes the NetworkManager instance if we go back to main menu
        ///Unsubscribes any Action listeners
        ///</summary>
        // private void OnDisable(){
        //     if(NetworkManager.Singleton != null){
        //         Destroy(NetworkManager.Singleton.gameObject);
        //     }
        //     joinedGame -= JoinGameCallback;
        //     //onClickBack -= OnClickBackCallback;
        // }

        // [ServerRpc(RequireOwnership = false)]
        // private void requestCharacterServerRpc(ulong clientID, int classIndex){
        //     if(classIndex > _playerPrefabs.Count-1)
        //     {
        //         print("Client sender error: invalid classIndex");
        //     }
        //     //print("assigning player for client " + clientID);
        //     GameObject spawningPlayer = Instantiate(_playerPrefabs[classIndex], Vector3.up*2, Quaternion.identity);
        //     spawningPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID);
            
        // }

        // public void requestCharacter(int classIndex){
        //     requestCharacterServerRpc(NetworkManager.Singleton.LocalClientId, classIndex);
        //     activeState = States.InGame;
        //     stateChanged?.Invoke();
        //     _playerCanvasManager.ToggleHudOnOff();
        // }

        // public void AddCharacterSelection(ulong clientID, CharacterTypes type){
        //     playerCharacterSelections.Add(clientID, type);
        // }

        [ServerRpc(RequireOwnership=false)]
        public void requestAssignCharacterServerRpc(ulong requestingClient, CharacterTypes character)
        {
            if(!playerCharacterSelections.ContainsKey(requestingClient))
            {
                playerCharacterSelections.Add(requestingClient, character);
                print($"Added {character.ToString()} for client {requestingClient.ToString()}");
            }else{
                print("SUGMA");
            }
        }
        public void requestAssignCharacter(ulong requestingClient, CharacterTypes character)
        {
            if(IsServer)
            {
                if(!playerCharacterSelections.ContainsKey(requestingClient))
                {
                    playerCharacterSelections.Add(requestingClient, character);
                    print($"Added {character.ToString()} for client {requestingClient.ToString()}");
                }else{
                    print("SUGMA");
                }
            }
            else
            {
                requestAssignCharacterServerRpc(requestingClient, character);
            }
        }
        


        public override void OnNetworkSpawn(){
            base.OnNetworkSpawn();
            //Load all prefabs for objects from "Assets/Resources/Objectives" folder
            _objectives = Resources.LoadAll<GameObject>("Objectives");
            if(_objectives.Length <= 0 )
            {
                print("NO RESOURCES FOUND");
            
            }else
            {
                foreach(GameObject item in _objectives){
                    NetworkManager.Singleton.AddNetworkPrefab(item);
                    //print(item.name);
                }
            }

            if(!IsServer) return;
            selectedObjective = NetworkManager.Instantiate(_objectives[0]);
            selectedObjective.GetComponent<NetworkObject>().Spawn();
        }
     
    }
}