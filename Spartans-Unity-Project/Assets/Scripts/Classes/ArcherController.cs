using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Spartans.Players{
    public class ArcherController : ClassController
    {

        //private PlayerController _playerController;
        private bool _isAiming;

        public override void Init(PlayerController playerController)
        {
            _playerController = playerController;
        }
        public override void Init(TargetDummy targetDummy)
        {
            _targetDummy = targetDummy;
        }

        public override void PrimaryAttack()
        {
            throw new System.NotImplementedException();
        }

        public override void SecondaryAttack()
        {
            SecondaryAttackServerRpc();
        }

        [ServerRpc]
        private void SecondaryAttackServerRpc()
        {
            if(_isAiming)
            {
                _isAiming = false;
                _playerController._animationManager.SetParameter("isAiming", _isAiming);
            }
            else
            {
                _isAiming = true;
                _playerController._animationManager.SetParameter("isAiming", _isAiming);
            }
        }

        public override void SpecialAttack()
        {
            throw new System.NotImplementedException();
        }
    }
}
