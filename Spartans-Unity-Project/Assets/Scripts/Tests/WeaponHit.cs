using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spartans.Players;

using Unity.Netcode;

public class WeaponHit : NetworkBehaviour
{
    [SerializeField] private Transform handRef;
    Animator _animator;
    [SerializeField]LayerMask attackMask;
    private bool _attackOnCooldown = false;
    [SerializeField]private int _playerDamage;
    //[SerializeField] private GameObject parentObj;

    private RaycastHit _lastAttackedObject;
    private List<Transform> _hitPlayers;

    //public static event System.Action<int, WeaponHit> onWeaponHit;
    public static event System.Action<WeaponHit> onAttackStart;

    public void Awake(){
        onAttackStart += Attack;
        _animator = GetComponent<Animator>();
        _hitPlayers = new List<Transform>();
    }
    // Update is called once per frame
    void Update()
    {
        if(!IsLocalPlayer) return;
        Debug.DrawRay(handRef.transform.position, transform.TransformDirection(handRef.transform.up), Color.magenta, 0.25f);

        if(Input.GetButtonDown("Fire1") && !_attackOnCooldown){
            onAttackStart.Invoke(this);
        }
    }
    void FixedUpdate(){
        if(_attackOnCooldown){
            if(Physics.Raycast(handRef.transform.position, transform.TransformDirection(handRef.transform.up), out _lastAttackedObject, 1, attackMask)){
                if(!_hitPlayers.Contains(_lastAttackedObject.transform)){
                    Health _healthAffected = _lastAttackedObject.transform.gameObject.GetComponent<Health>();
                    _healthAffected.TakeDamage(_playerDamage);
                    print("Did damage to: " + _healthAffected.GetHitpoints());
                    _hitPlayers.Add(_healthAffected.transform);
                }
            }
        }
    }

    private void Attack(WeaponHit reference){
        if(this != reference) return;
        _animator.SetBool("attack", true);
        _attackOnCooldown = true;
        StartCoroutine(ResetAttackCooldown());
        print(_animator.GetNextAnimatorStateInfo(1).fullPathHash + "\n" + 
        _animator.GetNextAnimatorStateInfo(1).normalizedTime);
    }

    IEnumerator ResetAttackCooldown(){
        yield return new WaitForSeconds(1);
        _animator.SetBool("attack", false);
        _attackOnCooldown = false;
        _hitPlayers.Clear();
    }
}
