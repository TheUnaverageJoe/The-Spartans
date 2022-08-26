using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spartans.UI;

namespace Spartans.Players{
    public class Health : MonoBehaviour
    {
        [SerializeField]private int _maxHitpoints;
        [SerializeField] private int _currentHitpoints;
        private int _previousHitpoints;
        //[SerializeField] private GameObject _floatingHealthPrefab;
        //private GameObject hpDisplay;


        //int param is new health value
        public static event System.Action<Health, int> onHealthChanged;

        private void Awake(){
            _maxHitpoints = 3;
            _currentHitpoints = _maxHitpoints;
            //hpDisplay = Instantiate(_floatingHealthPrefab, GetComponentInChildren<Canvas>().transform);
        }
        private void Start(){
            //onHealthChanged += OnHealthChangedCallback;
            //onHealthChanged?.Invoke(this, _maxHitpoints);
            StartCoroutine(WaitThenDo());
        }
        public void FixedUpdate(){
            //if()
        }
        public void TakeDamage(int damage){
            _currentHitpoints -= damage;
            onHealthChanged?.Invoke(this, _maxHitpoints);
        }

        public int GetMaxHitpoints(){
            return _maxHitpoints;
        }
        public int GetHitpoints(){
            return _currentHitpoints;
        }

        private void OnHealthChangedCallback(Health reference, int value){
            print("Ref is: " + reference);
        }

        //Below method only used for debugging
        IEnumerator WaitThenDo(){
            yield return new WaitForSeconds(2);
            onHealthChanged?.Invoke(this, _maxHitpoints);
        }
    }
}
