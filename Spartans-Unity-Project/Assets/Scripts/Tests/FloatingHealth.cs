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
        [SerializeField]private Transform camTransform;
        private Slider _slider;
        private Text _nameText;

        public void Awake(){
            _player =   GetComponentInParent<Canvas>().gameObject
                        .GetComponentInParent<Rigidbody>().gameObject;
            _slider = GetComponent<Slider>();
            _nameText = GetComponentInChildren<Text>();
            Health.onHealthChanged += HandleOnHealthChange;
            Health.onDie += OnDieCallback;
        }
        public void Start(){
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
        }

        // LateUpdate is called once per frame, Called after Update but before render cycle
        void LateUpdate()
        {
            //keep health bar next to player in the world space
            this.transform.rotation = camTransform.rotation;
            //this.transform.position = _player.transform.position + (Vector3.up * 4.5f);
        }

        private void HandleOnHealthChange(Health reference, int value){
            if(_player.GetComponent<Health>() != null && reference != _player.GetComponent<Health>()){
                //print("Not my update");
                return;
            }else{
                //print("ref val: " + _player.GetComponent<Health>().name);
                //print("Player val: " + _player.GetComponent<Health>().name);
                if(_player.GetComponent<Health>() == null){
                    //print(_player.GetComponent<Health>());
                    Debug.LogError("might not have Health component?");
                }
            }
            float maxHP = (float)reference.GetMaxHitpoints();
            if(value > maxHP){
                Debug.LogError("ISSUE, HP higher than MAX HP");
            }
            float returnVal = (float)value / maxHP;
            _slider.value = returnVal;
            //print("Set slider to: " + _slider.value);
            
        }
        public void OnDisable(){
            Health.onHealthChanged -= HandleOnHealthChange;
            print("Removed Listener");
        }
        private void OnDieCallback(Health reference){
            this.gameObject.SetActive(false);
        }

    }
}
