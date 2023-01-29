using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Spartans.Players{
    public abstract class ClassController : NetworkBehaviour
    {
        protected System.Action onAttackStart;
        protected System.Action onSecondaryAttackStart;
        protected PlayerController _playerController;

        //Hacky way of adding TargetDummy instead of using interfaces
        protected TargetDummy _targetDummy;

        protected bool _attackOnCooldown, _secondaryAttackOnCooldown, _specialAttackOnCooldown = false;

        public abstract void Init(PlayerController playerController);
        public abstract void Init(TargetDummy targetDummy);
        public abstract void PrimaryAttack();
        public abstract void SecondaryAttack();
        public abstract void SpecialAttack();
    }
}

