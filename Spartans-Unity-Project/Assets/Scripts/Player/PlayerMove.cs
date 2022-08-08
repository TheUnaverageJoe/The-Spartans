using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


namespace Spartans.Players{
    public class PlayerMove : NetworkBehaviour
    {
        private Rigidbody _rigidbody;
        private Animator _animator;
        private Vector3 _lastSentInput;
        private Camera _camera;
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

        public event Action onSpacePress;

        enum States{Grounded, Airborn}


        public void Init(Rigidbody rigidbody, Animator animator){
            _rigidbody = rigidbody;
            _animator = animator;

            previousState = -1;
            currentState = -1;

            Physics.gravity = new Vector3(0, -20f, 0);
            //previously in start
            _camera = GetComponentInChildren<Camera>();

            //This is already being done in Player on line 35
            //if(!IsLocalPlayer){
            //    _camera.gameObject.SetActive(false);
            //}
        }

        // Update is called once per frame
        void Update()
        {
            //Not player character of this client session
            if(!IsLocalPlayer){
                return;
            }
            //CheckGrounded();
            //Jump
            if(Input.GetButtonDown("Jump") && canJump && grounded){
                RequestJumpServerRpc();
                canJump = false;
                grounded = false;
                //print(_rigidbody.velocity);
            }
            //Update rotation
            float mouseX = Input.GetAxis("Mouse X");
            if (mouseX != 0){
                requestRotationServerRpc(mouseX*mouseSens);
            }
            //Update movement
            if(grounded){
                Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0 , Input.GetAxis("Vertical"));
                //print(input);
                if(input != _lastSentInput){
                    requestMoveServerRpc(input);
                    _lastSentInput = input;
                }
                //print("After Move: " + _rigidbody.velocity);
            }
        }
        void FixedUpdate(){
            if(!IsOwner) return;
            CheckGrounded();
        }

        [ServerRpc]
        public void RequestJumpServerRpc(){
            //print("Server and client grounded var out of sync");
            if(!grounded) return;
            //_rigidbody.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
            JumpResponseClientRpc();
        }

        [ClientRpc]
        public void JumpResponseClientRpc(){
            Vector3 horizPlane = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
            _rigidbody.velocity = horizPlane;
            _rigidbody.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
            StartCoroutine(ResetJump());
        }

        IEnumerator ResetJump(){
            yield return new WaitForSeconds(0.5f);
            canJump = true;
        }

        [ServerRpc]
        public void requestRotationServerRpc(float rotX){
            transform.Rotate(new Vector3(0, rotX, 0));
        }
        [ServerRpc]
        public void requestMoveServerRpc(Vector3 dir){
            Vector3 moveDir = dir.normalized;
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
                    moveDir = transform.TransformDirection(moveDir);
                    _rigidbody.AddForce(moveDir*moveSpeed, ForceMode.VelocityChange);
                }
            }
        }

        void CheckGrounded(){
            
            RaycastHit hit;
            bool hitOccured = Physics.Raycast(transform.position-Vector3.down*0.5f, Vector3.down, out hit, 1.15f, 1);
            Debug.DrawRay(transform.position-Vector3.down*0.5f, Vector3.down * 1.15f, Color.blue);
            
            if (hitOccured){
                currentState = (int)States.Grounded;
            }else{
                currentState=(int)States.Airborn;
            }
            
            if(previousState == (int)States.Grounded && currentState == (int)States.Grounded && canJump){
                grounded = true;
            }else if(previousState == (int)States.Grounded && currentState == (int)States.Grounded && !canJump){
                StartCoroutine(checkGrounded());
                //timer=true;
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
                print("We have been cleared for take off");
                grounded=true;
                _rigidbody.AddForce(transform.up*(jumpForce),ForceMode.VelocityChange);
            }
            //timer = false;
        }
    }
}
