using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spartans.Players;
using Unity.Netcode;

namespace Spartans.UI{
    public class FloatingHealth : MonoBehaviour
    {
        [SerializeField] public Transform camTransform;
        [SerializeField] private Health _associatedHealth;
        private Slider _slider;
        private Text _nameText;

        //stand in for start and awake, initiallization method, called from Health.cs
        //old coupled code
        /*
        public void Init(){
            _slider = GetComponent<Slider>();
            _nameText = GetComponentInChildren<Text>();
            //_associatedHealth = _player.GetComponent<Health>();

            _associatedHealth.OnHealthChanged += OnHealthChangeCallback;
            _associatedHealth.OnKilledBy += OnDieCallback;
            _associatedHealth.OnRespawn += OnRespawnCallback;

            _nameText.text = transform.parent.parent.GetComponent<PlayerController>().playerName.ToString();
            //print($"{_nameText.text} ran Init()");
        }
        */
        void Awake()
        {
            if(_associatedHealth == null)
            {
                Debug.LogWarning("Floating health display disabled due to missing assocaited Health");
                this.enabled = false;
            }
            else
            {
                _slider = GetComponent<Slider>();
                _nameText = GetComponentInChildren<Text>();
                //_associatedHealth = _player.GetComponent<Health>();

                _associatedHealth.OnHealthChanged += OnHealthChangeCallback;
                //_associatedHealth.OnKilledBy += OnDieCallback;
                Health.OnKilledBy += OnDieCallback;
                _associatedHealth.OnRespawn += OnRespawnCallback;
            }
        }


        // LateUpdate is called once per frame, Called after Update but before render cycle
        void LateUpdate()
        {
            //keep health bar next to player in the world space
            this.transform.rotation = PlayerCameraFollow.Instance.transform.rotation;
            //this.transform.position = _player.transform.position + (Vector3.up * 4.5f);
        }

        public void ChangeName(string newName)
        {
            _nameText.text = newName;
        }

        private void OnHealthChangeCallback(int value)
        {
            float maxHP = (float)_associatedHealth.GetMaxHitpoints();
            if(value > maxHP)
            {
                Debug.LogError("ISSUE, HP higher than MAX HP");
            }
            float returnVal = (float)value / maxHP;
            _slider.value = returnVal;
            //print("Set slider to: " + _slider.value);
            
        }
        private void OnDieCallback(Health health, Teams team){
            if(health == _associatedHealth)
                this.gameObject.SetActive(false);
        }

        private void OnRespawnCallback()
        {
            this.gameObject.SetActive(true);
        }
    }
}
