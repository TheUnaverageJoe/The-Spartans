using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spartans.Players{
    public class Health : MonoBehaviour
    {
        private int _maxHitpoints;
        private int _hitpoints;


        //int param is new health value
        public static event System.Action<int, Health> onHealthChanged;

        private void Start(){
            _maxHitpoints = 3;
            _hitpoints = _maxHitpoints;
            
            
        }

        public int GetMaxHitpoints(){
            return _maxHitpoints;
        }
        public int GetHitpoints(){
            return _hitpoints;
        }
    }
}
