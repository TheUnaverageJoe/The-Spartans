using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Spartans.UI;

namespace Spartans.UI{
    public class PlayerCanvasManager : MonoBehaviour
    {
        private static PanelManager _connectionsRegion;
        private PopUpText _alert;
        public void Init(){
            //get rectangular region at top of screen
            _connectionsRegion = GetComponentInChildren<PanelManager>();
            _alert = GetComponentInChildren<PopUpText>();
            _connectionsRegion.Init();
            _connectionsRegion.gameObject.SetActive(false);
            //_alert.gameObject.SetActive(false);
        }

        public static PanelManager GetPanelManager(){
            return _connectionsRegion;
        }
    }
}
