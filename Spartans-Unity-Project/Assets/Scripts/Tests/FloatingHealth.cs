using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spartans.Players;

namespace Spartans.UI{
    public class FloatingHealth : MonoBehaviour
    {
        private GameObject _player;
        private Transform camTransform;
        private Slider _slider;
        private string _name;

        public void Awake(){
            _player =   GetComponentInParent<Canvas>().gameObject
                        .GetComponentInParent<Rigidbody>().gameObject;
            _slider = GetComponent<Slider>();

            //camTransform = Camera.allCameras[0].transform;
            //foreach(Camera cam in Camera.allCameras){
            //    print("cam pos: " + cam.transform.position);
            //}
            //print("Num Cams: " + Camera.allCameras.Length);
            Health.onHealthChanged += HandleOnHealthChange;
            //print("_player's name: " + _player.name);
        }
        public void Start(){
            Camera[] temp = FindObjectsOfType<Camera>(false);
            print("enabled cams: " + temp.Length);
            foreach(Camera cam in temp){
                print("Name: " + cam.name);
            }
            camTransform = temp[0].transform;
        }

        // Update is called once per frame
        void Update()
        {
            //keep health bar next to player in the world space
            this.transform.rotation = camTransform.rotation;
            //this.transform.position = _player.transform.position + (Vector3.up * 4.5f);
        }

        private void HandleOnHealthChange(Health reference, int value){
            if(_player.GetComponent<Health>() != null && reference != _player.GetComponent<Health>()){
                print("Not my update");
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
            print("Set slider to: " + _slider.value);
            
        }
        public void OnDisable(){
            Health.onHealthChanged -= HandleOnHealthChange;
            //print("Removed Listener");
        }

    }
}
