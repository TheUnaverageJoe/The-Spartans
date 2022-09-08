using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CubeSpawn : MonoBehaviour
{
    [SerializeField] private GameObject objRef;
    
    GameObject newObj;
    
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F)){
            if(NetworkManager.Singleton.IsServer){
                newObj = NetworkManager.Instantiate(objRef,Vector3.zero,Quaternion.identity);
                newObj.GetComponent<NetworkObject>().Spawn();
            }
            else
                connectedServerRpc();
        }
    }
        //Proof of concept for spawning objects
    [ServerRpc]
    public void connectedServerRpc(){
        //connectedClientRpc();
        newObj = NetworkManager.Instantiate(objRef,Vector3.zero,Quaternion.identity);
        newObj.GetComponent<NetworkObject>().Spawn();
    }
}
