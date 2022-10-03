using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

using Spartans.Players;

namespace Spartans.UI{
    public class PlayerCanvasManager : MonoBehaviour
    {
        [SerializeField] private GameObject _connectionUI;
        [SerializeField] private GameObject _classSelect;
        [SerializeField] private GameObject _disconnectUI;
        [SerializeField] private GameObject _backButton;
        [SerializeField] private GameObject _volumeSettings;
        [SerializeField] private List<GameObject> _uiObjectList;
        
        private bool isOpen;

        public void Init()
        {
            isOpen = true;
            GameManager.leftGame += OnLeaveGame;
            GameManager.stateChanged += UpdateUIState;
            GameManager.stateChanged += PlayPressedSound;
        }
        void Update(){
            if(PlayerInput.Instance.tab){
                print("Called OnOff");
                ToggleHudOnOff();
            }
        }

        public void ToggleHudOnOff(){
            if(isOpen){
                _connectionUI.SetActive(false);
                _classSelect.SetActive(false);
                _disconnectUI.SetActive(false);
                _backButton.SetActive(false);
                //_volumeSettings.SetActive(false);
            }else{
                UpdateUIState();
            }
            isOpen = !isOpen;
        }

        private void UpdateUIState(){
            switch(GameManager.activeState){
                case GameManager.States.ModeSelect:
                    //print("updating in ModeSelect State");
                    _connectionUI.SetActive(true);
                    _classSelect.SetActive(false);
                    _disconnectUI.SetActive(false);
                    _backButton.SetActive(true);
                    //_volumeSettings.SetActive(false);
                    break;
                case GameManager.States.Connected:
                    //print("updating in Connected State");
                    _connectionUI.SetActive(false);
                    _classSelect.SetActive(true);
                    _disconnectUI.SetActive(true);
                    _backButton.SetActive(false);
                    break;
                case GameManager.States.InGame:
                    //print("updating in InGame State");
                    _connectionUI.SetActive(false);
                    _classSelect.SetActive(false);
                    _disconnectUI.SetActive(true);
                    _backButton.SetActive(false);
                    //_volumeSettings.SetActive(true);
                    break;
                case GameManager.States.PostGame:
                    print("Post game state not implimented");
                    break;
                default:
                    print("BUG IN PlayerCanvasManager");
                    break;
            }
        }
        private void OnLeaveGame(){
            UpdateUIState();
        }
        private void PlayPressedSound(){
            AudioManager.Instance.PlayAudio(AudioManager.AudioChannels.Channel2, AudioManager.SoundClipsIndex.spear_attack);
        }
        private void OnDisable(){
            GameManager.leftGame -= OnLeaveGame;
            GameManager.stateChanged -= UpdateUIState;
        }
    }
}
