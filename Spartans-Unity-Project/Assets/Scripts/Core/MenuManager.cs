using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Spartans.UI;

namespace Spartans{
    public class MenuManager : MonoBehaviour
    {
        private const string SCENE_TOGO_PLAYCLICKED = "Lobby";

        private CanvasManager _canvasManager;

        // Start is called before the first frame update
        void Start()
        {
            _canvasManager = FindObjectOfType<CanvasManager>();
            _canvasManager.Init();
        }

        public void PlayButtonPressed(){
            SceneManager.LoadScene(SCENE_TOGO_PLAYCLICKED);
        }
    }
}
