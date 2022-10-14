using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

using Spartans.UI;

namespace Spartans.GameMode{
    ///Summary
    ///This class controls game mode specific logic such as win conditions, also in charge of updating UI related to game mode data
    ///Do Not Destroy On Load Singleton use, starts in lobby scene and carries through(for now)
    public class GameModeManager : NetworkBehaviour
    {
        public enum States{
            None,
            Starting,
            InProgress,
            Finished
        }
        //Singleton Instance
        public static GameModeManager Instance;

        //private vars for passing data for GameMode
        
        //GameMode Data is a scritable object, so all clients have a copy of its data, 
        // but not the same copy, however we dont plan to modify these values, so syncing 
        // the state of the scritable objects is unessecary
        private GameModeData[] _gameModes;
        private GameModeData _currentGameMode;
        private States _currentState = States.None;
        

        ///----Lobby Scene variables------
        private CanvasManager _canvasManager;
        private Image _gameModeBanner;
        private Button[] buttons;

        //-------GameScene Variables-----------
        private Slider[] _scores;
        private TMP_Text TimerText;
        private GameModeBase _gameMode;
        float timeTillNextTimerUpdate;


        //Network Variables for syncing
        private NetworkList<int> TeamScores;
        private NetworkVariable<int> TimeRemaining = new NetworkVariable<int>(0);
        //below for UI display in lobby scene
        private NetworkVariable<int> SelectedMode = new NetworkVariable<int>();

        //consider making below var gamemode specific
        [SerializeField] private int MaxGameTime;

        //Awake occurs in Lobby Scene
        void Awake(){
            if(Instance == null){
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }else{
                Destroy(this.gameObject);
            }
        }

        //Start occurs during lobby scene
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

        void Update()
        {
            if(_currentState == States.None){
                return;
            }
            if(IsServer)
            {
                //Starting Phase
                if(_currentState==States.Starting && TimeRemaining.Value > 0)
                {
                    if(timeTillNextTimerUpdate <= 0)
                    {
                        TimeRemaining.Value -= 1;
                        timeTillNextTimerUpdate = 1;
                    }
                    else
                    {
                        timeTillNextTimerUpdate -= Time.deltaTime;
                    }
                }//Post starting state
                else if(_currentState!=States.Starting && TimeRemaining.Value > 0)
                {
                    if(timeTillNextTimerUpdate <= 0)
                    {
                        TimeRemaining.Value -= 1;
                        timeTillNextTimerUpdate = 1;
                    }
                    else
                    {
                        timeTillNextTimerUpdate -= Time.deltaTime;
                    }
                }
                else{
                    _currentState = _currentState+1;
                    TimeRemaining.Value = 85;
                    print("Updated State to " + _currentState);
                }
            }
        }

        void FixedUpdate(){
            if(IsServer && _currentState >= States.Starting)
            TeamScores[0]++;
        }
        public override void OnNetworkSpawn()
        {
            if(IsClient)
            {
                //print("Subeed");
                SelectedMode.OnValueChanged += UpdateGameMode;
                TimeRemaining.OnValueChanged += UpdateTimeRemaining;
            }
            if(IsServer)
            {
                foreach(Button item in buttons){
                    item.gameObject.SetActive(true);
                }
            }
            //initiallize for all game instances
            UpdateGameMode(0, 0); //Initialize so it has a sprite
            _currentGameMode = _gameModes[0]; 
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

        //only used for UI updating to show selected gamemode
        private void UpdateGameMode(int previousValue, int newValue)
        {
            print("Game Mode Index of " + newValue);
            _gameModeBanner.sprite = _gameModes[newValue].verticalBanner;
        }


        //Called by GameManager after game scene has loaded to start preperations for game
        //Should ONLY be called 1 time
        public void StartAsSelectedMode(){

            _canvasManager = FindObjectOfType<CanvasManager>();

            Transform containerObjForGameModeUI = _canvasManager.transform.Find("TDM_UI").GetChild(0);
            TimerText = containerObjForGameModeUI.GetComponentInChildren<TMP_Text>();
            _scores = containerObjForGameModeUI.GetComponentsInChildren<Slider>();

            //print("SLiders found " + _scores.Length);

            for(int i=0; i<_currentGameMode.numberOfTeams ; i++)
            {
                //print("I is:" + i);
                _scores[i].maxValue = _currentGameMode.numberOfTeams*5;
                TeamScores.Add(0);
            }
            TeamScores.OnListChanged += UpdateTeamScore;
            
            if(IsServer)
            {
                _gameMode = new TDM(2, 10, MaxGameTime);
                _currentState = States.Starting;
                TimeRemaining.Value = MaxGameTime;
            }
            

        }

        private void UpdateTimeRemaining(int previousValue, int newValue)
        {
            //print("called updateTime");
            string displayTime = "";
            int minute, second;
            minute = newValue/60;
            second = newValue%60;

            displayTime = string.Format("{0:00}", minute) + ":" + string.Format("{0:00}", second);

            TimerText.text = displayTime;
        }

        private void UpdateTeamScore(NetworkListEvent<int> changeEvent)
        {
            if(changeEvent.Type == NetworkListEvent<int>.EventType.Value)
            {
                _scores[changeEvent.Index].value = changeEvent.Value;
                //TeamScores[changeEvent.Index] = changeEvent.Value;
            }else{
                print("SHOULDNT OCCUR");
            }
        }

        void OnDisable(){
            SelectedMode.OnValueChanged -= UpdateGameMode;
            TimeRemaining.OnValueChanged -= UpdateTimeRemaining;
            TeamScores.OnListChanged -= UpdateTeamScore;
        }

    }
}
