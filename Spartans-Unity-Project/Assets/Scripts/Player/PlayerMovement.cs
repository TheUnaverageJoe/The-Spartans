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
        Vector3 moveDirection = new Vector3(0,0,0);
        

        //primative class variables-----
        [SerializeField] private bool isGrounded;
        [SerializeField] private float MAX_SPEED = 10.0f;
        [SerializeField] private float MOVE_SPEED = 1f;
        [SerializeField] private float X_SENS = 1.0f;

        //------------------------------
        // Start is called before the first frame update
        public void Init(){
            anim = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();

            //class variable init
            isGrounded = false;
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
            transform.position = Position.Value;
            //print("Transform pos is: " + transform.position);

        }

        void FixedUpdate(){
            if(!IsLocalPlayer) return;//Ensure Each client only moves there own player,
            //I dont understand why this is neccessary given each player should have their own script instance for the player character and own game window in focus
            //Debug.Log("In Fixed Update");

            //binary conversion, layer mask should be an int(Layermask is parameter of OverlapBox), convert it to binary to see which layers are true(1) or false(0)
            //00000000000000000000000000000001(Binary) = 1(Integer)
            // summation of 2^(Layer Number) for all layers that should be used for overlap
            //aka layer 0 will trigger OverlapBox because 2^0 = 1

            //only detect objects on layer 0, "default" layer
            Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, new Vector3(1, 0.5f, 0.5f),
                                                        Quaternion.identity, 1, QueryTriggerInteraction.Ignore);
            if(hitColliders.Length == 0) isGrounded = false;
            if(hitColliders.Length > 0) isGrounded = true;
            
            Look();
            Move();
        }     

        //return false to indicate "cannot move"
        void UpdateInput(){
            float horizInput = Input.GetAxis("Horizontal");
            float vertInput = Input.GetAxis("Vertical");
            Vector3 direction = new Vector3(horizInput, 0, vertInput).normalized;
            moveDirection = transform.TransformDirection(direction);
        }
        
        public void Move(){
            UpdateInput();
            if(IsServer && IsClient){
                Debug.Log("I is the host: ");
                if(isGrounded) Position.Value += moveDirection;
            }else if(IsClient && !IsServer){
                print("Moving in client");
                //PlayerMoveServerRpc();
                PlayerInputServerRpc(moveDirection);
            }else{
                Debug.Log("Something is terribly wrong");
            }
        }

        [ServerRpc]
        void PlayerInputServerRpc(Vector3 input){
            Position.Value += input;
        }   

        void Look(){
            transform.Rotate(0,Input.GetAxis("Mouse X")*X_SENS,0);
            //transform.localEulerAngles.y += Input.GetAxis("Mouse X") * X_SENS;
        }

        private IEnumerator throwAnimationTime(){
            yield return new WaitForSeconds(1);
            anim.SetBool("throw", false);
        }
    }
}
