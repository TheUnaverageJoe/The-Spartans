using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;

public class WeaponHit : MonoBehaviour
{
    [SerializeField] private Transform handRef;
    [SerializeField] private GameObject objRef;
    [SerializeField] private GameObject parentObj;

    // Start is called before the first frame update
    void Start()
    {
        parentObj = GameObject.Find("Testing");   
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(handRef.transform.position, transform.TransformDirection(handRef.transform.up), Color.magenta, 1);

        if(Input.GetKeyDown(KeyCode.Space)){
            if(NetworkManager.Singleton.IsServer)
                Instantiate(objRef,Vector3.zero,Quaternion.identity,parentObj.transform);
            else
                connectedServerRpc();

        }
    }

    [ServerRpc]
    public void connectedServerRpc(){
        connectedClientRpc();
    }
    [ClientRpc]
    public void connectedClientRpc(){
        print("called from server");
        if(!NetworkManager.Singleton.IsServer)
        Instantiate(objRef,Vector3.zero,Quaternion.identity,parentObj.transform);
    }
}
