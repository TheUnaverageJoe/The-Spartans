using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

using Spartans.Players;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private float _maxLifeSpanTime;//in seconds
    private PlayerController sourcePlayer;
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
        if(sourcePlayer == null){
            print("PROBLEM");
        }
        if(other.GetComponent<PlayerController>() == sourcePlayer){
            return;
        }
        Health hitTarget;
        if(other.TryGetComponent<Health>(out hitTarget)){
            //hitTarget.TakeDamageServerRpc(_damage);
            hitTarget.TakeDamage(_damage, sourcePlayer.GetTeamAssociation().Value);
        }
        print($"Projectile hit {other.name}");
        this.gameObject.GetComponent<NetworkObject>().Despawn();
    }

    public void SetSource(PlayerController source){
        sourcePlayer = source;
    }
}
