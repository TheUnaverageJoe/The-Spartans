using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;


using Spartans.UI;
using Spartans.GameMode;

namespace Spartans{
    //This class is a singleton
    public class GameManager : NetworkBehaviour
    {
        public enum States{
            Lobby,
            PreGame,
            InProgress,
            GameOver,
            PostGame
        }
        public static GameManager Instance;
        private const string MENU_SCENE_NAME = "MainMenu";
        public CanvasManager _canvasManager{get; private set;}
        //private List<Player> _players = new List<Player>();
        [SerializeField] private List<GameObject> _playerPrefabs;
        [SerializeField] private Camera _mainCamera;

        public States activeState{get; private set;}
        private Scene m_LoadedScene;
        
        //only for server use
        private Dictionary<ulong, PlayerGameData> playerData = new Dictionary<ulong, PlayerGameData>();

        void Awake(){
            _canvasManager = FindObjectOfType<CanvasManager>();
            if(Instance == null){
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }else{
                Destroy(this.gameObject);
            }
        }

        void Start(){
            NetworkManager.SceneManager.OnSceneEvent += SceneEventHandler;
            NetworkManager.OnClientDisconnectCallback += OnClientDisconnected;

            if(activeState >= States.PreGame){
                print("GameManager finding Canvas and Init()ing");
                //_canvasManager = FindObjectOfType<CanvasManager>();
                _canvasManager.Init();
            }
            
            activeState = States.Lobby;
            
            Physics.gravity = new Vector3(0, -20f, 0);
        }

        public void StopConnection(){
            NetworkManager.Singleton.Shutdown();
            activeState = States.Lobby;
            Instance = null;
            if(NetworkManager.Singleton != null){
                print("GameManager killed Network Manager");
                Destroy(NetworkManager.Singleton.gameObject);
            }
            SceneManager.LoadScene(MENU_SCENE_NAME);
            Destroy(GameModeManager.Instance.gameObject); //destroy for Server
            Destroy(this.gameObject);
        }

        private void OnClickBackCallback(){
            SceneManager.LoadScene(MENU_SCENE_NAME);
        }
        private void SpawnPlayersFromLobby()
        {
            foreach(KeyValuePair<ulong, PlayerGameData> item in playerData)
            {
                Vector3 spawnLocation;
                Quaternion startingRot;
                if(item.Value.Team == Teams.Red)
                {
                    spawnLocation = new Vector3(Random.Range(30, 35), 2, Random.Range(-10, 10));
                    startingRot = Quaternion.Euler(0, -90, 0);
                }else
                {
                    spawnLocation = new Vector3(Random.Range(-30, -35), 2, Random.Range(-10, 10));
                    startingRot = Quaternion.Euler(0, 90, 0);
                }

                //print($"Spawning {item.Value.Type} for client {item.Key} on {item.Value.Team} team");
                GameObject spawningPlayer = Instantiate(_playerPrefabs[(int)item.Value.Type], spawnLocation, startingRot);
                spawningPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(item.Key);
                spawningPlayer.GetComponent<Spartans.Players.PlayerController>().ChangeTeam(item.Value.Team);
            }
        }

        private void SceneEventHandler(SceneEvent sceneEvent)
        {
            if(sceneEvent.ClientsThatTimedOut != null && sceneEvent.ClientsThatTimedOut.Count>0){//sceneEvent.ClientsThatTimedOut.Count > 0
                foreach(ulong id in sceneEvent.ClientsThatTimedOut)
                {
                    print($"{id} timed Out on scene transition");
                }
                Debug.LogWarning("CLIENT FAILED TO LOAD AND/OR SYNC???!");
            }

            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.LoadComplete:
                    //print("Scene name " + sceneEvent.SceneName);
                    break;

                case SceneEventType.UnloadComplete:
                    if(sceneEvent.SceneName == "Lobby"){
                        activeState = States.PreGame;
                    }
                    //print("Unloaded " + sceneEvent.SceneName + " Scene");
                    break;

                case SceneEventType.LoadEventCompleted:
                    if (sceneEvent.ClientId == NetworkManager.ServerClientId)
                    {
                        // *** IMPORTANT ***
                        // Keep track of the loaded scene, you need this to unload it
                        m_LoadedScene = sceneEvent.Scene;
                    }

                    if(!_canvasManager)
                    {
                        //print("Had to find canvas after load");
                        _canvasManager = FindObjectOfType<CanvasManager>();
                        _canvasManager.Init();
                    }

                    if(playerData.Count > 0)
                    {
                        SpawnPlayersFromLobby();
                    }
                    //called on Clients and server, used for initiallization at beginning of game scene
                    // which is only know after the scene finishes loading on all players
                    GameModeManager.Instance.StartAsSelectedMode();
                    
                    break;
                    
                case SceneEventType.UnloadEventCompleted:
                    break;
            }
        }

        public void LoadPlayerData(){
            List<PlayerGameData> temp = SavedData.LoadDataFromPlayerLobby();

            foreach(var item in temp){
                if(playerData.ContainsKey(item.Id)){
                    Debug.LogWarning("DUPLICATE KEY ADDED WHEN LOADING PLAYER DATA FROM LOBBY");
                }
                playerData.Add(item.Id, item);
            }
        }


        //**SUMMARY**
        ///<summary>
        ///OnDisable Method destroyes the NetworkManager instance if we go back to main menu
        ///Unsubscribes any Action listeners
        ///</summary>
        private void OnDisable()
        {
            //print("Disabled Game Manager");
            //onClickBack -= OnClickBackCallback;
            NetworkManager.SceneManager.OnSceneEvent -= SceneEventHandler;
        }

        private void OnClientDisconnected(ulong clientId)
        {
            if (clientId == NetworkManager.ServerClientId)
            {
                Debug.LogWarning("server shutting down");
                Destroy(NetworkManager.gameObject);
                Destroy(GameModeManager.Instance.gameObject); //destroy for clients
                SceneManager.LoadScene(MENU_SCENE_NAME, LoadSceneMode.Single);
            }
        }
     
    }
}