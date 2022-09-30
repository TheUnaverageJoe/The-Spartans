using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spartans.Players{
    public class PlayerInput : MonoBehaviour
    {
        public static PlayerInput Instance{get; private set;} = null;
        public Vector3 movement{get; private set;}
        public float mouseX{get; private set;}
        public float mouseY{get; private set;}

        public bool jump{get; private set;}
        public bool escape{get; private set;}
        public bool tab{get; private set;}
        public bool primary{get; private set;}
        public bool secondary{get; private set;}
        public bool special{get; private set;}

        
        void Awake(){
            if(Instance == null){
                //print("assigned instance");
                Instance = this;
            }else{
                Destroy(this.gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            movement = Vector3.zero;
            jump = false;
            tab = false;
            escape = false;
            mouseX = 0;
            mouseY = 0;

            primary = false;
            secondary = false;
            special = false;
            
        }
        //*/

        // Update is called once per frame
        void Update()
        {
            movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0 , Input.GetAxisRaw("Vertical"));
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");

            jump = Input.GetButtonDown("Jump");
            escape = Input.GetKeyDown(KeyCode.Escape);
            tab = Input.GetKeyDown(KeyCode.Tab);

            primary = Input.GetButtonDown("Fire1");
            secondary = Input.GetButtonDown("Fire2");
            //special = Input.GetButtonDown("Fire3");
            special = Input.GetKeyDown(KeyCode.Q);
            

        }
        private void OnDisable(){
            Instance = null;
        }
    }
}
