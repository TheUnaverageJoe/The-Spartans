using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Spartans.UI{
    public class PopUpText : MonoBehaviour
    {
        private Text text;
        private float timeToDisplay;
        public static UnityEvent<string> queueDisplayText;
        private Queue<string> displayQueue;
        // Start is called before the first frame update
        void Start()
        {
            text = gameObject.GetComponent<Text>();
            timeToDisplay = 0;
            displayQueue = new Queue<string>();
            queueDisplayText = new UnityEvent<string>();
            queueDisplayText.AddListener(DisplayText);
            
        }
        private void DisplayText(string text){
            displayQueue.Enqueue(text);
        }
        public void Update(){
            if(timeToDisplay>0){
                timeToDisplay -= Time.deltaTime;
            }else{
                timeToDisplay = 0;
                text.text = " ";
            }
            if(displayQueue.Count == 0) return;
            if(timeToDisplay==0){
                timeToDisplay = 3;
                text.text = displayQueue.Dequeue();
            }
        }
    }
}
