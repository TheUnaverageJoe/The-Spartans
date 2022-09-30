using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Spartans.Players;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private float _maxLifeSpanTime;//in seconds
    public Collider sourceCollider;
    private float _lifeSpanTime = 0;
    //private Collider _collider;

    void FixedUpdate()
    {
        if(!IsServer) return;
        if(_lifeSpanTime >= _maxLifeSpanTime){
            print("Projectile lifespan terminated");
            //Destroy(this.gameObject);
            this.gameObject.GetComponent<NetworkObject>().Despawn();
        }else{
            _lifeSpanTime += Time.fixedDeltaTime;
        }
    }
    
    void OnTriggerEnter(Collider other){
        if(!IsServer){
            return;
        }
        if(sourceCollider == null){
            print("PROBLEM");
        }
        if(other == sourceCollider){
            return;
        }
        Health hitTarget;
        if(other.TryGetComponent<Health>(out hitTarget)){
            hitTarget.TakeDamageServerRpc(_damage);
        }
        print($"Projectile hit {other.name}");
        this.gameObject.GetComponent<NetworkObject>().Despawn();
    }
}
