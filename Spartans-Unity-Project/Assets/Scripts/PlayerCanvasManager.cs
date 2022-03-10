using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Spartans.UI;

namespace Spartans.UI{
    public class PlayerCanvasManager : MonoBehaviour
    {
        private PanelManager _connectionsRegion;
        public void Init(){
            //get rectangular region at top of screen
            _connectionsRegion = GetComponentInChildren<PanelManager>();
            _connectionsRegion.Init();
            _connectionsRegion.gameObject.SetActive(false);
        }

        public PanelManager GetPanelManager(){
            return _connectionsRegion;
        }
    }
}
