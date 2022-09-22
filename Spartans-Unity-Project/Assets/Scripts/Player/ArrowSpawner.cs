using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ArrowSpawner : NetworkBehaviour 
{
    public GameObject mainCamera;
    [SerializeField] private GameObject _arrowPrefab;
    [SerializeField] private float speedModifier;
    [SerializeField] private Transform _lookAt;
    [SerializeField] private LayerMask _aimColliderMask = new LayerMask();
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
            float x = Screen.width / 2f;
            float y = Screen.height / 2f;
            var ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(new Vector2(x, y));
            Vector3 initialPos = _lookAt.position;
            Quaternion rot = _lookAt.rotation;
            if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, _aimColliderMask)){
                Vector3 dir = raycastHit.point - mainCamera.transform.position;
                rot = Quaternion.LookRotation(dir);
                //print(raycastHit.point.x + " " + raycastHit.point.y + " " + raycastHit.point.z);
            }
            //Vector3 dir = ray.direction - ray.origin;

            GameObject newProjectile = NetworkManager.Instantiate(_arrowPrefab, initialPos, rot);
            newProjectile.GetComponent<Projectile>().sourceCollider = GetComponent<Collider>();
            newProjectile.GetComponent<NetworkObject>().Spawn();
            newProjectile.GetComponent<Rigidbody>().AddForce(newProjectile.transform.forward * speedModifier, ForceMode.VelocityChange);
            
        }else{
            SecondaryAttackServerRpc();
        }
    }

    [ServerRpc]
    public void SecondaryAttackServerRpc(){
        //Quaternion facingDirection = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        //GameObject newProjectile = NetworkManager.Instantiate(_arrowPrefab, _lookAt.position, _lookAt.rotation);
        float x = Screen.width / 2f;
        float y = Screen.height / 2f;
        var ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(new Vector2(x, y));
        Vector3 initialPos = _lookAt.position;
        Quaternion rot = _lookAt.rotation;
        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, _aimColliderMask)){
            Vector3 dir = raycastHit.point - mainCamera.transform.position;
            rot = Quaternion.LookRotation(dir);
            //print(raycastHit.point.x + " " + raycastHit.point.y + " " + raycastHit.point.z);
        }
        //Vector3 dir = ray.direction - ray.origin;

        GameObject newProjectile = NetworkManager.Instantiate(_arrowPrefab, initialPos, rot);
        newProjectile.GetComponent<Projectile>().sourceCollider = GetComponent<Collider>();
        newProjectile.GetComponent<NetworkObject>().Spawn();
        newProjectile.GetComponent<Rigidbody>().AddForce(newProjectile.transform.forward * speedModifier, ForceMode.VelocityChange);
    }
}
