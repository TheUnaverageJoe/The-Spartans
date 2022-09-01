using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerControl : NetworkBehaviour
{
    private float walkSpeed = 0.01f;
    private Vector2 defaultPostionRange = new Vector2(-4, 4);
    private NetworkVariable<float> forwardBackPosition = new NetworkVariable<float>();
    private NetworkVariable<float> leftRightPosition = new NetworkVariable<float>();

    private float oldForwardBackPosition;
    private float oldLeftRightPosition;

    private void Start(){
        if(IsClient && IsOwner){
            transform.position = new Vector3(Random.Range(defaultPostionRange.x, defaultPostionRange.y), 0,
                Random.Range(defaultPostionRange.x, defaultPostionRange.y));

        }
        
        
    }

    private void Update(){
        if(IsServer){
            UpdateServer();
        }
        if(IsClient && IsOwner){
            UpdateClient();
        }
    }

    private void UpdateServer(){
        transform.position = new Vector3(transform.position.x + leftRightPosition.Value, transform.position.y,
            transform.position.z + forwardBackPosition.Value);
    }

    private void UpdateClient(){
        float forwardBackward = 0;
        float leftRight = 0;

        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){
            forwardBackward += walkSpeed;
        }
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
            leftRight -= walkSpeed;
            
        }
        if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){
            forwardBackward -= walkSpeed;
        }
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
            leftRight += walkSpeed;
        }

        if(oldForwardBackPosition != forwardBackward || oldLeftRightPosition != leftRight){
            oldForwardBackPosition = forwardBackward;
            oldLeftRightPosition = leftRight;

            UpdateClientPostionServerRpc(forwardBackward, leftRight);
        }
    }

    [ServerRpc]
    private void UpdateClientPostionServerRpc(float fb, float lr){
        forwardBackPosition.Value = fb;
        leftRightPosition.Value = lr;
    }
}
