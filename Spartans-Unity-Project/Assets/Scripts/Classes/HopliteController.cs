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
        //private PlayerController _playerController;
        private RaycastHit[] allHit = new RaycastHit[0];
        
        private RaycastHit _lastAttackedObject;
        private List<Transform> _hitPlayers = new List<Transform>();
        private Rigidbody _rb;
        private bool _leaping;

        public override void Init(PlayerController playerController){
            _playerController = playerController;
            _rb = GetComponent<Rigidbody>();

            InputManager.Instance.OnPrimary += PrimaryAttack;
            InputManager.Instance.OnSecondary += SecondaryAttack;
            InputManager.Instance.OnSpecial += SpecialAttack;
        }

        // Update is called once per frame
        void Update()
        {
            Debug.DrawRay(handRef.transform.position, handRef.transform.forward*3, Color.magenta, 0.25f);
            if(!IsLocalPlayer) return;

            /*
            if(PlayerInput.Instance.primary && !_attackOnCooldown){
                //onAttackStart.Invoke();
                PrimaryAttack();
            }
            
            if(PlayerInput.Instance.secondary && !_secondaryAttackOnCooldown){
                //onSecondaryAttackStart?.Invoke();
                SecondaryAttack();
            }
            
            if(PlayerInput.Instance.special){
                SpecialAttack();
                _playerController.JumpStarted();
            }
            */
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
                            //print("You cant hit yourself silly");
                            continue;
                        }
                        //_healthAffected.TakeDamageServerRpc(_playerDamage);
                        _healthAffected.TakeDamage(_playerDamage, _playerController.GetTeamAssociation().Value);
                        _hitPlayers.Add(hit.transform);
                    }
                }
            }

            if(_leaping)
            {
                if(_playerController.IsAirborn())
                {
                    //_rb.AddForce(-transform.TransformDirection(Vector3.forward)*20, ForceMode.Acceleration);
                    _rb.AddForce(Vector3.down*30, ForceMode.Acceleration);
                }else{
                    _leaping = false;
                    _playerController._animationManager.SetParameter("leaping", false);
                }
            }
        }
        public override void PrimaryAttack()
        {
            if(_attackOnCooldown) return;

            AttackServerRpc();
            _attackOnCooldown = true;
            StartCoroutine(ResetAttackCooldown());
        }
        public override void SecondaryAttack()
        {
            if(_secondaryAttackOnCooldown) return;

            SecondaryAttackServerRpc();
            _secondaryAttackOnCooldown = true;
            StartCoroutine(ResetSecondaryAttackCooldown());
        }
        public override void SpecialAttack()
        {
            if(_specialAttackOnCooldown || _playerController.IsAirborn()) return;

            SpecialAttackServerRpc();
            _specialAttackOnCooldown = true;
            StartCoroutine(ResetSpecialAttackCooldown());
        }

        IEnumerator ResetAttackCooldown()
        {
            yield return new WaitForSeconds(1);
            _playerController._animationManager.SetParameter("attack", false);
            _attackOnCooldown = false;
            _hitPlayers.Clear();
        }
        IEnumerator ResetSecondaryAttackCooldown()
        {
            yield return new WaitForSeconds(3);
            _secondaryAttackOnCooldown = false;
        }
        IEnumerator ResetSpecialAttackCooldown()
        {
            yield return new WaitForSeconds(2);
            _specialAttackOnCooldown = false;
        }

        [ServerRpc]
        public void AttackServerRpc()
        {
            //onAttackStart.Invoke();
            _playerController._animationManager.SetParameter("attack", true);
            _attackOnCooldown = true;
            StartCoroutine(ResetAttackCooldown());

        }
        
        [ServerRpc]
        public void SecondaryAttackServerRpc()
        {
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
            newProjectile.GetComponent<Projectile>().SetSource(GetComponent<PlayerController>());
            newProjectile.GetComponent<NetworkObject>().Spawn();
            newProjectile.GetComponent<Rigidbody>().AddForce(newProjectile.transform.forward * speedModifier, ForceMode.VelocityChange);
        }

        [ServerRpc]
        public void SpecialAttackServerRpc()
        {
            _playerController.JumpStarted();
            _leaping = true;
            _playerController._animationManager.SetParameter("leaping", true);

            _rb.AddForce(transform.TransformDirection(Vector3.forward)*30, ForceMode.VelocityChange);
            _rb.AddForce(Vector3.up*30, ForceMode.VelocityChange);

            //_rb.AddForce(-transform.TransformDirection(Vector3.forward)*10, ForceMode.Acceleration);
        }

        void OnDisable()
        {
            InputManager.Instance.OnPrimary -= PrimaryAttack;
            InputManager.Instance.OnSecondary -= SecondaryAttack;
            InputManager.Instance.OnSpecial -= SpecialAttack;
        }

    }
}

