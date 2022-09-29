using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;

using Spartans.UI;
namespace Spartans.Players
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] GameObject cameraPrefab;
        [SerializeField] public GameObject worldSpaceCanvas;
        private GameObject _mainCamera;
        private GameManager _gameManager;
        //private PlayerMove _playerMove;
        private PlayerMoveRefactor _playerMove;
        private PlayerCanvasManager _HUD;
        private Camera cam;
        private Rigidbody _rigidbody;
        //private Animator _animator;
        [SerializeField] public AnimationManager _animationManager;
        private Health _myHealth;
        private int players_in_lobby;
        public string playerName{ get; private set; }

        public void Awake(){
            _rigidbody = GetComponent<Rigidbody>();
            //_animator = GetComponentInChildren<Animator>();
            _myHealth = GetComponent<Health>();
            _animationManager = GetComponent<AnimationManager>();

            _playerMove = GetComponent<PlayerMoveRefactor>();
            _gameManager = FindObjectOfType<GameManager>();
            _HUD = _gameManager._playerCanvasManager;
            players_in_lobby = 0;
            //_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            // cam = GetComponentInChildren<Camera>();
            
        }

        public void Start(){
            playerName = "Player " + NetworkObjectId;
            //print("Player name is: " + playerName);
            //print("Is local player: " + IsLocalPlayer);
            //isLocalPlayer makes anything in player scripts happen only on 1 time because theres only 1 player object
            
            if(IsLocalPlayer){
                _mainCamera = Instantiate(cameraPrefab, transform.position, transform.rotation) as GameObject; 
                _mainCamera.transform.GetComponent<CinemachineVirtualCamera>().LookAt = transform.GetChild(0).transform;
                _mainCamera.transform.GetComponent<CinemachineVirtualCamera>().Follow = transform.GetChild(0).transform;
                transform.GetComponentInChildren<FloatingHealth>().camTransform = _mainCamera.transform;
                worldSpaceCanvas.GetComponent<Canvas>().worldCamera = _mainCamera.GetComponent<Camera>();
                worldSpaceCanvas.GetComponentInChildren<FloatingHealth>().camTransform = _mainCamera.transform;
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

                if(Input.GetKeyDown(KeyCode.Tab)){
                    _HUD.ToggleBackButtonActive();
                    _HUD.ToggleConnectionButtonsActive();
                }

                if(GameObject.FindGameObjectsWithTag("Player").Length != players_in_lobby){
                    var players = GameObject.FindGameObjectsWithTag("Player");
                    for(int i = 0; i<players.Length; i++){
                        players[i].transform.Find("WorldSpaceUI").GetComponent<Canvas>().worldCamera = _mainCamera.GetComponent<Camera>();
                        players[i].GetComponentInChildren<FloatingHealth>().camTransform = _mainCamera.transform;
                    }
                    
                    players_in_lobby = players.Length;
                }
            }
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
            //if(!IsServer) return;
            //print("HI MY NAME IS: " + playerName);
            base.OnNetworkSpawn();


            //GameObject spawnpoint = GameObject.FindGameObjectsWithTag("Spawn1")[0];
            //float xSpawnPos = spawnpoint.transform.position.x;
            //float zSpawnPos = spawnpoint.transform.position.z;
            
            //transform.position = new Vector3(Random.Range(xSpawnPos - 3,xSpawnPos + 3), 
            //                                1, Random.Range(zSpawnPos - 3, zSpawnPos + 3));
            //print("MOVE MMEEEEE");
        }
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            print("Despawned: " + playerName);
        }

    }
}
