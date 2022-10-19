using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using Spartans.UI;

namespace Spartans.Players
{
    [RequireComponent(typeof(AnimationManager), typeof(Rigidbody))]
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] public GameObject worldSpaceCanvas;
        [SerializeField] public Transform lookAtPoint;
        [SerializeField] public AnimationManager _animationManager;
        [SerializeField] public Transform spine;
        [SerializeField] public string playerName{ get; private set; }
        [SerializeField] private bool _canJump = true;
        [SerializeField] private bool _grounded = false;
        [SerializeField] private float _jumpForce = 12.0f;
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _MAX_SPEED = 8.0f;
        [SerializeField] private float _mouseSens = 1.0f;
        [SerializeField] private Vector3 _lastSentInput;
        [SerializeField] private Vector3 input;

        
        private PageUI _pauseScreen;
        private GameManager _gameManager;
        private CanvasManager _HUD;
        private Rigidbody _rigidbody;
        private Animator _animator;
        private ClassController _classController;
        private Health _myHealth;
        

        public void Awake(){
            _rigidbody = GetComponent<Rigidbody>();
            _myHealth = GetComponent<Health>();
            _animationManager = GetComponent<AnimationManager>();
            _gameManager = FindObjectOfType<GameManager>();
            _HUD = _gameManager._canvasManager;
            _pauseScreen = _HUD.transform.Find("PauseScreen").GetComponent<PageUI>();
        }

        public void Start(){
            playerName = "Player " + NetworkObjectId;
            _animator = GetComponentInChildren<Animator>();
            _classController = GetComponent<ClassController>();
            //previously in start
            _myHealth = GetComponent<Health>();
            _myHealth.onDie += OnDieCallback;
            _myHealth.onRespawn += OnRespawnCallback;

            //isLocalPlayer makes anything in player scripts happen only on 1 time because theres only 1 player object
            if(IsClient && IsOwner){
                PlayerCameraFollow.Instance.FollowPlayer(transform.GetChild(0).transform);
                PlayerCameraFollow.Instance.LookAtPlayer(transform.GetChild(0).transform);
                transform.GetComponentInChildren<FloatingHealth>().camTransform = PlayerCameraFollow.Instance.camera.transform;
                worldSpaceCanvas.GetComponent<Canvas>().worldCamera = PlayerCameraFollow.Instance.camera;
                worldSpaceCanvas.GetComponentInChildren<FloatingHealth>().camTransform = PlayerCameraFollow.Instance.camera.transform;
            }

            
            _myHealth.Init(this);
            _classController.Init(this);
        }
        
        // Update is called once per frame
        void Update()
        {
            if(_animationManager == null || _animator == null){

                print("Assign an animator/animationManager dummy!!!");
                return;
            }
            //Not player character of this client session
            if(!IsLocalPlayer){
                return;
            }

            if(PlayerInput.Instance.escape)
            {
                TogglePauseMenu();
            }

            //Jump
            if(PlayerInput.Instance.jump && _canJump && _grounded){
                RequestJumpServerRpc();
                _canJump = false;
                _grounded = false;
            }

            //Update rotation
            requestRotationServerRpc(PlayerInput.Instance.mouseX*_mouseSens, -PlayerInput.Instance.mouseY*_mouseSens);
            
            //Update movement
            input = PlayerInput.Instance.movementDir;
            if(input != _lastSentInput){
                _lastSentInput = input;
            }
        }

        //server needs to update grounded state for all players on server side
        //if not the server or a local player object, aka the player spawned when join game, dont update
        void FixedUpdate(){
            if(IsServer && !IsLocalPlayer){
                CheckGrounded();
            }
            if(IsLocalPlayer){
                CheckGrounded();
                requestMoveServerRpc(input);
            }
        }

    
        [ClientRpc]
        public void JumpResponseClientRpc(){
            Vector3 horizPlane = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
            _rigidbody.AddForce(transform.up * _jumpForce, ForceMode.VelocityChange);
            StartCoroutine(ResetJump());
        }

        [ServerRpc]
        public void RequestJumpServerRpc(){
            if(!_grounded) return;
            _canJump = false;
            _grounded = false;
            _animationManager.SetParameter("grounded", false);
            StartCoroutine(ResetJump());
            JumpResponseClientRpc();
        }

        [ServerRpc]
        public void requestRotationServerRpc(float rotX, float rotY){
            if (rotX != 0){
                transform.Rotate(new Vector3(0, rotX, 0));
            }
            if (rotY != 0){
                lookAtPoint.Rotate(new Vector3(rotY, 0, 0));
            }
        }

        [ServerRpc]
        public void requestMoveServerRpc(Vector3 dir){
            if(!_grounded) return;
            Vector3 moveDir = dir.normalized;
            _animationManager.SetParameter("speed", dir.magnitude);
            if (moveDir == Vector3.zero && _grounded){
                _rigidbody.velocity = Vector3.zero;
            }
            
            Vector2 horizPlane = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.z);
            float velocityComponentY = _rigidbody.velocity.y;
            if(horizPlane.magnitude < _MAX_SPEED){
                moveDir = transform.TransformDirection(moveDir);
                _rigidbody.AddForce(moveDir*_moveSpeed, ForceMode.VelocityChange);
            }else if(horizPlane.magnitude >= _MAX_SPEED){
                _rigidbody.velocity = _rigidbody.velocity.normalized*_MAX_SPEED;
                return;
            }
        }

        void CheckGrounded(){
            if(!_canJump || _grounded) return;
            RaycastHit hit;
            bool hitOccured = Physics.Raycast(transform.position-(Vector3.down*0.5f), Vector3.down, out hit, 0.6f, 1);
            Debug.DrawRay(transform.position-(Vector3.down*0.5f), Vector3.down * 0.6f, Color.blue);

            if(hitOccured && !_grounded){
                //check if we are the server because This func gets run on server and client
                //thus it would produce a duplicate instruction to change the Animator parameter
                if(IsServer){
                  _animationManager.SetParameter("grounded", true);
                }
                _grounded = true;
            }else if(!hitOccured){
                _grounded = false;
            }else{
                print("Unanticipated condition occured");
            }
        }

        void TogglePauseMenu()
        {
            //Handle closing of open UI pages but exclude base PauseScreen case
            if(_HUD.GetStackCount() > 2){
                _HUD.PopPage();
                return;
            }

            //Handle closing and opening of base PauseScreen UI
            if(_pauseScreen.gameObject.activeSelf){
                _HUD.PopPage();
                MouseLock(true);
            }else{
                _HUD.PushPage(_pauseScreen);
                MouseLock(false);
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

        private void OnDieCallback(){
            this.GetComponent<Rigidbody>().useGravity = false;
            this.GetComponent<BoxCollider>().enabled = false;
            this.enabled = false;
        }

        private void OnRespawnCallback()
        {
           this.GetComponent<Rigidbody>().useGravity = true;
            this.GetComponent<BoxCollider>().enabled = true;
            this.enabled = true;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
        }
        IEnumerator ResetJump(){
            yield return new WaitForSeconds(0.5f);
            _canJump = true;
        }
    }
}
