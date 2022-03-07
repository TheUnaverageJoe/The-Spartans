using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

using Spartans.UI;
namespace Spartans.Players
{
    public class Player : NetworkBehaviour
    {
        private GameObject _mainCamera;
        private PlayerMovement _playerMovement;
        private PlayerCanvasManager _HUD;
        private Camera cam;
        public string playerName{ get; private set; }
        public void Start(){
            
            _playerMovement = GetComponent<PlayerMovement>();
            _HUD = FindObjectOfType<PlayerCanvasManager>();
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            cam = GetComponentInChildren<Camera>();

            
            playerName = NetworkObject.NetworkObjectId.ToString();
            print("playerName: " + playerName);

            if(IsLocalPlayer){
                _mainCamera.SetActive(false);
                _playerMovement.Init();
                _HUD.Init();
            }else{
                //All players have a camera object on the prefab, disable all other cameras if its not ours
                cam.gameObject.SetActive(false);
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        public override void OnNetworkSpawn(){
            
        }
    }
}
