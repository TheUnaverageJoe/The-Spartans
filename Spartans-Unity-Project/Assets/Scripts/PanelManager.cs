using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Spartans.UI{
    public class PanelManager : MonoBehaviour
    {
        public class ConnectionInfo
        {
            public string name;
            public float ping;
            public string connectedAs;
            public ConnectionInfo(string name="", float ping=-1, string connectedAs="None"){
                this.name = name;
                this.ping = ping;
                this.connectedAs = connectedAs;
            }
        }
        private List<ConnectionInfo> activeConnections = new List<ConnectionInfo>();
        UnityEvent<ConnectionInfo> NewConnection;
        private Text _title;
        public void Init(){
            _title = GetComponentInChildren<Text>();
            _title.text = "Connections: " + activeConnections.Count;
            NewConnection = new UnityEvent<ConnectionInfo>();
            NewConnection.AddListener(AddActiveConnection);
        }
        public void AddActiveConnection(ConnectionInfo connection){
            activeConnections.Add(connection);
        }
    }
}