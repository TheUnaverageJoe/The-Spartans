using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spartans.Players{
    public class Health : MonoBehaviour
    {
        private int _maxHitpoints;
        private int _hitpoints;


        //int param is new health value
        public event System.Action<int> onHealthChanged;

        private void Start(){
            _maxHitpoints = 3;
            _hitpoints = _maxHitpoints;
            
        }
    }
}
