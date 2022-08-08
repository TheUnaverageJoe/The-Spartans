using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Spartans.UI;

namespace Spartans.UI{
    public class PlayerCanvasManager : MonoBehaviour
    {
        private static PanelManager _connectionsRegion;
        private Healthbar _healthBar;
        private PopUpText _alert;
        public void Init(){
            //get rectangular region at top of screen
            _connectionsRegion = GetComponentInChildren<PanelManager>();
            _alert = GetComponentInChildren<PopUpText>();
            _connectionsRegion.Init();
            _connectionsRegion.gameObject.SetActive(false);
            //_alert.gameObject.SetActive(false);

            _healthBar = GetComponentInChildren<Healthbar>();
            if(_healthBar != null){
                _healthBar.Init();
                _healthBar.gameObject.SetActive(false);
            }
        }

        public void OnJoinGame(){
            if(_healthBar != null)
                _healthBar.gameObject.SetActive(true);
        }

        public static PanelManager GetPanelManager(){
            return _connectionsRegion;
        }
    }
}
