using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Spartans.Players{
    public class HopliteController : ClassController
    {
        [SerializeField] private GameObject _arrowPrefab;
        [SerializeField] private float speedModifier;
        [SerializeField] private Transform handRef;
        
        [SerializeField] private LayerMask attackMask;
        [SerializeField] private LayerMask _aimColliderMask = new LayerMask();

        [SerializeField]private int _playerDamage;
        private PlayerController _playerController;
        private RaycastHit[] allHit = new RaycastHit[0];
        
        private RaycastHit _lastAttackedObject;
        private List<Transform> _hitPlayers = new List<Transform>();

        public override void Init(PlayerController playerController){
            _playerController = playerController;
            onAttackStart += PrimaryAttack;
            onSecondaryAttackStart += SecondaryAttack;
        }

        // Update is called once per frame
        void Update()
        {
            Debug.DrawRay(handRef.transform.position, handRef.transform.forward*3, Color.magenta, 0.25f);
            if(!IsLocalPlayer) return;

            if(Input.GetButtonDown("Fire1") && !_attackOnCooldown){
                onAttackStart.Invoke();         
            }
            if(Input.GetButtonDown("Fire2") && !_secondaryAttackOnCooldown){
                onSecondaryAttackStart?.Invoke();
            }
        }
        void FixedUpdate(){
            if(!IsServer){
                return;
            }
            
            if(_attackOnCooldown){
                allHit = Physics.RaycastAll(handRef.transform.position, transform.TransformDirection(handRef.transform.up), 3, attackMask);
                foreach(RaycastHit hit in allHit){
                    if(!_hitPlayers.Contains(hit.transform)){
                        Health _healthAffected = hit.transform.gameObject.GetComponent<Health>();
                        if(_healthAffected == GetComponent<Health>())
                        {
                            print("You cant hit yourself silly");
                            return;
                        }
                        _healthAffected.TakeDamageServerRpc(_playerDamage);
                        _hitPlayers.Add(hit.transform);
                    }
                }
            }
        }
        public override void PrimaryAttack(){
            _playerController._animationManager.SetParameter("attack", true);
            _attackOnCooldown = true;
            StartCoroutine(ResetAttackCooldown());
        }
        public override void SecondaryAttack(){
            SecondaryAttackServerRpc();
            _secondaryAttackOnCooldown = true;
            StartCoroutine(ResetSecondaryAttackCooldown());
        }
        public override void SpecialAttack(){

        }

        

        IEnumerator ResetAttackCooldown(){
            yield return new WaitForSeconds(1);
            _playerController._animationManager.SetParameter("attack", false);
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
        
        [ServerRpc]
        public void SecondaryAttackServerRpc(){
            float x = Screen.width / 2f;
            float y = Screen.height / 2f;
            var ray = PlayerCameraFollow.Instance.camera.ScreenPointToRay(new Vector2(x, y));
            Vector3 initialPos = _playerController.lookAtPoint.position;
            Quaternion rot = _playerController.lookAtPoint.rotation;
            if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, _aimColliderMask)){
                Vector3 dir = raycastHit.point - PlayerCameraFollow.Instance.camera.transform.position;
                rot = Quaternion.LookRotation(dir);
            }
            GameObject newProjectile = NetworkManager.Instantiate(_arrowPrefab, initialPos, rot);
            newProjectile.GetComponent<Projectile>().sourceCollider = GetComponent<Collider>();
            newProjectile.GetComponent<NetworkObject>().Spawn();
            newProjectile.GetComponent<Rigidbody>().AddForce(newProjectile.transform.forward * speedModifier, ForceMode.VelocityChange);
        }

    }
}

