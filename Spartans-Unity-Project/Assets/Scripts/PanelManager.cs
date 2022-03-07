using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Spartans.UI{
    public class PanelManager : MonoBehaviour
    {
        public List<GameObject> activeConnections = new List<GameObject>();
        private Text _title;
        public void Init(){
            _title = GetComponentInChildren<Text>();
            _title.text = "Connections: " + activeConnections.Count;
        }
    }
}