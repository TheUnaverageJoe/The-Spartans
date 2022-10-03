using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Spartans{
    public class MenuManager : MonoBehaviour
    {
        private const string SCENE_TOGO_PLAYCLICKED = "Lobby";
        public System.Action onPlayClicked;
        public System.Action onSettingsClicked;
        public System.Action onCreditsClicked;
        // Start is called before the first frame update
        void Start()
        {
            onPlayClicked += onPlayClickedCallback;
            onSettingsClicked += onSettingsClickedCallback;
            onCreditsClicked += onCreditsClickedCallback;
        }

        public void PlayButtonPressed(){
            onPlayClicked.Invoke();
        }

        private void onPlayClickedCallback(){
            SceneManager.LoadScene(SCENE_TOGO_PLAYCLICKED);
        }
        private void onSettingsClickedCallback(){
            //Yet to implement
        }
        private void onCreditsClickedCallback(){
            //yet to implement
        }

        private void OnDestroy(){
            onPlayClicked -= onPlayClickedCallback;
            onSettingsClicked -= onSettingsClickedCallback;
            onCreditsClicked -= onCreditsClickedCallback;
        }
    }
}
