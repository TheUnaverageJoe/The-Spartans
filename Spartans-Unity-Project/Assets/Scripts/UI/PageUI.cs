using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spartans.UI
{
    public class PageUI : MonoBehaviour
    {
        public bool ExitOnNewPage;
        // Start is called before the first frame update
        void Start()
        {
            print("ExitOnNewPage is " + ExitOnNewPage);
        }

        public void Enter()
        {
            this.gameObject.SetActive(true);
        }

        public void Exit()
        {
            this.gameObject.SetActive(false);
        }
    }
}
