using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour
{
    private GameObject playerObj;
    private Vector3 offsetPos;
    // Start is called before the first frame update
    void Start()
    {
        playerObj = GameObject.Find("spartans");
        offsetPos = new Vector3(0, 0, 0);//playerObj.transform.forward*-7.7);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Cam Control script is running");
        transform.position = playerObj.transform.position;
    }
}
