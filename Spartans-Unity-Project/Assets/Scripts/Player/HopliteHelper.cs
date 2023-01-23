using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spartans.Players
{
    public class HopliteHelper : MonoBehaviour
    {
        HopliteController _parentHopliteController;
        void Start()
        {
            _parentHopliteController = transform.parent.GetComponent<HopliteController>();
        }

        //FinisherDone called by animation event on hoplite character
        //Animation which calls event: "finisher_baked_retarget"
        public void FinisherDone()
        {
            _parentHopliteController.UnpinnedEvent();
        }
    }
}
