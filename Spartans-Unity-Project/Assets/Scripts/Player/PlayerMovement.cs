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
        public NetworkVariable<Vector3> networkVelocity = new NetworkVariable<Vector3>();
        public NetworkVariable<float> networkRotationAroundY = new NetworkVariable<float>();
        private Vector3 _inputDirection = new Vector3(0, 0, 0);
        

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

            //class variable init
            isGrounded = false;

            //EXAMPLE LINE
            networkVelocity.OnValueChanged += (Vector3 prev, Vector3 updated) => {
                //updated.y = rb.velocity.y;
                rb.velocity = updated;
                //rb.AddForce(networkVelocity.Value, ForceMode.VelocityChange);
            };
            networkRotationAroundY.OnValueChanged += (float prev, float updated) => {
                transform.rotation = Quaternion.Euler(0, updated, 0);
            };
        }
        
        void FixedUpdate(){
            if(IsClient && IsOwner){
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
            
            //EDIT DATE 3/8/2022: This is because every running .exe file has all exact same set of objects in it.
            //Thus each client has multiple player objects in the hierarchy

            //binary conversion, layer mask should be an int(Layermask is parameter of OverlapBox), convert it to binary to see which layers are true(1) or false(0)
            //00000000000000000000000000000001(Binary) = 1(Integer)
            // summation of 2^(Layer Number) for all layers that should be used for overlap
            //aka layer 0 will trigger OverlapBox because 2^0 = 1

            //only detect objects on layer 0, "default" layer
        }

        //UpdateServer is only called from Server in fixedUpdate, every fixedUpdate tick on server
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
        }

        //Take in local user input and send it to the server version of that player obj
        private void UpdateClientInput(){
            //Get Input manager inputs for Up down arrow keys and left right arrow keys
            float inputVert = Input.GetAxisRaw("Vertical");
            float inputHoriz = Input.GetAxisRaw("Horizontal");
            float mouseX = Input.GetAxis("Mouse X")*X_SENS;

            //create a object space vector3 with inputs and normalize
            Vector2 input = new Vector2(inputHoriz, inputVert).normalized;
            //Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

            UpdateMoveServerRpc(input);
            if(mouseX!=0) UpdateRotationServerRpc(mouseX);
        }

        [ServerRpc]
        private void UpdateMoveServerRpc(Vector2 input){
            //need to update _inputDirection so that local version of obj on server side knows what the inputs were to work from
            Vector3 worldSpaceDir = new Vector3(input.x, 0, input.y);
            //store player input converted to object space coords
            _inputDirection = transform.TransformDirection(worldSpaceDir);
            //_inputDirection = worldSpaceDir;

            if(!isGrounded){
                return;
            }

            if(input == Vector2.zero){
                networkVelocity.Value = Vector3.zero;
            }
            else if((networkVelocity.Value + _inputDirection * MOVE_SPEED).magnitude < MAX_SPEED)
            {
                networkVelocity.Value += _inputDirection * MOVE_SPEED;
            }
            else if((networkVelocity.Value + _inputDirection * MOVE_SPEED).magnitude >= MAX_SPEED)
            {
                //Dot product to determine if direction of motion is same/similar to inputDirection
                if(Vector3.Dot(networkVelocity.Value.normalized, _inputDirection.normalized) < 0){
                    networkVelocity.Value = Vector3.zero;
                    networkVelocity.Value += _inputDirection*MOVE_SPEED;
                }else{
                    networkVelocity.Value = _inputDirection*MAX_SPEED;
                }
            }
        }

        [ServerRpc]
        void UpdateRotationServerRpc(float xRot){
            networkRotationAroundY.Value += xRot;
        }

        public override void OnNetworkSpawn(){
            Vector3 temp = transform.position;
            temp.y += 3;
            transform.position = temp;
        }

        private IEnumerator throwAnimationTime(){
            yield return new WaitForSeconds(1);
            anim.SetBool("throw", false);
        }
    }
}
