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
        private Camera cam;
        
        //Network Variable neccessary for syncing with server,
        //because we must update client side from the info on server side
        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
        

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
            cam = GetComponentInChildren<Camera>();

            Position.Value = new Vector3(0.0f,0.0f,0.0f);
    

            //class variable init
            isGrounded = false;

            //All players have a camera object on the prefab, disable all other cameras if its not ours
            if(!IsLocalPlayer)
            {
                cam.enabled = false;
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

            if(Input.GetKeyDown(KeyCode.Escape)){
                if(Cursor.visible) MouseLock(true);
                else MouseLock(false);
            }

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
        }        

        //return Vector3.negativeInfinity to indicate that movement should not be allowed on this call of Move()
        Vector3 TryMove(){
            if(!isGrounded) return Vector3.negativeInfinity; //we arent touching the floor, we can't move in the air

            //store Y axis component of velocity because it should remain unchanged by player input aside from jumping
            float YAXIS = rb.velocity.y;
            Vector3 yAxis = new Vector3(0, YAXIS, 0);

            float horizInput = Input.GetAxis("Horizontal");
            float vertInput = Input.GetAxis("Vertical");
            Vector3 direction = new Vector3(horizInput, 0, vertInput).normalized;
            Vector3 moveDirection = transform.TransformDirection(direction);

            if(rb.velocity.magnitude < MAX_SPEED){
                //Debug.Log("Direction: " + moveDirection);
                rb.velocity = moveDirection * MOVE_SPEED;
                return rb.velocity;
            }
            return  Vector3.negativeInfinity;
            
        }
        [ServerRpc]
        void PlayerMoveServerRpc(ServerRpcParams rpcParams = default){
            Vector3 holder = TryMove();
            if(holder != Vector3.negativeInfinity){
                Position.Value = holder;
                return;
            }
            Debug.Log("Player not grounded and cant move");
        }
        
        public void Move(){
            if(IsServer && IsClient){
                //Debug.Log("I is the host");
                Vector3 vec = TryMove();
                if(vec != Vector3.negativeInfinity) Position.Value = vec;
                return;
            }
            if(IsClient){
                PlayerMoveServerRpc();
            }else{
                Debug.Log("Something is terribly wrong");
            }
        }

        void Look(){
            transform.Rotate(0,Input.GetAxis("Mouse X")*X_SENS,0);
            //transform.localEulerAngles.y += Input.GetAxis("Mouse X") * X_SENS;
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
            if(hasFocus){
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }else{
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        //OnDrawGizmos is being usec purly for debugging  purposes to see where the hitbox is in world space
        private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
        Gizmos.DrawWireCube(transform.position-Vector3.down*0.1f, new Vector3(1, 0.5f, 0.5f));
        }

        private IEnumerator throwAnimationTime(){
            yield return new WaitForSeconds(1);
            anim.SetBool("throw", false);
        }
    }
}
