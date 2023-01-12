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
        //PlayerController otherPlayer;
        if(!other.TryGetComponent<PlayerController>(out PlayerController otherPlayer)){
            //Not a player
            this.gameObject.GetComponent<NetworkObject>().Despawn();
            return;
        }
        if(sourcePlayer == null){
            print("PROBLEM");
        }
        if(otherPlayer == sourcePlayer){
            return;
        }
        if(sourcePlayer.GetTeamAssociation().Value == otherPlayer.GetTeamAssociation().Value)
        {
            return;
        }
        
        //Health hitTarget;
        if(other.TryGetComponent<Health>(out Health hitTarget)){
            //hitTarget.TakeDamageServerRpc(_damage);
            hitTarget.TakeDamage(_damage, sourcePlayer.GetTeamAssociation().Value);
            this.gameObject.GetComponent<NetworkObject>().Despawn();
        }
        //print($"Projectile hit {other.name}");    
    }

    public void SetSource(PlayerController source){
        sourcePlayer = source;
    }
}
