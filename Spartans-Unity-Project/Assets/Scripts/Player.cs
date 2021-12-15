using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator anim;
    private Rigidbody rb;
    //primative class variables-----
    private bool isGrounded;

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
        anim.SetFloat("Moving", Mathf.Abs(Input.GetAxis("Vertical")));//change animator variable for walking and Idle

    }

    void FixedUpdate(){
        if(Input.GetKey(KeyCode.W)){
            rb.velocity = new Vector3(0,0,1);
        }
    }
}
