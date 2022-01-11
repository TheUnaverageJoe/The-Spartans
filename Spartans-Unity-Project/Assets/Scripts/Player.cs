using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private Animator anim;
    private Rigidbody rb;
    //primative class variables-----
    private bool isGrounded;
    private Vector3 walk = new Vector3(0,0,1);

    //------------------------------
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        //class variable init
        isGrounded = false;
    }

    // Update is called once per frame
    void Update()
    {
        //consider changing |Input.GetAxis("Vertical")| --> |rb.velocity|
        if(Input.GetKey("w")){
            anim.SetBool("isWalking", true);//change animator variable for walking and Idle
        }else{
            anim.SetBool("isWalking", false);//change animator variable for walking and Idle
        }

        
        if(Input.GetKeyDown("f")){
            anim.SetTrigger("throw");
        }


    }

    void FixedUpdate(){
        if(Input.GetKey("w")){
            rb.velocity = walk;
        }else{
            rb.velocity = Vector3.zero;
        }
    }
}
