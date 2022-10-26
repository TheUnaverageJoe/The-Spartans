using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


namespace Spartans.Players{
    public class PlayerMoveRefactor : NetworkBehaviour
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
        [SerializeField] private bool _canJump = true;
        [SerializeField] private bool _grounded = false;
        [SerializeField] private float _jumpForce = 12.0f;
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _MAX_SPEED = 8.0f;
        [SerializeField] private float _mouseSens = 1.0f;
        //[SerializeField] private bool timer = false;


        public void Init(Rigidbody rigidbody){
            _rigidbody = rigidbody;
            _animator = GetComponentInChildren<Animator>();

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
            if(Input.GetButtonDown("Jump") && _canJump && _grounded){
                RequestJumpServerRpc();
                _canJump = false;
                _grounded = false;
            }
            //Update rotation
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            requestRotationServerRpc(mouseX*_mouseSens, -mouseY*_mouseSens);
            float rotMouseY = -mouseY*_mouseSens;
            /*
            if (rotMouseY != 0){
                lookAtPoint.Rotate(new Vector3(rotMouseY, 0, 0));
                //lookAtPoint.eulerAngles = new Vector3(Mathf.Clamp(lookAtPoint.eulerAngles.x, -75.0f, 75.0f),lookAtPoint.eulerAngles.y ,lookAtPoint.eulerAngles.z );
            }
            */
            
            //Update movement
            //if(_grounded){
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
            if(!_grounded) return;
            _canJump = false;
            _grounded = false;
            _player._animationManager.SetParameter("grounded", false);
            StartCoroutine(ResetJump());
            //_rigidbody.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
            JumpResponseClientRpc();
        }

        [ClientRpc]
        public void JumpResponseClientRpc(){
            Vector3 horizPlane = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
            //_rigidbody.velocity = horizPlane.normalized;
            _rigidbody.AddForce(transform.up * _jumpForce, ForceMode.VelocityChange);
            StartCoroutine(ResetJump());
        }

        IEnumerator ResetJump(){
            yield return new WaitForSeconds(0.5f);
            _canJump = true;
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
            if(!_grounded) return;
            Vector3 moveDir = dir.normalized;
            //_animator.SetFloat("speed", dir.magnitude);
            _player._animationManager.SetParameter("speed", dir.magnitude);
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
                  _player._animationManager.SetParameter("grounded", true);
                }
                _grounded = true;
            }else if(!hitOccured){
                _grounded = false;
            }else{
                //if()
                print("Unanticipated condition occured");
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
    }
}
