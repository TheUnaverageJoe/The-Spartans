using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;

using Spartans.UI;
namespace Spartans.Players
{
    [RequireComponent(typeof(AnimationManager), typeof(Rigidbody))]
    public class Player : NetworkBehaviour
    {
        [SerializeField] GameObject cameraPrefab;
        [SerializeField] public GameObject worldSpaceCanvas;
        private Camera _characterCam;
        private GameManager _gameManager;
        //private PlayerMove _playerMove;
        private PlayerMoveRefactor _playerMove;
        private PlayerCanvasManager _HUD;
        private Rigidbody _rigidbody;
        //private Animator _animator;
        [SerializeField] public AnimationManager _animationManager;
        private Health _myHealth;
        public string playerName{ get; private set; }

        public void Awake(){
            _rigidbody = GetComponent<Rigidbody>();
            //_animator = GetComponentInChildren<Animator>();
            _myHealth = GetComponent<Health>();
            _animationManager = GetComponent<AnimationManager>();

            _playerMove = GetComponent<PlayerMoveRefactor>();
            _gameManager = FindObjectOfType<GameManager>();
            _HUD = _gameManager._playerCanvasManager;
            //_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            // cam = GetComponentInChildren<Camera>();
            
        }

        public void Start(){
            playerName = "Player " + NetworkObjectId;
            //print("Player name is: " + playerName);
            //print("Is local player: " + IsLocalPlayer);
            //isLocalPlayer makes anything in player scripts happen only on 1 time because theres only 1 player object
            
            if(IsLocalPlayer){
                GameObject newCharacterCam = Instantiate(cameraPrefab, transform.position, transform.rotation)
                _characterCam = newCharacterCam.GetComponent<Camera>();
                _characterCam.transform.GetComponent<CinemachineVirtualCamera>().LookAt = transform.GetChild(0).transform;
                _characterCam.transform.GetComponent<CinemachineVirtualCamera>().Follow = transform.GetChild(0).transform;
                transform.GetComponentInChildren<FloatingHealth>().camTransform = _characterCam.transform;
                worldSpaceCanvas.GetComponent<Canvas>().worldCamera = _characterCam.GetComponent<Camera>();
                worldSpaceCanvas.GetComponentInChildren<FloatingHealth>().camTransform = _characterCam.transform;
                //_mainCamera.SetActive(false);
                //_HUD.Init();
            }
            //else{
                //All players have a camera object on the prefab, disable all other cameras if its not ours
            //    cam.gameObject.SetActive(false);
            //}
            //Do in the case of any type of user
            //initialize all players on spawn
            _myHealth.Init();
            _playerMove.Init(_rigidbody);
        }

        // Update is called once per frame
        void Update()
        {
            if(_animationManager == null){

                print("Assign an animator dummy!!!");
                return;
            }
            if(IsLocalPlayer){
                if(Input.GetKeyDown(KeyCode.Escape)){
                    if(Cursor.visible) MouseLock(true);
                    else MouseLock(false);
                }

            }
        }
        void MouseLock(bool Lock){
            if(Lock && Application.isFocused){
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                return;
            }else{
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        
        //Used to handle logic when Alt Tabbing in and out of the application
        void OnApplicationFocus(bool hasFocus){
            if(!IsLocalPlayer || Application.isEditor) return;
            
            if(hasFocus){
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }else{
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
        }

    }
}
