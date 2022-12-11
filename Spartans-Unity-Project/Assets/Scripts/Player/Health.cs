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
        [SerializeField] private float _respawnTime = 4;
        PlayerController _playerController;
        //SFloatingHealth _healthDisplay;
        private float timeOfDeath = 0;
        //private int _previousHitpoints;



        //int param is new health value
        public event System.Action<int> OnHealthChanged;
        public event System.Action<Teams> OnKilledBy;
        public event System.Action OnRespawn;


        //Stand in for Awake and Start, Initialization method called from Player.cs
        public void Init(PlayerController playerController){
            //_healthDisplay = GetComponentInChildren<FloatingHealth>();
            _playerController = playerController;

            //if(IsServer)
            OnKilledBy += OnDieCallback;

            //if(_healthDisplay == null){
            //    print("floating health not ready!");
            //    return;
            //}
            //_healthDisplay.Init();

            //need to invoke after healthDisplay is setup to get it to update
            OnHealthChanged?.Invoke(_maxHitpoints);
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
        
        //Method should only be called from server exclusive code blocks
        public void TakeDamage(int damage, Teams userTeam)
        {
            if(!IsServer){
                print("TakeDamage not called from server");
                return;
            }
            //NEED TO consider case of is friendly fire off
            if(_playerController.GetTeamAssociation() == userTeam)
            {
                //NO FRIENDLY FIRE
                Debug.LogWarning("No Friendly Fire");
                return;
            }
            UpdateHealthClientRpc(damage, userTeam);
            _currentHitpoints -= damage;
            OnHealthChanged?.Invoke(_currentHitpoints);
            if(_currentHitpoints <= 0) OnKilledBy?.Invoke(userTeam);
            //deal damage

        }

        [ClientRpc]
        public void UpdateHealthClientRpc(int damage, Teams team){
            if(IsServer) return;
            _currentHitpoints -= damage;
            OnHealthChanged?.Invoke(_currentHitpoints);
            if(_currentHitpoints <= 0) OnKilledBy?.Invoke(team);
        }

        public int GetMaxHitpoints(){
            return _maxHitpoints;
        }
        public int GetHitpoints(){
            return _currentHitpoints;
        }

        private void OnDieCallback(Teams killedByTeam){
            if(IsServer){
                Spartans.GameMode.GameModeManager.Instance.AddScore(killedByTeam, 1);
                timeOfDeath = _respawnTime;
                _playerController._animationManager.SetParameter("dead", true);
            }
        }
        //Respawn is only called by the server hence why only a 
        //  clientRpc exists for respawning and not a serverRpc
        private void Respawn(){
            //_healthDisplay.gameObject.SetActive(true);
            _currentHitpoints = _maxHitpoints;
            OnHealthChanged?.Invoke(_maxHitpoints);
            OnRespawn.Invoke();
            _playerController._animationManager.SetParameter("dead", false);

            RespawnClientRpc();
        }
        [ClientRpc]
        public void RespawnClientRpc(){
            if(IsServer) return;
            //_healthDisplay.gameObject.SetActive(true);
            _currentHitpoints = _maxHitpoints;
            OnHealthChanged?.Invoke(_maxHitpoints);
            OnRespawn.Invoke();
        }
        void OnDisable(){
            OnKilledBy -= OnDieCallback;
        }
    }
}
