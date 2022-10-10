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
        [SerializeField] private TMP_Text _startTimerText;
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
            if(playerConnectionInstances.TryGetValue(dataObject.Id, out GameObject obj)){
                UpdatePlayerConnection(dataObject, obj);
                return;
            }
            GameObject newConnection;
            switch(dataObject.Team){
                case Teams.Red:
                    newConnection= Instantiate(_connectionUiPrefab, _redTeamField.transform);
                    newConnection.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = dataObject.Id.ToString();
                    newConnection.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = _characterIcons[(int)dataObject.Type];
                    newConnection.transform.GetChild(2).gameObject.SetActive(dataObject.IsReady);

                    playerConnectionInstances.Add(dataObject.Id, newConnection);
                    break;
                case Teams.Blue:
                    newConnection= Instantiate(_connectionUiPrefab, _blueTeamField.transform);
                    newConnection.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = dataObject.Id.ToString();
                    newConnection.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = _characterIcons[(int)dataObject.Type];
                    newConnection.transform.GetChild(2).gameObject.SetActive(dataObject.IsReady);

                    playerConnectionInstances.Add(dataObject.Id, newConnection);
                    break;
            }
        }
        public void UpdatePlayerConnection(PlayerLobbyData dataObject, GameObject obj){
            obj.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = dataObject.Id.ToString();
            obj.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = _characterIcons[(int)dataObject.Type];
            obj.transform.GetChild(2).gameObject.SetActive(dataObject.IsReady);
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

        public void UpdateTimeToStart(float time){
            if(time < 0){
                _startTimerText.text = "";
            }
            _startTimerText.text = time.ToString();
        }

        private void OnDestroy(){
            Instance = null;
        }
    }
}