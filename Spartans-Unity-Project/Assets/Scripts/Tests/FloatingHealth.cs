using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spartans.Players;
public class FloatingHealth : MonoBehaviour
{
    private GameObject _player;
    
    private bool playerFound = false;

    // Update is called once per frame
    void Update()
    {
        //find player object once they spawn
        if(!playerFound){
            _player = GameObject.FindObjectOfType<Player>().gameObject;
            if(_player != null) playerFound = true;
        }else{
            //keep health bar next to player in the world space
            this.transform.rotation = _player.transform.rotation;
            this.transform.position = _player.transform.position + (Vector3.up * 4.5f);
        }
    }
}
