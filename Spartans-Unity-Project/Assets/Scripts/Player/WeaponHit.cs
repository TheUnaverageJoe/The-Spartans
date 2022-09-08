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
    private bool _attackOnCooldown, _secondaryAttackOnCooldown = false;
    [SerializeField]private int _playerDamage;
    //[SerializeField] private GameObject parentObj;

    private RaycastHit _lastAttackedObject;
    private List<Transform> _hitPlayers;

    private RaycastHit[] allHit;

    //public static event System.Action<int, WeaponHit> onWeaponHit;

    public event System.Action onAttackStart;
    public event System.Action onSecondaryAttackStart;

    public void Awake(){
        onAttackStart += Attack;
        _animator = GetComponentInChildren<Animator>();
        _hitPlayers = new List<Transform>();
    }
    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(handRef.transform.position, handRef.transform.forward*3, Color.magenta, 0.25f);
        if(!IsLocalPlayer) return;

        if(Input.GetButtonDown("Fire1") && !_attackOnCooldown){
            //call rpc to invoke event
            if(IsServer){
                onAttackStart.Invoke();
            }
            else
                AttackServerRpc();
        }
        if(Input.GetButtonDown("Fire2") && !_secondaryAttackOnCooldown){
            onSecondaryAttackStart?.Invoke();
            _secondaryAttackOnCooldown = true;
            StartCoroutine(ResetSecondaryAttackCooldown());
        }
    }
    void FixedUpdate(){
        if(!IsServer){
            return;
        }
        //allHit = Physics.RaycastAll(handRef.transform.position, transform.TransformDirection(handRef.transform.up), 3, attackMask);
        if(_attackOnCooldown){
            if(Physics.Raycast(handRef.transform.position, handRef.transform.forward, out _lastAttackedObject, 3, attackMask)){
                if(!_hitPlayers.Contains(_lastAttackedObject.transform)){
                    Health _healthAffected = _lastAttackedObject.transform.gameObject.GetComponent<Health>();
                    if(_healthAffected == GetComponent<Health>()){
                        print("You cant hit yourself silly");
                        return;
                    }
                    _healthAffected.TakeDamageServerRpc(_playerDamage);
                    //print("Did damage to: " + _healthAffected.GetHitpoints());
                    _hitPlayers.Add(_healthAffected.transform);
                }
            }
        }
    }

    private void Attack(){
        _animator.SetBool("attack", true);
        _attackOnCooldown = true;
        StartCoroutine(ResetAttackCooldown());
        //print(_animator.GetNextAnimatorStateInfo(1).fullPathHash + "\n" + 
        //_animator.GetNextAnimatorStateInfo(1).normalizedTime);
    }

    IEnumerator ResetAttackCooldown(){
        yield return new WaitForSeconds(1);
        _animator.SetBool("attack", false);
        _attackOnCooldown = false;
        _hitPlayers.Clear();
    }
    IEnumerator ResetSecondaryAttackCooldown(){
        yield return new WaitForSeconds(3);
        _secondaryAttackOnCooldown = false;
    }

    [ServerRpc]
    public void AttackServerRpc(){
        onAttackStart.Invoke();
    }
    
}
