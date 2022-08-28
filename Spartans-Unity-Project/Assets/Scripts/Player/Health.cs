using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spartans.UI;
using Unity.Netcode;

namespace Spartans.Players{
    public class Health : NetworkBehaviour
    {
        [SerializeField]private int _maxHitpoints;
        [SerializeField] private int _currentHitpoints;
        Animator _animator;
        FloatingHealth _healthDisplay;
        private int _previousHitpoints;
        //[SerializeField] private GameObject _floatingHealthPrefab;
        //private GameObject hpDisplay;


        //int param is new health value
        public event System.Action<int> onHealthChanged;
        public event System.Action onDie;


        //Stand in for Awake and Start, Initialization method called from Player.cs
        public void Init(){
            //_maxHitpoints = 3;
            //_currentHitpoints = _maxHitpoints;
            //hpDisplay = Instantiate(_floatingHealthPrefab, GetComponentInChildren<Canvas>().transform);
            _animator = GetComponent<Animator>();
            _healthDisplay = GetComponentInChildren<FloatingHealth>();

            onDie += OnDieCallback;

            if(_healthDisplay == null){
                print("floating health not ready!");
                return;
            }
            _healthDisplay.Init();

            //need to invoke after healthDisplay is setup to get it to update
            onHealthChanged?.Invoke(_maxHitpoints);
        }

        public void FixedUpdate(){
            //if()
        }
        public void TakeDamage(int damage){
            _currentHitpoints -= damage;
            onHealthChanged?.Invoke(_currentHitpoints);
            if(_currentHitpoints <= 0) onDie?.Invoke();
        }

        public int GetMaxHitpoints(){
            return _maxHitpoints;
        }
        public int GetHitpoints(){
            return _currentHitpoints;
        }

        private void OnDieCallback(){
            _animator.SetBool("dead", true);
            print("Killed: " + this.GetComponent<Player>().playerName.ToString());
        }



        //***Currently Unused***
        //Below method only used for debugging
        IEnumerator WaitThenDo(){
            yield return new WaitForSeconds(2);
            onHealthChanged?.Invoke(_maxHitpoints);
        }
    }
}
