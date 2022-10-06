using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Spartans.Players{
    public abstract class ClassController : NetworkBehaviour
    {
        protected System.Action onAttackStart;
        protected System.Action onSecondaryAttackStart;

        protected bool _attackOnCooldown, _secondaryAttackOnCooldown = false;

        public abstract void Init(PlayerController playerController);
        public abstract void PrimaryAttack();
        public abstract void SecondaryAttack();
        public abstract void SpecialAttack();
    }
}

