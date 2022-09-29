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
        private bool isOpen;
        private System.Action onUiOpen;

        public void Init()
        {
            isOpen = true;
            UpdateUIState();
        }
        void Update(){
            if(PlayerInput.Instance.tab){
                ToggleHudOnOff();
            }
        }
        public void FixedUpdate(){
            //if(!isOpen || _connectionUI.activeSelf) return;
            //UpdateUIState();
        }
        public void ToggleHudOnOff(){
            if(isOpen){
                _connectionUI.SetActive(false);
                _classSelect.SetActive(false);
                _disconnectUI.SetActive(false);
                _backButton.SetActive(false);
            }else{
                UpdateUIState();
            }
            isOpen = !isOpen;
        }
        public void ToggleBackButtonActive(bool state)
        {
            _backButton.SetActive(state);
            if(state == true)
            {
                onUiOpen?.Invoke();
            }

        }
        public void ToggleBackButtonActive()
        {
            _backButton.SetActive(!_backButton.activeSelf);
            if(_backButton.activeSelf == true)
            {
                onUiOpen?.Invoke();
            }
        }
        public void ToggleConnectionButtonsActive()
        {
            if(NetworkManager.Singleton.IsClient)
            {
                bool state = !_disconnectUI.activeSelf;
                _disconnectUI.SetActive(state);
                _connectionUI.SetActive(false);
                if(_disconnectUI.activeSelf == true)
                {
                    onUiOpen?.Invoke();
                }
            }
            else
            {
                _connectionUI.SetActive(!_connectionUI.activeSelf);
                _disconnectUI.SetActive(false);
                if(_connectionUI.activeSelf == true)
                {
                    onUiOpen?.Invoke();
                }
            }
        }
        public void ToggleConnectionButtonsActive(bool state)
        {
            if(NetworkManager.Singleton.IsClient)
            {
                _disconnectUI.SetActive(state);
                _connectionUI.SetActive(false);
            }
            else
            {
                _connectionUI.SetActive(state);
                _disconnectUI.SetActive(false);
                
            }
            if(state == true)
            {
                onUiOpen?.Invoke();
            }
        }
        private void UpdateUIState(){
            switch(GameManager.activeState){
                case GameManager.States.ModeSelect:
                    _connectionUI.SetActive(true);
                    _classSelect.SetActive(false);
                    _disconnectUI.SetActive(false);
                    _backButton.SetActive(true);
                    break;
                case GameManager.States.Connected:
                    _connectionUI.SetActive(false);
                    _classSelect.SetActive(true);
                    _disconnectUI.SetActive(true);
                    _backButton.SetActive(true);
                    break;
                case GameManager.States.InGame:
                    _connectionUI.SetActive(false);
                    _classSelect.SetActive(false);
                    _disconnectUI.SetActive(true);
                    _backButton.SetActive(true);
                    break;
                case GameManager.States.PostGame:
                    print("Post game not implimented");
                    break;
                default:
                    print("BUG IN PlayerCanvasManager");
                    break;
            }
        }
    }
}
