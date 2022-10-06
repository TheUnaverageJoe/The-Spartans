using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;


using Spartans.UI;
using Spartans.Players;

namespace Spartans{
    //This class is a singleton
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance;
        private const string MENU_SCENE_NAME = "MainMenu";
        public CanvasManager _canvasManager{get; private set;}
        //private List<Player> _players = new List<Player>();
        [SerializeField] private List<GameObject> _playerPrefabs;
        [SerializeField] private Camera _mainCamera;

        private GameMode.GameModeBase _gameMode;
        //private GameObject[] _objectives;
        //private GameObject selectedObjective;
        public static States activeState{get; private set;}
        private Scene m_LoadedScene;
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
            NetworkManager.SceneManager.OnSceneEvent += SceneEventHandler;
            //onClickBack += OnClickBackCallback;

            if(activeState >= States.InGame){
                print("GameManager finding Canvas and Init()ing");
                _canvasManager = FindObjectOfType<CanvasManager>();
                _canvasManager.Init();
            }
            

            activeState = States.ModeSelect;
            stateChanged?.Invoke();

            playerCharacterSelections = new Dictionary<ulong, CharacterTypes>();
            
            Physics.gravity = new Vector3(0, -20f, 0);
        }

        public void StopConnection(){
            NetworkManager.Singleton.Shutdown();
            activeState = States.ModeSelect;
            Instance = null;
            if(NetworkManager.Singleton != null){
                print("GameManager killed Network Manager");
                Destroy(NetworkManager.Singleton.gameObject);
            }
            SceneManager.LoadScene(MENU_SCENE_NAME);
            Destroy(this.gameObject);
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
         private void OnDisable(){
            print("Disabled Game Manager");
            
            
            joinedGame -= JoinGameCallback;
            //onClickBack -= OnClickBackCallback;
        }

        public override void OnNetworkSpawn(){
            base.OnNetworkSpawn();
            /*
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
            _gameMode = selectedObjective.GetComponent<GameMode.GameModeBase>();
            */
        }

        private void SceneEventHandler(SceneEvent sceneEvent){
            var clientOrServer = sceneEvent.ClientId == NetworkManager.ServerClientId ? "server" : "client";
            if(sceneEvent.ClientsThatTimedOut != null && sceneEvent.ClientsThatTimedOut.Count>0){//sceneEvent.ClientsThatTimedOut.Count > 0
                foreach(ulong id in sceneEvent.ClientsThatTimedOut){
                    print($"{id} timed Out on scene transition");
                }
                Debug.LogWarning("CLIENT FAILED TO LOAD AND/OR SYNC???!");
            }
            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.LoadComplete:
                    print("Scene name " + sceneEvent.SceneName);
                    break;
                case SceneEventType.UnloadComplete:
                    print("Unloaded " + sceneEvent.SceneName + " Scene");
                    break;
                case SceneEventType.LoadEventCompleted:
                    // if(IsServer){
                    //     NetworkManager.SceneManager.UnloadScene(SceneManager.GetSceneByName("Lobby"));
                    // }
                    print("LoadEventCompleted fired");
                    // We want to handle this for only the server-side
                    if (sceneEvent.ClientId == NetworkManager.ServerClientId)
                    {
                        // *** IMPORTANT ***
                        // Keep track of the loaded scene, you need this to unload it
                        m_LoadedScene = sceneEvent.Scene;
                    }
                    Debug.Log($"Loaded the {sceneEvent.SceneName} scene on " +
                        $"{clientOrServer}-({sceneEvent.ClientId}).");
                    
                    if(playerCharacterSelections.Count > 0 ){
                        
                        foreach(KeyValuePair<ulong, CharacterTypes> item in playerCharacterSelections){
                            print("Spawning player for client" + item.Key);
                            GameObject spawningPlayer = Instantiate(_playerPrefabs[(int)item.Value], Vector3.up*2, Quaternion.identity);
                            spawningPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(item.Key);
                        }
                    }
                    else
                    {
                        print("Would have spawned players but none to spawn");
                    }
                    
                    break;
                    
                case SceneEventType.UnloadEventCompleted:
                    break;
            }
        }

        public void PopulatePlayerSelections(KeyValuePair<ulong, CharacterTypes>[] array){
            //Only needs to be run on server machine
            print("Copied dictonary");
            foreach(KeyValuePair<ulong, CharacterTypes> entry in array){
                playerCharacterSelections.Add(entry.Key, entry.Value);
                print(entry);
            }
        }
        
     
    }
}