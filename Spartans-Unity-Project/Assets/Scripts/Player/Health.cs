using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spartans.UI;
using Unity.Netcode;

namespace Spartans.Players{
    public class Health : NetworkBehaviour
    {
        [SerializeField]private int _maxHitpoints;
        [SerializeField] private int _currentHitpoints;
        PlayerController _playerController;
        FloatingHealth _healthDisplay;
        [SerializeField] private float _respawnTime = 4;
        private float timeOfDeath = 0;
        private int _previousHitpoints;



        //int param is new health value
        public event System.Action<int> onHealthChanged;
        public event System.Action onDie;
        public event System.Action onRespawn;


        //Stand in for Awake and Start, Initialization method called from Player.cs
        public void Init(PlayerController playerController){
            _healthDisplay = GetComponentInChildren<FloatingHealth>();
            _playerController = playerController;

            onDie += OnDieCallback;

            if(_healthDisplay == null){
                print("floating health not ready!");
                return;
            }
            _healthDisplay.Init();

            //need to invoke after healthDisplay is setup to get it to update
            onHealthChanged?.Invoke(_maxHitpoints);
        }

        public void FixedUpdate(){
            if(!IsServer)return;
            if(timeOfDeath > 0){
                timeOfDeath -= Time.fixedDeltaTime;
                if(timeOfDeath <= 0){
                    Respawn();
                }
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void TakeDamageServerRpc(int damage){
            updateHealthClientRpc(damage);
            _currentHitpoints -= damage;
            onHealthChanged?.Invoke(_currentHitpoints);
            if(_currentHitpoints <= 0) onDie?.Invoke();
        }
        [ClientRpc]
        public void updateHealthClientRpc(int damage){
            if(IsServer) return;
            _currentHitpoints -= damage;
            onHealthChanged?.Invoke(_currentHitpoints);
            if(_currentHitpoints <= 0) onDie?.Invoke();
        }

        public int GetMaxHitpoints(){
            return _maxHitpoints;
        }
        public int GetHitpoints(){
            return _currentHitpoints;
        }

        private void OnDieCallback(){
            if(IsServer){
                timeOfDeath = _respawnTime;
                //_animator.SetBool("dead", true);
                _playerController._animationManager.SetParameter("dead", true);
            }
        }
        //Respawn is only called by the server hence why only a 
        //  clientRpc exists for respawning and not a serverRpc
        private void Respawn(){
            _healthDisplay.gameObject.SetActive(true);
            _currentHitpoints = _maxHitpoints;
            _playerController._animationManager.SetParameter("dead", false);
            onHealthChanged?.Invoke(_maxHitpoints);
            onRespawn.Invoke();

            RespawnClientRpc();
        }
        [ClientRpc]
        public void RespawnClientRpc(){
            if(IsServer) return;
            _healthDisplay.gameObject.SetActive(true);
            _currentHitpoints = _maxHitpoints;
            onHealthChanged?.Invoke(_maxHitpoints);
            onRespawn.Invoke();
        }
    }
}
