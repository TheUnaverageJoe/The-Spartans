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
        private const string GAMESCENE = "TestMap";
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

            _canvasManager = FindObjectOfType<CanvasManager>();
            _canvasManager.Init();

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
         private void OnDisable(){
            print("Disabled Game Manager");
            if(NetworkManager.Singleton != null){
                Destroy(NetworkManager.Singleton.gameObject);
            }
            joinedGame -= JoinGameCallback;
            //onClickBack -= OnClickBackCallback;
        }

        [ServerRpc(RequireOwnership=false)]
        public void requestAssignCharacterServerRpc(ulong requestingClient, CharacterTypes character)
        {
            if(!playerCharacterSelections.ContainsKey(requestingClient))
            {
                playerCharacterSelections.Add(requestingClient, character);
                //var status = NetworkManager.SceneManager.LoadScene(GAMESCENE, LoadSceneMode.Single);
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
                    StartCoroutine(StartSelectionCountdown());
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
            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.LoadComplete:
                    break;
                case SceneEventType.UnloadComplete:
                    break;
                case SceneEventType.LoadEventCompleted:
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

                    foreach(KeyValuePair<ulong, CharacterTypes> item in playerCharacterSelections){
                        print("Spawning player for client" + item.Key);
                        GameObject spawningPlayer = Instantiate(_playerPrefabs[(int)item.Value], Vector3.up*2, Quaternion.identity);
                        spawningPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(item.Key);
                    }
                    break;
                    
                case SceneEventType.UnloadEventCompleted:
                    break;
            }
        }

        IEnumerator StartSelectionCountdown(){
            print("Starting ready countdown");
            yield return new WaitForSeconds(5);
            var status = NetworkManager.SceneManager.LoadScene(GAMESCENE, LoadSceneMode.Additive);
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogWarning($"Failed to load {GAMESCENE} " +
                        $"with a {nameof(SceneEventProgressStatus)}: {status}");
            }
            //StopCoroutine(StartSelectionCountdown());
        }
     
    }
}