using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;

using Spartans.Players;
using Spartans.UI;

namespace Spartans{
    public class LobbyManager : NetworkBehaviour
    {
        [SerializeField] private GameObject _gameManagerPrefab;
        [SerializeField] private TMP_InputField _input;
        private CanvasManager _canvasManager;
        private const string MENU_SCENE_NAME = "MainMenu";
        private const string GAMESCENE = "TestMap";
        private UnityTransport connection;
        private Dictionary<ulong, CharacterTypes> playerCharacterSelections;
        private Coroutine startingRoutine;

        // Start is called before the first frame update
        void Start()
        {
            //print("Lobby scene loaded");
            connection = NetworkManager.Singleton.GetComponent<UnityTransport>();
            _canvasManager = FindObjectOfType<CanvasManager>();
            _canvasManager.Init();

            NetworkManager.OnClientConnectedCallback += NotifyClientConnected;
            
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            //print("Lobby spawned into " + NetworkManager.Singleton.ConnectedHostname);

            if(!IsServer) return;
            playerCharacterSelections = new Dictionary<ulong, CharacterTypes>();
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

        private void NotifyClientConnected(ulong clientID){
            //print($"Client {clientID} connected");

            if(IsServer){
                StopCoroutine(startingRoutine);
            }
        }
        public void LeaveLobby(){
            NetworkManager.Singleton.Shutdown();
            Destroy(NetworkManager.Singleton.gameObject);
            SceneManager.LoadScene(MENU_SCENE_NAME);
        }
        public void BackToMenu(){
            //if(!IsServer && !IsClient){
            Destroy(NetworkManager.Singleton.gameObject);
            //}
            SceneManager.LoadScene("MainMenu");
        }

        private void PassOffToGameManager()
        {
            if(!IsServer) return;
            int counter = 0;
            KeyValuePair<ulong, CharacterTypes>[] saved = new KeyValuePair<ulong, CharacterTypes>[playerCharacterSelections.Count];

            foreach(KeyValuePair<ulong, CharacterTypes> entry in playerCharacterSelections){
                //playerCharacterSelections.Remove(entry.Key);  Can do this if memory needs to be conserved
                saved[counter] = entry;
                counter+=1;
                
            }
            GameManager.Instance.PopulatePlayerSelections(saved);  
        }

        [ServerRpc(RequireOwnership=false)]
        public void RequestAssignCharacterServerRpc(ulong requestingClient, CharacterTypes character)
        {
            if(!playerCharacterSelections.ContainsKey(requestingClient))
            {
                playerCharacterSelections.Add(requestingClient, character);
                //var status = NetworkManager.SceneManager.LoadScene(GAMESCENE, LoadSceneMode.Single);
                print($"Added {character.ToString()} for client {requestingClient.ToString()}");
            }else{
                print("Changing class not implimented");
            }

            if(CheckCanStart()){
                StartStartingCountdown();
            }
        }
        public void RequestAssignCharacter(int character)
        {
            RequestAssignCharacterServerRpc(NetworkManager.LocalClientId, (CharacterTypes)character);
        }
        private bool CheckCanStart(){
            if(!IsServer) return false;
            //print("checking Can Start");
            foreach(ulong id in NetworkManager.ConnectedClientsIds){
                if(!playerCharacterSelections.ContainsKey(id)){
                    return false;
                }
            }
            return true;
        }

        
        IEnumerator StartSelectionCountdown(){
            //print("Starting ready countdown");
            yield return new WaitForSeconds(5);
            //PlayerInput.Instance.OnDisable();
            var status = NetworkManager.SceneManager.LoadScene(GAMESCENE, LoadSceneMode.Single);
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogWarning($"Failed to load {GAMESCENE} " +
                        $"with a {nameof(SceneEventProgressStatus)}: {status}");
            }
            //StopCoroutine(StartSelectionCountdown());
        }
        
        private void StartStartingCountdown(){
            startingRoutine = StartCoroutine(StartSelectionCountdown());
        }
        private void OnDisable(){
            
            //print("Disabling LobbyManager");
            PassOffToGameManager();
            NetworkManager.OnClientConnectedCallback -= NotifyClientConnected;
        }
    }
}