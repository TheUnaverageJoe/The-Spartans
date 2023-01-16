using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using Spartans.UI;
using UnityEngine.InputSystem;

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
        [SerializeField] public bool IsNPC; // Used to isolate target dummy logic
        private Vector3 input;
        [SerializeField] private Vector2 ClientInput;
        private bool _sprinting;
        
        private NetworkVariable<Teams> myTeam = new NetworkVariable<Teams>();
        private PageUI _pauseScreen;
        private GameManager _gameManager;
        private CanvasManager _HUD;
        private Rigidbody _rigidbody;
        private ClassController _classController;
        private Health _myHealth;
        private FlagCarrier _flagCarrier;

        private void Init()
        {
            if(NetworkObject.IsSceneObject == true || NetworkObject.IsSceneObject == null && IsNPC)
            {
                //print("Is Scene Object: " + NetworkObject.IsSceneObject);
                _myHealth = GetComponent<Health>();
                _animationManager = GetComponent<AnimationManager>();
                _gameManager = FindObjectOfType<GameManager>();
                //Debug.Log("Ran Init() for In scene");
                return;
            }
            _rigidbody = GetComponent<Rigidbody>();
            _myHealth = GetComponent<Health>();
            _animationManager = GetComponent<AnimationManager>();
            _classController = GetComponent<ClassController>();
            _flagCarrier = GetComponent<FlagCarrier>();
            _gameManager = FindObjectOfType<GameManager>();
            _HUD = _gameManager._canvasManager;
            _pauseScreen = _HUD.transform.Find("PauseScreen").GetComponent<PageUI>();
            //Debug.Log("Ran Init()");
        }

        public void Start()
        {
            if(NetworkManager.Singleton == null || IsNPC)
            {
                //Debug.LogWarning("Player Controller started as offline or NPC/Target DUmmy");
                //Debug.Log("myTeam is: " + myTeam.Value);
                Init();
                _myHealth.Init(this);
            }
            //_myHealth = GetComponent<Health>();
            _myHealth.OnKilledBy += OnDieCallback;
            _myHealth.OnRespawn += OnRespawnCallback;

            if(NetworkObject.IsSceneObject == true || NetworkObject.IsSceneObject == null && IsNPC)
            {
                //print(NetworkObject.IsSceneObject);
                return;
            }
            //isLocalPlayer makes anything in player scripts happen only on 1 time because theres only 1 player object
            if(IsClient && IsOwner && IsLocalPlayer){
                //PlayerControls.PlayerActions inputEvents = InputManager.Instance.CurrentActionMap();
                //inputEvents.Interact.performed += TryInteract;
                InputManager.Instance.OnInteract += TryInteract;
                InputManager.Instance.OnEscape += Escape;
                InputManager.Instance.OnJump += TryJump;

                InputManager.Instance.OnMove += UpdateMoveInput;
                InputManager.Instance.OnLook += Look;
                InputManager.Instance.OnSprint += Sprint;

                PlayerCameraFollow.Instance.FollowPlayer(transform.GetChild(0).transform);
                PlayerCameraFollow.Instance.LookAtPlayer(transform.GetChild(0).transform);
                transform.GetComponentInChildren<FloatingHealth>().camTransform = PlayerCameraFollow.Instance.camera.transform;
                worldSpaceCanvas.GetComponent<Canvas>().worldCamera = PlayerCameraFollow.Instance.camera;
                worldSpaceCanvas.GetComponentInChildren<FloatingHealth>().camTransform = PlayerCameraFollow.Instance.camera.transform;
            }

            
            _myHealth.Init(this);
            _classController.Init(this);
            if(myTeam.Value != Teams.Neutral)
            {
                //print("Flag carrier Team Value: " + myTeam.Value);
                _flagCarrier.Init(this);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(IsNPC)
            {
                return;
            }
            if(IsServer){
                Move(ClientInput);
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
                //requestMoveServerRpc(input);
            }
        }

        public override void OnNetworkSpawn()
        {
            if(IsNPC){
                return;
            }
            Init();
            playerName = "Player " + OwnerClientId.ToString();
            transform.GetComponentInChildren<FloatingHealth>().ChangeName(playerName);
            myTeam.OnValueChanged += SetTeamColor;
            //myTeam.OnValueChanged += (prev, newVal)=>{print("Team value changed:" + newVal);};
            //print("States SCOL: " + IsServer + IsClient + IsOwner + IsLocalPlayer);
        }
        //InputAction.CallbackContext context
        private void TryInteract()
        {
            _flagCarrier.InteractFlag();
        }

        private void Escape()
        {
            TogglePauseMenu();
        }
        private void TryJump()
        {
            if(_canJump && _grounded){
                RequestJumpServerRpc();
                // _canJump = false;
                // _grounded = false;
            }
        }

        private void Sprint()
        {
            if(IsServer)
            {
                if(!_sprinting)
                {
                    _MAX_SPEED = _MAX_SPEED*5;
                    _sprinting = true;
                }
                else
                {
                    _MAX_SPEED = _MAX_SPEED/5;
                    _sprinting = false;
                }
            }
            else
            {
                SprintServerRpc();
            }
            
        }
        [ServerRpc]
        private void SprintServerRpc()
        {
            if(!_sprinting)
            {
                _MAX_SPEED = _MAX_SPEED*5;
                _sprinting = true;
            }
            else
            {
                _MAX_SPEED = _MAX_SPEED/5;
                _sprinting = false;
            }
        }

        private void Look(Vector2 vector2)
        {
            //print("updated look");
            RequestRotationServerRpc(vector2.x*_mouseSens, -vector2.y*_mouseSens);
        }

        [ServerRpc]
        public void RequestRotationServerRpc(float rotX, float rotY){
            if (rotX != 0){
                transform.Rotate(new Vector3(0, rotX, 0));
            }
            if (rotY != 0){
                lookAtPoint.Rotate(new Vector3(rotY, 0, 0));
            }
        }

        private void UpdateMoveInput(Vector2 vector2)
        {
            SendMovementInputServerRpc(vector2);
        }

        [ServerRpc]
        private void SendMovementInputServerRpc(Vector2 input)
        {
            this.ClientInput = input;
        }

        [ClientRpc]
        public void JumpResponseClientRpc(){
            JumpStarted();
            Vector3 horizPlane = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
            _rigidbody.AddForce(transform.up * _jumpForce, ForceMode.VelocityChange);
        }

        [ServerRpc]
        public void RequestJumpServerRpc(){
            if(!_grounded) return;
            JumpStarted();
            _animationManager.SetParameter("grounded", false);
            JumpResponseClientRpc();
        }


        private void Move(Vector2 inputDir){
            if(!_grounded) return;
            Vector2 moveDir = inputDir.normalized;
            _animationManager.SetParameter("speed", inputDir.magnitude);
            if (moveDir == Vector2.zero && _grounded){
                _rigidbody.velocity = Vector3.zero;
            }
            
            Vector2 horizPlane = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.z);
            float velocityComponentY = _rigidbody.velocity.y;
            if(horizPlane.magnitude < _MAX_SPEED){
                Vector3 moveDirWldSpace = transform.TransformDirection(new Vector3(moveDir.x,0, moveDir.y));
                _rigidbody.AddForce(moveDirWldSpace*_moveSpeed*10, ForceMode.Force);
            }else if(horizPlane.magnitude >= _MAX_SPEED){
                _rigidbody.velocity = _rigidbody.velocity.normalized*_MAX_SPEED;
                return;
            }
        }

        void CheckGrounded(){
            if(!_canJump) return;
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
                //ALREADY ON GROUND IN THIS CASE
                //print("Unanticipated condition occured");
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
                InputManager.Instance.ResumeInput();
                MouseLock(true);
            }else{
                InputManager.Instance.PauseInput();
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

        private void OnDieCallback(Teams team){
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

        public override void OnNetworkDespawn()
        {
            Debug.Log("Despawned player " + playerName);
            base.OnNetworkDespawn();
            if(IsClient && IsOwner)
            {
                InputManager.Instance.OnInteract -= TryInteract;
                InputManager.Instance.OnEscape -= Escape;
                InputManager.Instance.OnJump -= TryJump;

                InputManager.Instance.OnMove -= UpdateMoveInput;
                InputManager.Instance.OnLook -= Look;
                InputManager.Instance.OnSprint -= Sprint;
            }
        }
        IEnumerator ResetJump(){
            yield return new WaitForSeconds(0.5f);
            _canJump = true;
        }
        public void JumpStarted(){
            _canJump = false;
            _grounded = false;
            StartCoroutine(ResetJump());
        }
        public bool IsAirborn(){
            return !_grounded;
        }

        public void ChangeTeam(Teams team)
        {
            SetTeamColor(Teams.Neutral, team);
            if(myTeam.Value == team) return;

            myTeam.Value = team;
        }

        public System.Nullable<Teams> GetTeamAssociation(){
            if(!IsServer){
                Debug.LogError("GetTeamAssociation can only be called from server");
                return null;
            }

            //print("myTeam var is: " + myTeam.Value);
            return myTeam.Value;
        }
        private void SetTeamColor(Teams prevTeam, Teams currentTeam)
        {
            Transform colorRegion;

            if(_classController.GetType() == typeof(Spartans.Players.ShieldBarerController))
            {
                colorRegion = transform.GetChild(2);
            }else if(_classController.GetType() == typeof(Spartans.Players.ArcherController)){
                colorRegion = transform.GetChild(1).GetChild(0);
            }else
            {
                colorRegion = transform.GetChild(1).GetChild(4);
            }
            switch (currentTeam)
            {
                case Teams.Neutral:
                    colorRegion.GetComponent<Renderer>().material.color = new Color(127, 0, 204); // Color = Purple
                    break;
                case Teams.Red:
                    colorRegion.GetComponent<Renderer>().material.color = Color.red;
                    break;
                case Teams.Blue:
                    colorRegion.GetComponent<Renderer>().material.color = Color.blue;
                    break;
                case Teams.Purple:
                    colorRegion.GetComponent<Renderer>().material.color = Color.magenta;
                    break;
                case Teams.Green:
                    colorRegion.GetComponent<Renderer>().material.color = Color.green;
                    break;
            }
        }
    }
}
