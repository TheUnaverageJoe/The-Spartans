using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


namespace Spartans.Players{
    public class PlayerMove : NetworkBehaviour
    {
        [SerializeField] public Transform lookAtPoint;
        private Player _player;
        [SerializeField] public Transform spine;
        private Rigidbody _rigidbody;
        private Animator _animator;
        private Vector3 _lastSentInput;
        private Vector3 input;
        //private Camera _camera;
        private Health _myHealth;
        [SerializeField] private bool canJump = true;
        [SerializeField] private bool grounded = false;
        [SerializeField] private float jumpForce = 8.0f;
        [SerializeField] private float moveSpeed = 0.05f;
        [SerializeField] private float MAX_SPEED = 3.0f;
        [SerializeField] private float mouseSens = 1.0f;
        //[SerializeField] private bool timer = false;
        private Coroutine groundingTimer;
        private int previousState = -1;
        private int currentState = -1;

        //public event Action onSpacePress;

        enum States{Grounded, Airborn}


        public void Init(Rigidbody rigidbody){
            _rigidbody = rigidbody;
            _animator = GetComponentInChildren<Animator>();

            previousState = -1;
            currentState = -1;

            //previously in start
            //_camera = GetComponentInChildren<Camera>();
            _myHealth = GetComponent<Health>();
            _player = GetComponent<Player>();

            //This is already being done in Player on line 35
            //if(!IsLocalPlayer){
            //    _camera.gameObject.SetActive(false);
            //}
            //_myHealth.onKilledBy += OnDieCallback;
            _myHealth.onRespawn += OnRespawnCallback;
        }

        // Update is called once per frame
        void Update()
        {
            //Not player character of this client session
            if(!IsLocalPlayer){
                return;
            }
            if(_animator == null || _player._animationManager == null){

                print("Assign an animator/animationManager dummy!!!");
                return;
            }
            //Jump
            if(Input.GetButtonDown("Jump") && canJump && grounded){
                RequestJumpServerRpc();
                canJump = false;
                grounded = false;
            }
            //Update rotation
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            requestRotationServerRpc(mouseX*mouseSens, -mouseY*mouseSens);
            float rotMouseY = -mouseY*mouseSens;
            if (rotMouseY != 0){
                lookAtPoint.Rotate(new Vector3(rotMouseY, 0, 0));
                //lookAtPoint.eulerAngles = new Vector3(Mathf.Clamp(lookAtPoint.eulerAngles.x, -75.0f, 75.0f),lookAtPoint.eulerAngles.y ,lookAtPoint.eulerAngles.z );
            }
            
            //Update movement
            //if(grounded){
            input = new Vector3(Input.GetAxisRaw("Horizontal"), 0 , Input.GetAxisRaw("Vertical"));
            if(input != _lastSentInput){
                _lastSentInput = input;
            }
            //}
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

        [ServerRpc]
        public void RequestJumpServerRpc(){
            //print("Server and client grounded var out of sync");
            if(!grounded) return;
            canJump = false;
            grounded = false;
            //_rigidbody.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
            JumpResponseClientRpc();
        }

        [ClientRpc]
        public void JumpResponseClientRpc(){
            Vector3 horizPlane = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
            //_rigidbody.velocity = horizPlane.normalized;
            _rigidbody.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
            StartCoroutine(ResetJump());
        }

        IEnumerator ResetJump(){
            yield return new WaitForSeconds(0.5f);
            canJump = true;
        }

        [ServerRpc]
        public void requestRotationServerRpc(float rotX, float rotY){
           
            if (rotX != 0){
                transform.Rotate(new Vector3(0, rotX, 0));
            }
            if (rotY != 0){
                
                lookAtPoint.Rotate(new Vector3(rotY, 0, 0));
                //lookAtPoint.eulerAngles = new Vector3(Mathf.Clamp(lookAtPoint.eulerAngles.x, -75.0f, 75.0f),lookAtPoint.eulerAngles.y ,lookAtPoint.eulerAngles.z );
            }
        }


        [ServerRpc]
        public void requestMoveServerRpc(Vector3 dir){
            if(!grounded) return;
            Vector3 moveDir = dir.normalized;
            _animator.SetFloat("speed", dir.magnitude);
            //_player._animationManager.SetParameter("speed", dir.magnitude);
            if (moveDir == Vector3.zero && grounded && canJump){
                _rigidbody.velocity = Vector3.zero;
            }
            
            Vector2 horizPlane = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.z);
            float velocityComponentY = _rigidbody.velocity.y;

            if(horizPlane.magnitude > MAX_SPEED){
                //print("TOO FAST" + horizPlane.magnitude);
                horizPlane = horizPlane.normalized*MAX_SPEED;
                Vector3 normalized = new Vector3(horizPlane.x, velocityComponentY, horizPlane.y);
                _rigidbody.velocity = _rigidbody.velocity.normalized*MAX_SPEED;
                return;
            }

            if(horizPlane.magnitude < MAX_SPEED){
                moveDir = transform.TransformDirection(moveDir);
                _rigidbody.AddForce(moveDir*moveSpeed, ForceMode.VelocityChange);
            }
            if(horizPlane.magnitude == MAX_SPEED){
                //check if vectors point same direction
                if(Vector3.Dot(moveDir, new Vector3(horizPlane.x,0,horizPlane.y).normalized) == 1){
                    print("denied movement change, cant accelerate further");
                    return;
                }else{
                    if(!grounded) return;
                    moveDir = transform.TransformDirection(moveDir);
                    _rigidbody.AddForce(moveDir.normalized*moveSpeed, ForceMode.VelocityChange);
                }
            }
        }

        void CheckGrounded(){
            //if(!canJump) return;
            RaycastHit hit;
            bool hitOccured = Physics.Raycast(transform.position-(Vector3.down*0.5f), Vector3.down, out hit, 0.6f, 1);
            Debug.DrawRay(transform.position-(Vector3.down*0.6f), Vector3.down * 0.5f, Color.blue);
            
            if (hitOccured){
                currentState = (int)States.Grounded;
                //_player._animationManager.SetParameter("grounded", true);
                _animator.SetBool("grounded", true);
            }else{
                currentState=(int)States.Airborn;
                //_player._animationManager.SetParameter("grounded", false);
                _animator.SetBool("grounded", false);
            }
            
            if(previousState == (int)States.Grounded && currentState == (int)States.Grounded && canJump){
                grounded = true;
            }else if(previousState == (int)States.Grounded && currentState == (int)States.Grounded && !canJump){
                StartCoroutine(checkGrounded());
                grounded=false;
            }else if(previousState == (int)States.Grounded && currentState==(int)States.Airborn){
                grounded = false;
            }else if(previousState==(int)States.Airborn && currentState==(int)States.Grounded){
                grounded=true;
            }else if(previousState == (int)States.Airborn && currentState == (int)States.Airborn){
                grounded = false;
            }
            previousState=currentState;
        }
        private IEnumerator checkGrounded(){
            yield return new WaitForSeconds(0.2f);
            if(currentState==(int)States.Grounded){
                //print("We have been cleared for take off");
                grounded=true;
            }
            //timer = false;
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
    }
}
