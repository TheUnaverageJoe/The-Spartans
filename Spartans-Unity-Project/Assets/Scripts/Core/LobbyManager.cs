using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;

using Spartans.Players;
using Spartans.UI;
using System;

namespace Spartans{
    public class LobbyManager : NetworkBehaviour
    {
        [SerializeField] private GameObject _gameManagerPrefab;
        [SerializeField] private TMP_InputField _input;
        private CanvasManager _canvasManager;
        private const string MENU_SCENE_NAME = "MainMenu";
        private const string GAMESCENE = "TestMap";
        private UnityTransport connection;
        private Dictionary<ulong, CharacterTypes> playerCharacterSelections = new Dictionary<ulong, CharacterTypes>();
        private List<ulong> redTeam = new List<ulong>();
        private List<ulong> blueTeam = new List<ulong>();
        private Coroutine countdownRoutine;
        private float _timer;


        //Network Variables
        private NetworkList<PlayerLobbyData> connectedPlayers;
        private NetworkVariable<float> _startCountdown = new NetworkVariable<float>();
        
        
        // Start is called before the first frame update
        void Start()
        {
            //print("Lobby scene loaded");
            connectedPlayers = new NetworkList<PlayerLobbyData>();

            connection = NetworkManager.Singleton.GetComponent<UnityTransport>();
            _canvasManager = FindObjectOfType<CanvasManager>();
            _canvasManager.Init();

        }
        void Update(){
            if(IsServer)
            {
                if(_startCountdown.Value >= 0)
                {
                    if(_timer <= 0f)
                    {
                        _startCountdown.Value -= 1;
                        _timer = 1;
                    }
                    else
                    {
                        _timer -= Time.deltaTime;
                    }
                }
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if(IsClient){
                connectedPlayers.OnListChanged += LobbyPlayersHandler;
                _startCountdown.OnValueChanged += StartTimeUpdate;
                //print("Connected players " + connectedPlayers.Count);
                foreach(PlayerLobbyData client in connectedPlayers){
                    print("Addeding connection for client Instance " + client.Id);
                    LobbySync.Instance.AddPlayerConnection(client);
                }
            }
            if(IsServer){
                NetworkManager.OnClientConnectedCallback += NotifyClientConnected;
                NetworkManager.OnClientDisconnectCallback += NotifyClientDisconnected;

                foreach(NetworkClient client in NetworkManager.ConnectedClientsList){
                    NotifyClientConnected(client.ClientId);
                }
            }
            
            //print("Lobby spawned into " + NetworkManager.Singleton.ConnectedHostname);

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

        private void NotifyClientConnected(ulong clientID){
            print($"Client {clientID} connected");
            LobbySync.Instance.StartButtonActive(false);

            if(IsServer){
                if(countdownRoutine != null){
                   StopCoroutine(countdownRoutine);     
                }
                else{
                    //print("No routine to stop???");
                }
            }
            Teams team = redTeam.Count == blueTeam.Count ? Teams.Red : Teams.Blue;
            CharacterTypes type;
            bool isReady = false;
            if(!playerCharacterSelections.TryGetValue(clientID, out type)){
                type = CharacterTypes.Hoplite;
                isReady = false;
            }
            PlayerLobbyData newPlayer = new PlayerLobbyData(clientID, team, type, isReady);
            if(team == Teams.Red){
                redTeam.Add(clientID);
            }else if(team == Teams.Blue){
                blueTeam.Add(clientID);
            }
            //LobbySync.Instance.AddPlayerConnection(newPlayer);
            connectedPlayers.Add(newPlayer);
            print("Added player " + newPlayer.Id + " to lobby");
        }
        private void NotifyClientDisconnected(ulong clientID)
        {
            foreach(PlayerLobbyData item in connectedPlayers)
            {
                if(item.Id == clientID)
                {
                    connectedPlayers.Remove(item);
                }
            }
        }
        public void LeaveLobby()
        {
            NetworkManager.Singleton.Shutdown();
            Destroy(NetworkManager.Singleton.gameObject);
            SceneManager.LoadScene(MENU_SCENE_NAME);
        }
        public void BackToMenu()
        {
            //if(!IsServer && !IsClient){
            Destroy(NetworkManager.Singleton.gameObject);
            //}
            SceneManager.LoadScene("MainMenu");
        }

        private void PassOffToGameManager()
        {
            if(!IsServer) return;
            /*
            int counter = 0;
            KeyValuePair<ulong, CharacterTypes>[] saved = new KeyValuePair<ulong, CharacterTypes>[playerCharacterSelections.Count];
            foreach(KeyValuePair<ulong, CharacterTypes> entry in playerCharacterSelections)
            {
                //playerCharacterSelections.Remove(entry.Key);  Can do this if memory needs to be conserved
                saved[counter] = entry;
                counter+=1;
                
            }
            GameManager.Instance.PopulatePlayerSelections(saved);
            */


            //trying new savedData static class strategy
            //Explicitly call methods which the GameManager needs to run on scene start
            PlayerLobbyData[] playerDataArray = new PlayerLobbyData[connectedPlayers.Count];
            for(int i=0; i<connectedPlayers.Count; i++){
                playerDataArray[i] = connectedPlayers[i];
            }
            SavedData.SavePlayerLobbyData(playerDataArray);
            GameManager.Instance.LoadPlayerData();
            
        }

        [ServerRpc(RequireOwnership=false)]
        public void RequestAssignCharacterServerRpc(ulong requestingClient, CharacterTypes character)
        {
            if(!playerCharacterSelections.ContainsKey(requestingClient))
            {
                playerCharacterSelections.Add(requestingClient, character);
                
                Teams teamAssignment = Teams.Red;
                int clientEntryIndex = -1;
                foreach(var item in connectedPlayers){
                    if(item.Id == requestingClient){
                        clientEntryIndex = connectedPlayers.IndexOf(item);
                    }
                    teamAssignment = redTeam.Contains(item.Id) ? Teams.Red : Teams.Blue;
                }
                
                PlayerLobbyData newPlayer = new PlayerLobbyData(requestingClient, teamAssignment, character, true);
                if(clientEntryIndex >= 0){
                    connectedPlayers[clientEntryIndex] = newPlayer;
                }
                //print(newPlayer.ToString());

                OfferStartIfAllReady();
                //var status = NetworkManager.SceneManager.LoadScene(GAMESCENE, LoadSceneMode.Single);
                //print($"Added {character.ToString()} for client {requestingClient.ToString()}");
            }
            else
            {
                print("Changing class not implimented");
            }

            //if(CheckCanStart()){
            //    OfferStartIfAllReady();
            //}
        }
        public void RequestAssignCharacter(int character)
        {
            RequestAssignCharacterServerRpc(NetworkManager.LocalClientId, (CharacterTypes)character);
        }
        private bool CheckCanStart()
        {
            if(!IsServer) return false;
            //print("checking Can Start");
            foreach(ulong id in NetworkManager.ConnectedClientsIds)
            {
                if(!playerCharacterSelections.ContainsKey(id))
                {
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
        
        private void StartStartingCountdown()
        {
            countdownRoutine = StartCoroutine(StartSelectionCountdown());
        }

        private void StartTimeUpdate(float previousValue, float newValue)
        {
            LobbySync.Instance.UpdateTimeToStart(newValue);
        }

        private void OnDisable()
        {
            //print("Disabling LobbyManager");
            NetworkManager.OnClientConnectedCallback -= NotifyClientConnected;
            NetworkManager.OnClientDisconnectCallback -= NotifyClientDisconnected;
            connectedPlayers.OnListChanged -= LobbyPlayersHandler;
            _startCountdown.OnValueChanged -= StartTimeUpdate;
        }

        //Called from start game button on Lobby scene
        public void StartGame()
        {
            if(CheckCanStart())
            {
                //GameMode.GameModeManager.Instance.SelectMode();
                _startCountdown.Value = 5;
                LobbySync.Instance.StartButtonActive(false);
                StartStartingCountdown();
                PassOffToGameManager();
            }
        }

        private void OfferStartIfAllReady()
        {
            if(CheckCanStart())
            {
                //print("Offered Start");
                LobbySync.Instance.StartButtonActive(true);
            }
        }

        private void LobbyPlayersHandler(NetworkListEvent<PlayerLobbyData> changeEvent)
        {

            if(changeEvent.Type == NetworkListEvent<PlayerLobbyData>.EventType.Add || 
                changeEvent.Type == NetworkListEvent<PlayerLobbyData>.EventType.Value)
            {
                LobbySync.Instance.AddPlayerConnection(changeEvent.Value);
            }//else if(changeEvent.Type == NetworkListEvent<PlayerLobbyData>.EventType.Value){}
            else if(changeEvent.Type == NetworkListEvent<PlayerLobbyData>.EventType.Remove)
            {
                LobbySync.Instance.RemovePlayerConnection(changeEvent.Value.Id);
            }
            else
            {
                print("Unknown case needed to be handled in LobbyPlayersHandler");
            }
        }
    }
}