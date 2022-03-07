using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

using Spartans.UI;
namespace Spartans.Players
{
    public class Player : NetworkBehaviour
    {
        private GameObject _mainCamera;
        private PlayerMovement _playerMovement;
        private PlayerCanvasManager _HUD;
        public string playerName{ get; private set; }
        public void Start(){
            _playerMovement = GetComponent<PlayerMovement>();
            _HUD = FindObjectOfType<PlayerCanvasManager>();
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

            _playerMovement.Init();
            _HUD.Init();
            playerName = NetworkManager.Singleton.LocalClientId.ToString();

            _mainCamera.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
