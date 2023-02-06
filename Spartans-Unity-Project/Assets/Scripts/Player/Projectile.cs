using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

using Spartans;
using Spartans.Players;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private float _maxLifeSpanTime;//in seconds
    private System.Nullable<Teams> sourceTeam;
    private PlayerController originPlayer;
    private float _lifeSpanTime = 0;
    //private Collider _collider;

    void FixedUpdate()
    {
        if(!IsServer) return;
        if(_lifeSpanTime >= _maxLifeSpanTime)
        {
            print("Projectile lifespan terminated");
            //Destroy(this.gameObject);
            this.gameObject.GetComponent<NetworkObject>().Despawn();
        }
        else
        {
            _lifeSpanTime += Time.fixedDeltaTime;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        TargetDummy dummyPlayer;
        PlayerController otherPlayer;
        if(!IsServer)
        {
            return;
        }
        
        other.TryGetComponent<PlayerController>(out otherPlayer);
        other.TryGetComponent<TargetDummy>(out dummyPlayer);
        
        if(otherPlayer!=null)
        {
            if(originPlayer == otherPlayer)
            {
                return;
            }
            if(otherPlayer.GetTeamAssociation().Value == sourceTeam)
            {
                return;
            }
        }
        if(dummyPlayer == null && otherPlayer == null)
        {
            if(this.gameObject.GetComponent<NetworkObject>().IsSpawned)
            {
                print("Is spawned");
                this.gameObject.GetComponent<NetworkObject>().Despawn();
            }
            return;
        }

        if(other.TryGetComponent<Health>(out Health hitTarget))
        {
            //hitTarget.TakeDamageServerRpc(_damage);
            hitTarget.TakeDamage(_damage, sourceTeam.Value);
            //Debug.Log("Dummy took damage", dummyPlayer.transform);
            this.gameObject.GetComponent<NetworkObject>().Despawn();
        }
        //print($"Projectile hit {other.name}");    
    }

    public void SetSource(PlayerController sourcePlayer)
    {
        originPlayer = sourcePlayer;
        this.sourceTeam = originPlayer.GetTeamAssociation();
        
    }
}
