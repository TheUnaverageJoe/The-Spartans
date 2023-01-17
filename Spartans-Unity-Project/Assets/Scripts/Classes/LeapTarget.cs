using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spartans.Players
{
    public class LeapTarget : MonoBehaviour
    {
        private AnimationManager _animManager;
        private bool pinned;
        public event System.Action<bool> OnPinned;

        public void Init(AnimationManager anim)
        {
            pinned = false;
            _animManager = anim;
        }

        public void LeapedTarget()
        {
            pinned = true;
            //_animManager.SetParameter("pinned", true);
            _animManager.Play("FallBackDeathAnim", 0);
            OnPinned.Invoke(true);
        }

        public void Unpinned()
        {
            pinned = false;
            _animManager.SetParameter("pinned", false);
            OnPinned.Invoke(false);
        }
    }
}

