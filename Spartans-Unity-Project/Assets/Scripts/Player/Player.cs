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
        private GameObject _mainCamera;
        private GameManager _gameManager;
        private PlayerMove _playerMovement;
        private PlayerCanvasManager _HUD;
        private Camera cam;
        private Rigidbody _rigidbody;
        private Animator _animator;
        private Health _myHealth;
        public string playerName{ get; private set; }

        public void Awake(){
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
            _myHealth = GetComponent<Health>();

            _playerMovement = GetComponent<PlayerMove>();
            _gameManager = FindObjectOfType<GameManager>();
            _HUD = _gameManager.GetComponent<PlayerCanvasManager>();
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
            _playerMovement.Init(_rigidbody, _animator);
        }

        // Update is called once per frame
        void Update()
        {
            if(IsLocalPlayer){
                if(Input.GetKeyDown(KeyCode.Escape)){
                    if(Cursor.visible) MouseLock(true);
                    else MouseLock(false);
                }

                if(Input.GetKeyDown(KeyCode.Tab)){
                    PlayerCanvasManager.GetPanelManager().gameObject.SetActive(!PlayerCanvasManager.GetPanelManager().gameObject.activeSelf);
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
            if(!IsLocalPlayer) return;
            
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
            if(!IsServer) return;
            //print("HI MY NAME IS: " + playerName);
            base.OnNetworkSpawn();


            GameObject spawnpoint = GameObject.FindGameObjectsWithTag("Spawn1")[0];
            float xSpawnPos = spawnpoint.transform.position.x;
            float zSpawnPos = spawnpoint.transform.position.z;
            
            transform.position = new Vector3(Random.Range(xSpawnPos - 3,xSpawnPos + 3), 
                                            1, Random.Range(zSpawnPos - 3, zSpawnPos + 3));
            //print("MOVE MMEEEEE");
        }
        //OnDrawGizmos is being usec purly for debugging  purposes to see where the hitbox is in world space
        private void OnDrawGizmos() {
            //Gizmos.color = Color.red;
            //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
            //Gizmos.DrawWireCube(transform.position, new Vector3(1.4f, 0.1f, 1.4f));
        }
    }
}
