using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spartans.Events;

using Unity.Netcode;

public class WeaponHit : NetworkBehaviour
{
    [SerializeField] private Transform handRef;
    [SerializeField] private GameObject objRef;
    //[SerializeField] private GameObject parentObj;
    GameObject newObj;
    Animator _animator;
    private bool _attackOnCooldown = false;

    public static event System.Action<int, WeaponHit> onWeaponHit;
    public static event System.Action onAttackStart;

    public void Awake(){
        onAttackStart += Attack;
        _animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        if(!IsLocalPlayer) return;
        Debug.DrawRay(handRef.transform.position, transform.TransformDirection(handRef.transform.up), Color.magenta, 0.25f);

        if(Input.GetButtonDown("fire1") && !_attackOnCooldown){
            onAttackStart.Invoke();
        }
        if(_attackOnCooldown){
            Physics.Raycast(handRef.transform.position, transform.TransformDirection(handRef.transform.up));
        }

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

    private void Attack(){
        _animator.SetBool("attack", true);
        _attackOnCooldown = true;
        StartCoroutine(ResetAttackCooldown());
        print(_animator.GetNextAnimatorStateInfo(1).fullPathHash + "\n" + 
        _animator.GetNextAnimatorStateInfo(1).normalizedTime);
    }

    IEnumerator ResetAttackCooldown(){
        yield return new WaitForSeconds(1.5f);
        _attackOnCooldown = false;
    }
}
