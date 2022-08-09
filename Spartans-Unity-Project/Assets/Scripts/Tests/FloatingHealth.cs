using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spartans.Players;
public class FloatingHealth : MonoBehaviour
{
    private GameObject _player;
    private Slider _slider;

    public void Start(){
        _player = this.GetComponentInParent<Transform>().gameObject
                      .GetComponentInParent<Transform>().gameObject;
        Health.onHealthChanged += HandleOnHealthChange;
    }

    // Update is called once per frame
    void Update()
    {
        //keep health bar next to player in the world space
        this.transform.rotation = _player.transform.rotation;
        //this.transform.position = _player.transform.position + (Vector3.up * 4.5f);
    }

    private void HandleOnHealthChange(int value, Health reference){
        if(_player.GetComponent<Health>() != null && reference != _player.GetComponent<Health>()){
            print("Not my update");
            return;
        }else{
            Debug.LogError("might not have Health component?");
        }
        float maxHP = (float)reference.GetMaxHitpoints();
        if(value > maxHP){
            Debug.LogError("ISSUE, HP higher than MAX HP");
        }
        float returnVal = (float)value / maxHP;
        _slider.value = returnVal;
    }

    public void OnDisable(){
        Health.onHealthChanged -= HandleOnHealthChange;
    }

}
