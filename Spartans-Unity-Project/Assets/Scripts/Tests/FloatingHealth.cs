using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spartans.Players;
using Unity.Netcode;

namespace Spartans.UI{
    public class FloatingHealth : MonoBehaviour
    {
        private GameObject _player;
        [SerializeField] Transform camTransform;
        private Slider _slider;
        private Text _nameText;
        private Health _myHealth;

        //stand in for start and awake, initiallization method, called from Health.cs
        public void Init(){
            _player =   GetComponentInParent<Canvas>().gameObject
                        .GetComponentInParent<Rigidbody>().gameObject;
            _slider = GetComponent<Slider>();
            _nameText = GetComponentInChildren<Text>();
            _myHealth = _player.GetComponent<Health>();

            _myHealth.onHealthChanged += HandleOnHealthChange;
            _myHealth.onDie += OnDieCallback;

            Camera[] temp = FindObjectsOfType<Camera>(false);
            /*print("enabled cams: " + temp.Length);
            if(_player.GetComponent<NetworkObject>().IsLocalPlayer){
                int x = 0;
                foreach(Camera cam in temp){
                    ++x;
                    print("Cam " + x + ": " + cam.name);
                }
            }
            */
            
            camTransform = temp[0].transform;
            _nameText.text = _player.GetComponent<Player>().playerName.ToString();
            print($"{_nameText.text} ran Init()");
        }

        // LateUpdate is called once per frame, Called after Update but before render cycle
        void LateUpdate()
        {
            //keep health bar next to player in the world space
            this.transform.rotation = camTransform.rotation;
            //this.transform.position = _player.transform.position + (Vector3.up * 4.5f);
        }

        private void HandleOnHealthChange(int value){
            
            if(_player.GetComponent<Health>() == null){
                //print(_player.GetComponent<Health>());
                Debug.LogError("might not have Health component?");
            }

            float maxHP = (float)_myHealth.GetMaxHitpoints();
            if(value > maxHP){
                Debug.LogError("ISSUE, HP higher than MAX HP");
            }
            float returnVal = (float)value / maxHP;
            _slider.value = returnVal;
            //print("Set slider to: " + _slider.value);
            
        }
        /*
        public void OnEnable(){
            _myHealth.onHealthChanged += HandleOnHealthChange;
            _myHealth.onDie += OnDieCallback;
           print("re-enabled listeners for updating");
        }
        
        public void OnDisable(){
            _myHealth.onHealthChanged -= HandleOnHealthChange;
            _myHealth.onDie -= OnDieCallback;
            print("Removed Listener");
        }
        */
        private void OnDieCallback(){
            this.gameObject.SetActive(false);
        }

    }
}
