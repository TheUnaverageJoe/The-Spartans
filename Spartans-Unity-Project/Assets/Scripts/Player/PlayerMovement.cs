using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Spartans.Players
{
    public class PlayerMovement : NetworkBehaviour
    {
        private Animator anim;
        private Rigidbody rb;
        
        //Network Variable neccessary for syncing with server,
        //because we must update client side from the info on server side
        public NetworkVariable<Vector3> userInput = new NetworkVariable<Vector3>();
        private Vector2 _inputDirection = new Vector2(0,0);
        

        //primative class variables-----
        [SerializeField] public bool isGrounded{ get; private set; }
        [SerializeField] private float MAX_SPEED = 1.0f;
        [SerializeField] private float MOVE_SPEED = 0.1f;
        [SerializeField] private float X_SENS = 1.0f;

        //------------------------------
        // Start is called before the first frame update
        public void Init(Rigidbody rigidbody, Animator animator){
            rb = rigidbody;
            anim = animator;
            /*
            if(rb == null || anim == null){
                Debug.LogError("FROM PlayerMovement: rb =" + rb + " anim = " + anim);
            }else{
                print("All G");
            }
            */

            //class variable init
            isGrounded = false;

            //print("init called from player movement");

            //EXAMPLE LINE
            userInput.OnValueChanged += (Vector3 prev, Vector3 updated) => {
                
            };
                    
        }
        

        // Update is called once per frame
        void Update()
        {
            /*
            if(!IsLocalPlayer) return; //Ensure Each client only moves there own player, 
            //I dont understand why this is neccessary given each player should have their own script instance for the player character and own game window in focus

            if((Input.GetAxis("Horizontal")!=0 || Input.GetAxis("Vertical")!=0) && rb.velocity != Vector3.zero){
                anim.SetBool("isWalking", true);//change animator variable for walking and Idle
            }else{
                anim.SetBool("isWalking", false);//change animator variable for walking and Idle
            }

            if(Input.GetKeyDown("f")){
                anim.SetBool("throw", true);
                StartCoroutine("throwAnimationTime");
            }
            */

            //print("transform of: " + NetworkObject.NetworkObjectId + " is " + userInput.Value);
            //if(IsClient) transform.position = userInput.Value;
            //rb.velocity += userInput.Value;
            //print("Transform pos is: " + transform.position + "from player " +  NetworkObjectId);
        }

        void FixedUpdate(){
            if(IsClient && IsOwner){
                Look();
                UpdateClientInput();
                //print("is client and owner");
            }     
            if(IsServer){
                //print("Server side var values: " + inputDirection + " for " + NetworkObjectId);
                UpdateServer();
                //print("is server");
            }
            
            //if(!IsLocalPlayer) return;//Ensure Each client only moves there own player,
            //I dont understand why this is neccessary given each player should have their own script instance for the player character and own game window in focus
            //Debug.Log("In Fixed Update");
            //EDIT DATE 3/8/2022: This is because every running .exe file has all exact same set of objects in it.
            //Thus each client has multiple player objects in the hierarchy

            //binary conversion, layer mask should be an int(Layermask is parameter of OverlapBox), convert it to binary to see which layers are true(1) or false(0)
            //00000000000000000000000000000001(Binary) = 1(Integer)
            // summation of 2^(Layer Number) for all layers that should be used for overlap
            //aka layer 0 will trigger OverlapBox because 2^0 = 1

            //only detect objects on layer 0, "default" layer

            //if(!IsLocalPlayer) return;
            //Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, new Vector3(1, 0.5f, 0.5f),
            //                                            Quaternion.identity, 1, QueryTriggerInteraction.Ignore);
            //if(hitColliders.Length == 0) isGrounded = false;
            //if(hitColliders.Length > 0) isGrounded = true;
            
            
        }

        //UpdateServer is only called from Server in fixedUpdate
        private void UpdateServer(){
            //make each player obj update their isGrounded var
            Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, new Vector3(1, 0.5f, 0.5f),
                                                        Quaternion.identity, 1, QueryTriggerInteraction.Ignore);
            if(hitColliders.Length == 0) isGrounded = false;
            if(hitColliders.Length > 0) isGrounded = true;
            //use isGrounded to determine if we are allowed to move
            if(!isGrounded){
                //print("client with gameObj ID: " + NetworkObjectId + " is not grounded");
                return;
            }
            
            //print("Object: " + NetworkObjectId + " tried to move " + inputDirection);
            userInput.Value = _inputDirection;
        }

        [ServerRpc]
        private void UpdateServerRpc(Vector2 input){
            //need to update _inputDirection so that local version of obj on server side knows what the inputs were to work from
            _inputDirection = new Vector3(input.x, 0 , input.y);

            if(isGrounded){
                UpdateClientRpc(input);
            }
        }

        //Take in local user input and send it to the server version of that player obj
        private void UpdateClientInput(){
            //Get Input manager inputs for Up down arrow keys and left right arrow keys
            float inputVert = Input.GetAxisRaw("Vertical");
            float inputHoriz = Input.GetAxisRaw("Horizontal");

            //create a object space vector3 with inputs and normalize 
            Vector2 input = new Vector2(inputHoriz, inputVert).normalized;

            UpdateServerRpc(input);
        }

        [ClientRpc]
        private void UpdateClientRpc(Vector2 moveDir){
            //isolate x and z components for check against MAX_SPEED
            //if we consider the y component then falling fast could prevent movement
            //on second thought it doesnt matter because we cant move when falling anyway     
            Vector2 rbVelocityXZ = new Vector2(rb.velocity.x, rb.velocity.z);
            Vector3 converter = new Vector3 (rbVelocityXZ.x, 0.0f , rbVelocityXZ.y);
            float rbVelocityY = rb.velocity.y;

            if(isGrounded)

            //if speed <= Max speed allow more input
            //<= is important for when we are already moving max speed and want to change dir
            if((rbVelocityXZ + moveDir).magnitude < MAX_SPEED){
                rb.velocity += new Vector3(moveDir.x*MOVE_SPEED, rbVelocityY, moveDir.y*MOVE_SPEED);
            }else{
                //print("MAX SPEED HIT");
                if(Vector2.Dot(rbVelocityXZ.normalized, moveDir) > 0.95f){
                    
                } 
            }
        }

        void Look(){
            transform.Rotate(0,Input.GetAxis("Mouse X")*X_SENS,0);
        }

        private IEnumerator throwAnimationTime(){
            yield return new WaitForSeconds(1);
            anim.SetBool("throw", false);
        }
    }
}
