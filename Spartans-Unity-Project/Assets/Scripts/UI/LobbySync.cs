using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;

namespace Spartans{
    public class LobbySync : MonoBehaviour
    {
        public static LobbySync Instance;

        [SerializeField] private GameObject _redTeamField;
        [SerializeField] private GameObject _blueTeamField;
        [SerializeField] private GameObject _connectionUiPrefab;
        [SerializeField] public GameObject _startButton;
        [SerializeField] private LobbyManager _lobbyManager;
        [SerializeField] private List<Sprite> _characterIcons;
        private Dictionary<ulong, GameObject> playerConnectionInstances = new  Dictionary<ulong, GameObject>();

        public enum Teams{
            Red,
            Blue
        }

        private void Awake(){
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public void AddPlayerConnection(PlayerLobbyData dataObject)
        {
            if(playerConnectionInstances.TryGetValue(dataObject._id, out GameObject obj)){
                UpdatePlayerConnection(dataObject, obj);
            }
            GameObject newConnection;
            switch(dataObject._team){
                case Teams.Red:
                    newConnection= Instantiate(_connectionUiPrefab, _redTeamField.transform);
                    newConnection.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = dataObject._id.ToString();
                    newConnection.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = _characterIcons[0];
                    newConnection.transform.GetChild(2).gameObject.SetActive(dataObject._isReady);

                    playerConnectionInstances.Add(dataObject._id, newConnection);
                    break;
                case Teams.Blue:
                    newConnection= Instantiate(_connectionUiPrefab, _blueTeamField.transform);
                    playerConnectionInstances.Add(dataObject._id, newConnection);
                    break;
            }
        }
        public void UpdatePlayerConnection(PlayerLobbyData dataObject, GameObject obj){
            obj.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = dataObject._id.ToString();
            obj.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = _characterIcons[0];
            obj.transform.GetChild(2).gameObject.SetActive(dataObject._isReady);
        }

        public void RemovePlayerConnection(ulong id){
            GameObject connectionObj;
            if(playerConnectionInstances.TryGetValue(id, out connectionObj)){
                Destroy(connectionObj);
                playerConnectionInstances.Remove(id);
            }
        }

        public void StartButtonActive(bool ready){
            _startButton.SetActive(ready);
        }

        private void OnDisable(){
            Instance = null;
        }
    }
}