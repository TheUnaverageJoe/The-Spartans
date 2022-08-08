using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spartans.Players;
public class FloatingHealth : MonoBehaviour
{
    private GameObject _player;

    public void Start(){
        _player = this.gameObject;
        
    }

    // Update is called once per frame
    void Update()
    {
        //keep health bar next to player in the world space
        this.transform.rotation = _player.transform.rotation;
        this.transform.position = _player.transform.position + (Vector3.up * 4.5f);
    }
}
