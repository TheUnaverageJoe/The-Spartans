using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

using Spartans.UI;
using System;

namespace Spartans.GameMode{
    public class GameModeManager : NetworkBehaviour
    {
        //Singleton Instance
        public static GameModeManager Instance;

        //private vars for passing data for GameMode
        
        //GameMode Data is a scritable object, so all clients have a copy of its data, 
        // but not the same copy, however we dont plan to modify these values, so syncing 
        // the state of the scritable objects is unessecary
        private GameModeData[] _gameModes;
        private GameModeData _currentGameMode;
        //private bool _gameModeSelected;

        ///----Lobby Scene variables------
        private CanvasManager _canvasManager;
        private Image _gameModeBanner;
        private Button[] buttons;


        //Network Variables for syncing
        private NetworkList<int> TeamScores;
        private NetworkVariable<int> TimeRemaining = new NetworkVariable<int>(0);
        private NetworkVariable<int> SelectedMode = new NetworkVariable<int>();

        void Awake(){
            if(Instance == null){
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }else{
                Destroy(this.gameObject);
            }
        }
        void Start(){
            _canvasManager = FindObjectOfType<CanvasManager>();
            TeamScores = new NetworkList<int>();
            _gameModeBanner = _canvasManager.transform.Find("GameModeDisplay").GetComponent<Image>();

            buttons = _gameModeBanner.gameObject.GetComponentsInChildren<Button>(true);
            if(buttons.Length == 2){
                buttons[0].onClick.AddListener(GetPrevGameMode);
                buttons[1].onClick.AddListener(GetNextGameMode);
            }else
            {
                print("OTHER BUTTONS!??!?! " + buttons.Length);
            }

            //Load all prefabs for objects from "Assets/Resources/Objectives" folder
            _gameModes = Resources.LoadAll<GameModeData>("Modes");
            if(_gameModes.Length <= 0 )
            {
                print("NO RESOURCES FOUND");
            
            }else
            {
                print("Number of GameModes found: " + _gameModes.Length);
            }

            if(!_gameModeBanner){
                Debug.LogWarning("NO BANNER IMAGE COMPONENT");
            }
        }

        public override void OnNetworkSpawn()
        {
            if(IsClient)
            {
                print("Subeed");
                SelectedMode.OnValueChanged += UpdateGameMode;
                TimeRemaining.OnValueChanged += UpdateTimeRemaining;
            }
            if(IsServer)
            {
                foreach(Button item in buttons){
                    item.gameObject.SetActive(true);
                }
                UpdateGameMode(0, 0);
            }
        }

        private void GetNextGameMode(){
            if(SelectedMode.Value < _gameModes.Length-1)
            {
                SelectedMode.Value++;
            }
            else
            {
                SelectedMode.Value = 0;
            }

            _currentGameMode = _gameModes[SelectedMode.Value];
        }

        private void GetPrevGameMode(){
            if(SelectedMode.Value > 0)
            {
                SelectedMode.Value--;
            }
            else
            {
                SelectedMode.Value = _gameModes.Length-1;
            }

            _currentGameMode = _gameModes[SelectedMode.Value];
        }

        private void UpdateGameMode(int previousValue, int newValue)
        {
            print("Game Mode Index of " + newValue);
            _gameModeBanner.sprite = _gameModes[newValue].verticalBanner;
        }

        public void StartSelectedMode(){
            for(int i=0; i<_currentGameMode.numberOfTeams ; i++)
            {
                TeamScores.Add(0);
            }
            TeamScores.OnListChanged += updateTeamScore;
        }

        private void UpdateTimeRemaining(int previousValue, int newValue)
        {
            throw new NotImplementedException();
        }

        private void updateTeamScore(NetworkListEvent<int> changeEvent)
        {
            if(changeEvent.Type == NetworkListEvent<int>.EventType.Value)
            {

            }
        }

    }
}
