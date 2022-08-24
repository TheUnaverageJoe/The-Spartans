using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;

public class WeaponHit : NetworkBehaviour
{
    [SerializeField] private Transform handRef;
    [SerializeField] private GameObject objRef;
    [SerializeField] private GameObject parentObj;
    GameObject newObj;

    public static event System.Action<int, WeaponHit> onWeaponHit;

    // Update is called once per frame
    void Update()
    {
        if(!IsLocalPlayer) return;
        Debug.DrawRay(handRef.transform.position, transform.TransformDirection(handRef.transform.up), Color.magenta, 1);

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

    //NOT IN USE ATM
    //_____________________________________________________
    [ClientRpc]
    public void connectedClientRpc(){
        print("called from server");
        if(!NetworkManager.Singleton.IsServer)
        NetworkManager.Instantiate(objRef,Vector3.zero,Quaternion.identity);
    }
}
