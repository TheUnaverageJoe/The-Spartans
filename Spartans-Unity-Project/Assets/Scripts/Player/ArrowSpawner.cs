using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ArrowSpawner : NetworkBehaviour 
{
    [SerializeField] private GameObject _arrowPrefab;
    [SerializeField] private float speedModifier;
    [SerializeField] private Transform _lookAt;
    private WeaponHit _weaponHit;

    public void Awake(){
        _weaponHit = GetComponent<WeaponHit>();
    }
    // Start is called before the first frame update
    void Start()
    {
        _weaponHit.onSecondaryAttackStart += SecondaryAttackCallback;
    }

    private void SecondaryAttackCallback(){
        //Quaternion facingDirection = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        if(IsServer){
            GameObject newProjectile = NetworkManager.Instantiate(_arrowPrefab, _lookAt.position, _lookAt.rotation);
            newProjectile.GetComponent<Projectile>().sourceCollider = GetComponent<Collider>();
            newProjectile.GetComponent<NetworkObject>().Spawn();
            newProjectile.GetComponent<Rigidbody>().AddForce(transform.forward * speedModifier, ForceMode.VelocityChange);
            
        }else{
            SecondaryAttackServerRpc();
        }
    }

    [ServerRpc]
    public void SecondaryAttackServerRpc(){
        //Quaternion facingDirection = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        GameObject newProjectile = NetworkManager.Instantiate(_arrowPrefab, _lookAt.position, _lookAt.rotation);
        newProjectile.GetComponent<Projectile>().sourceCollider = GetComponent<Collider>();
        newProjectile.GetComponent<NetworkObject>().Spawn();
        newProjectile.GetComponent<Rigidbody>().AddForce(transform.forward * speedModifier, ForceMode.VelocityChange);
    }
}
