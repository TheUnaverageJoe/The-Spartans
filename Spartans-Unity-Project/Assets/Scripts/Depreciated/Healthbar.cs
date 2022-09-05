using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Spartans.UI{
    public class Healthbar : MonoBehaviour
    {
        private Slider _healthBar;
        public void Init(){
            _healthBar = GetComponent<Slider>();
            _healthBar.value = _healthBar.maxValue;
        }
        public void UpdateHealthbar(float value){
            _healthBar.value = value;
        }
    }
}
