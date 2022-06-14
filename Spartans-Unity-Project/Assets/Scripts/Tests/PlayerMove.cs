using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMove : NetworkBehaviour
{
    private Rigidbody _rigidbody;
    //private Vector3 _input;
    private Camera _camera;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool grounded = false;
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float moveSpeed = 0.05f;
    [SerializeField] private float MAX_SPEED = 3.0f;
    [SerializeField] private float mouseSens = 1.0f;
    [SerializeField] private bool timed = false;
    [SerializeField] private Coroutine groundingTimer;

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponentInChildren<Camera>();
        _rigidbody = GetComponent<Rigidbody>();
        //print(_input);

        if(!IsLocalPlayer){
            _camera.gameObject.SetActive(false);
        }
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
            requestMoveServerRpc(input);
        }
    }
    void FixedUpdate(){
        CheckGrounded();
    }

    [ServerRpc]
    public void RequestJumpServerRpc(){
        if(!grounded) return;
        _rigidbody.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
        JumpResponseClientRpc();
    }

    [ClientRpc]
    public void JumpResponseClientRpc(){
        _rigidbody.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
        canJump = false;
        grounded = false;
        StartCoroutine(ResetJump());
    }

    IEnumerator ResetJump(){
        yield return new WaitForSeconds(1);
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
        if(_rigidbody.velocity.magnitude > MAX_SPEED){
            print("TOO FAST");
            _rigidbody.velocity = _rigidbody.velocity.normalized*MAX_SPEED;
            return;
        }

        if(_rigidbody.velocity.magnitude <= MAX_SPEED){
            moveDir = transform.TransformDirection(moveDir);
            _rigidbody.AddForce(moveDir*moveSpeed, ForceMode.VelocityChange);
        }

    }

    void OnCollisionEnter(Collision collision){
        grounded = true;
        canJump = true;
        print("Breached");
    }
    void OnCollisionExit(Collision collision){
        grounded = false;
        canJump = false;
        print("pullout");
    }
    void CheckGrounded(){
        Debug.DrawRay(transform.position, Vector3.down * 1.25f, Color.blue);
        if(grounded) return;
        RaycastHit hit;
        bool hitOccured = Physics.Raycast(transform.position, Vector3.down, out hit, 1.25f, 3);
        print("Debug phyics line stuff");
        if(timed == true){
            if(!hitOccured){
                timed = false;
                StopCoroutine(groundingTimer);
            }else{
                if(groundingTimer==null){
                    timed = false;
                }
            }
        }else{
            if(hitOccured){
                timed = true;
                groundingTimer = StartCoroutine(checkGrounded());
            }
        }
        //print(hit);
    }
    private IEnumerator checkGrounded(){
        yield return new WaitForSeconds(0.25f);
        grounded = true;
        canJump = true;
        timed = false;
    }
}
