using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Unity.Netcode;

namespace Spartans.UI{
    public class PanelManager : MonoBehaviour
    {
        public class ConnectionInfo : INetworkSerializable
        {
            public string name;
            public float ping;
            public string connectedAs;
            public ConnectionInfo(string name="", float ping=-1, string connectedAs="None"){
                this.name = name;
                this.ping = ping;
                this.connectedAs = connectedAs;
            }
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref this.name);
                serializer.SerializeValue(ref this.ping);
                serializer.SerializeValue(ref this.connectedAs);
            }
        }
        [SerializeField] private GameObject connectionTextPrefab;
        private List<ConnectionInfo> activeConnections = new List<ConnectionInfo>();
        private List<GameObject> connectionInfoObjects;
        public static UnityEvent<ConnectionInfo> NewConnection;
        private Text _title;
        public void Init(){
            _title = GetComponentInChildren<Text>();
            _title.text = "Connections: " + activeConnections.Count;
            connectionInfoObjects = new List<GameObject>();
            NewConnection = new UnityEvent<ConnectionInfo>();
            NewConnection.AddListener(AddActiveConnection);
        }
        public void AddActiveConnection(ConnectionInfo connection){
            activeConnections.Add(connection);

            GameObject temp = Instantiate(connectionTextPrefab, Vector3.zero, Quaternion.identity,transform);
            connectionInfoObjects.Add(temp);
            temp.GetComponent<Text>().text = FormatConnectionText(connection.name, connection.ping, connection.connectedAs);
            
        }
        private string FormatConnectionText(string name, float ping, string type){
            const int lineSpace = 40;
            const int nameSpace = 20;
            const int pingSpace = 8;
            const int typeSpace = 12;
            int nameLen = name.Length;
            int pingDigits = ping.ToString().Length;
            int typeLen = type.Length;

            string output = "";
            for(int k=0; k<nameSpace; k++){
                if(name[k]!=null){
                    output+=name[k];
                }
                output += " ";
                if(k == nameSpace-1){
                    output += "|";
                }
            }

            int pad = pingSpace - pingDigits;
            for(int k=0; k<pad-1; k++){
                output += " ";
            }
            output += ping.ToString();
            output += " | ";

            output += type;
            return output;
        }
    }
}