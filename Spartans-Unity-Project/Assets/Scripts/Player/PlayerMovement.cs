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
        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
        public Vector3 moveDirection = new Vector3(0,0,0);
        

        //primative class variables-----
        [SerializeField] public bool isGrounded{ get; private set; }
        [SerializeField] private float MAX_SPEED = 10.0f;
        [SerializeField] private float MOVE_SPEED = 1.0f;
        [SerializeField] private float X_SENS = 1.0f;

        //------------------------------
        // Start is called before the first frame update
        public void Init(){
            anim = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();

            //class variable init
            isGrounded = false;

            if(IsClient && !IsServer){
                Position.OnValueChanged += (Vector3 prev, Vector3 updated) => {
                    if(prev != updated){
                        transform.position = updated;
                    }
                    else{
                        //print("prev and update are:" + prev + " , " + updated);
                    }
                };
            }
        }
        

        // Update is called once per frame
        void Update()
        {
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
            //print("transform of: " + NetworkObject.NetworkObjectId + " is " + Position.Value);
            //transform.position = Position.Value;
            //print("Transform pos is: " + transform.position + "from player " +  NetworkObjectId);

        }

        void FixedUpdate(){        
            if(IsServer){
                //print("Server side var values: " + moveDirection + " for " + NetworkObjectId);
                UpdateServer();
                //print("is server");
            }
            if(IsClient && IsOwner){
                Look();
                UpdateClient();
                //print("is client and owner");
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

        private void UpdateServer(){
            Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, new Vector3(1, 0.5f, 0.5f),
                                                        Quaternion.identity, 1, QueryTriggerInteraction.Ignore);
            if(hitColliders.Length == 0) isGrounded = false;
            if(hitColliders.Length > 0) isGrounded = true;
            if(!isGrounded){
                print("client with gameObj ID: " + NetworkObjectId + " is not grounded");
                return;
            }
            print("Object: " + NetworkObjectId + " tried to move " + moveDirection);
            Position.Value += moveDirection;
        }
        [ServerRpc]
        private void UpdateServerRpc(Vector3 dir, ulong id){
            moveDirection = dir;
            UpdateClientRpc();
            Debug.LogError("Server is clientID: " + NetworkManager.Singleton.LocalClientId);
            Debug.LogError("Server Rpc Called from client: " + id);
        }

        private void UpdateClient(){
            //Get Input manager inputs for Up down arrow keys and left right arrow keys
            float inputVert = Input.GetAxis("Vertical");
            float inputHoriz = Input.GetAxis("Horizontal");
            //create a object space vector3 with inputs and normalize 
            Vector3 input = new Vector3(inputHoriz, 0, inputVert).normalized;
            //transform vector3 to world space to get desired direction of translation
            input = transform.TransformDirection(input) * MOVE_SPEED;
            //print("Getting Input from client: " + NetworkManager.Singleton.LocalClientId);

            moveDirection = input;
            //print("input: " + input);
            UpdateServerRpc(input, NetworkManager.Singleton.LocalClientId);
        }

        [ClientRpc]
        private void UpdateClientRpc(){
            transform.position = Position.Value;
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
