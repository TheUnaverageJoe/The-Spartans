using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Spartans.GameMode
{
    public class FlagSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject  _flagPrefab;
        [SerializeField] private Teams TeamAssociation;
        [SerializeField] private Vector3 Position;
        [SerializeField] private Vector3 Rotation;

        private GameObject _flag;

        // Start is called before the first frame update
        void Start()
        {
            if(!NetworkManager.Singleton.IsServer) return;
            _flag = NetworkManager.Instantiate(_flagPrefab, Position, Quaternion.Euler(Rotation.x, Rotation.y, Rotation.z));
            _flag.GetComponent<NetworkObject>().Spawn();

            _flag.GetComponent<Flag>().Init(this);
        }

        public void ResetFlag()
        {
            _flag.transform.position = Position;
            _flag.transform.rotation = Quaternion.Euler(Rotation.x, Rotation.y, Rotation.z);
        }

        public Teams GetTeamAssociation()
        {
            return  TeamAssociation;
        }
    }
}
