using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spartans.Players;
using Unity.Netcode;

namespace Spartans.Players
{
    public class ShieldBarerController : ClassController
    {

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        public override void Init(PlayerController playerController)
        {
            _playerController = playerController;
            //_rb = GetComponent<Rigidbody>();
        }

        public override void PrimaryAttack()
        {
            throw new System.NotImplementedException();
        }

        public override void SecondaryAttack()
        {
            throw new System.NotImplementedException();
        }

        public override void SpecialAttack()
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
