using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMove : NetworkBehaviour
{
    private Rigidbody _rigidbody;
    private Vector3 _input;
    private Camera _camera;
    private bool canJump = true;
    private bool grounded = false;
    private float SPEED = 8.0f;

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponentInChildren<Camera>();
        _rigidbody = GetComponent<Rigidbody>();
        print(_input);

        if(!IsLocalPlayer){
            _camera.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsLocalPlayer){
            return;
        }
        if(Input.GetButtonDown("Jump") && canJump && grounded){
            RequestJumpServerRpc();
        }
    }

    [ServerRpc]
    public void RequestJumpServerRpc(){
        _rigidbody.AddForce(transform.up * SPEED, ForceMode.VelocityChange);
        JumpResponseClientRpc();
    }

    [ClientRpc]
    public void JumpResponseClientRpc(){
        _rigidbody.AddForce(transform.up * SPEED, ForceMode.VelocityChange);
        canJump = false;
        StartCoroutine(ResetJump());
    }

    IEnumerator ResetJump(){
        yield return new WaitForSeconds(1);
        canJump = true;
    }

    void OnCollisionEnter(Collision collision){
        grounded = true;
        print("Breached");
    }
    void OnCollisionExit(Collision collision){
        grounded = false;
        print("pullout");

    }
}
