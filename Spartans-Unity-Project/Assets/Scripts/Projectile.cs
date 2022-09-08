using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spartans.Players;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private float _maxLifeSpanTime;//in seconds
    public Collider sourceCollider;
    //private float _lifeSpanTime = 0;
    //private Collider _collider;

    void Awake(){
        //_collider = GetComponent<BoxCollider>();
        Destroy(this.gameObject, _maxLifeSpanTime);
    }
    /*
    void FixedUpdate()
    {
        if(_lifeSpanTime >= _maxLifeSpanTime){
            print("Projectile lifespan terminated");
            Destroy(this.gameObject);
        }else{
            _lifeSpanTime += Time.fixedDeltaTime;
        }
        
    }
    */
    void OnTriggerEnter(Collider other){
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
        Destroy(this.gameObject);
    }
}
