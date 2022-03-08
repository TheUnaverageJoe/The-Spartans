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
            if(Input.GetKeyDown(KeyCode.Escape)){
                if(Cursor.visible) MouseLock(true);
                else MouseLock(false);
            }
            
        }
        public override void OnNetworkSpawn(){
            print("IsLocalPlayer: " + IsLocalPlayer + " ID: " + NetworkObject.NetworkObjectId);
        }

        void MouseLock(bool Lock){
            if(Lock){
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                return;
            }else if(!Lock){
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        //Used to handle logic when Alt Tabbing in and out of the application
        void OnApplicationFocus(bool hasFocus){
            if(hasFocus){
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }else{
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        //OnDrawGizmos is being usec purly for debugging  purposes to see where the hitbox is in world space
        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
            Gizmos.DrawWireCube(transform.position-Vector3.down*0.1f, new Vector3(1, 0.5f, 0.5f));
        }
    }
}
