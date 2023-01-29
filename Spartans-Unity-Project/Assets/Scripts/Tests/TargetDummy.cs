using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Spartans.UI;

namespace  Spartans.Players
{
    public class TargetDummy : NetworkBehaviour
    {
        [SerializeField] public GameObject worldSpaceCanvas;
            [SerializeField] public Transform lookAtPoint;
            [SerializeField] public AnimationManager _animationManager;
            [SerializeField] public string playerName{ get; private set; }
            [SerializeField] private bool _canJump = true;
            [SerializeField] private bool _grounded = false;
            [SerializeField] private float _mouseSens = 1.0f;
            [SerializeField]private bool immobilized = false;
            //private Vector3 input;
            //[SerializeField] private Vector2 ClientInput;
            //private bool _sprinting;
            
            private NetworkVariable<Teams> myTeam = new NetworkVariable<Teams>();
            private GameManager _gameManager;
            private ClassController _classController;
            private Health _myHealth;
            //private FlagCarrier _flagCarrier;
            private LeapTarget _leapTarget;

            private void Init()
            {
                _myHealth = GetComponent<Health>();
                _animationManager = GetComponent<AnimationManager>();
                _gameManager = FindObjectOfType<GameManager>();
                _leapTarget = GetComponent<LeapTarget>();
                if(_leapTarget) _leapTarget.Init(_animationManager);
                if(_leapTarget) _leapTarget.OnPinned += Immobilized;
            }

            public void Start()
            {
                if(NetworkManager.Singleton == null)
                {
                    //Debug.LogWarning("Player Controller started as offline or NPC/Target DUmmy");
                    //Debug.Log("myTeam is: " + myTeam.Value);
                    Init();
                    //_myHealth.Init(this);
                }
                _myHealth.OnKilledBy += OnDieCallback;
                _myHealth.OnRespawn += OnRespawnCallback;
            }

            //server needs to update grounded state for all players on server side
            //if not the server or a local player object, aka the player spawned when join game, dont update
            void FixedUpdate(){
                if(IsServer && !IsLocalPlayer){
                    CheckGrounded();
                }
            }

            public override void OnNetworkSpawn()
            {
                Init();
                _myHealth.Init(this);
                playerName = "Player " + OwnerClientId.ToString();
                transform.GetComponentInChildren<FloatingHealth>().ChangeName(playerName);
                myTeam.OnValueChanged += SetTeamColor;
                //myTeam.OnValueChanged += (prev, newVal)=>{print("Team value changed:" + newVal);};
                //print("States SCOL: " + IsServer + IsClient + IsOwner + IsLocalPlayer);
            }
            
            public void Immobilize(bool immobile)
            {
                immobilized = immobile;
            }
            private void Immobilized(bool pinned)
            {
                immobilized = pinned;
                GetComponent<Collider>().enabled = !pinned;
            }

            void CheckGrounded(){
                if(!_canJump) return;
                RaycastHit hit;
                bool hitOccured = Physics.Raycast(transform.position-(Vector3.down*0.5f), Vector3.down, out hit, 0.6f, 1);
                Debug.DrawRay(transform.position-(Vector3.down*0.5f), Vector3.down * 0.6f, Color.blue);

                if(hitOccured && !_grounded){
                    //check if we are the server because This func gets run on server and client
                    //thus it would produce a duplicate instruction to change the Animator parameter
                    if(IsServer){
                    _animationManager.SetParameter("grounded", true);
                    }
                    _grounded = true;
                }else if(!hitOccured){
                    _grounded = false;
                }else{
                    //ALREADY ON GROUND IN THIS CASE
                    //print("Unanticipated condition occured");
                }
            }

            private void OnDieCallback(Teams team){
                this.GetComponent<Rigidbody>().useGravity = false;
                this.GetComponent<BoxCollider>().enabled = false;
                this.enabled = false;
            }

            private void OnRespawnCallback()
            {
            this.GetComponent<Rigidbody>().useGravity = true;
                this.GetComponent<BoxCollider>().enabled = true;
                this.enabled = true;
            }

            public override void OnNetworkDespawn()
            {
                Debug.Log("Despawned player " + playerName);
                base.OnNetworkDespawn();
                if(IsClient && IsOwner)
                {
                    if(_leapTarget) _leapTarget.OnPinned -= Immobilized;
                }
            }

            public bool IsAirborn(){
                return !_grounded;
            }

            public void ChangeTeam(Teams team)
            {
                SetTeamColor(Teams.Neutral, team);
                if(myTeam.Value == team) return;

                myTeam.Value = team;
            }

            public System.Nullable<Teams> GetTeamAssociation(){
                if(!IsServer){
                    Debug.LogError("GetTeamAssociation can only be called from server");
                    return null;
                }

                //print("myTeam var is: " + myTeam.Value);
                return myTeam.Value;
            }
            private void SetTeamColor(Teams prevTeam, Teams currentTeam)
            {
                Transform colorRegion;

                if(_classController.GetType() == typeof(Spartans.Players.ShieldBarerController))
                {
                    colorRegion = transform.GetChild(2);
                }else if(_classController.GetType() == typeof(Spartans.Players.ArcherController)){
                    colorRegion = transform.GetChild(1).GetChild(0);
                }else
                {
                    colorRegion = transform.GetChild(1).GetChild(4);
                }
                switch (currentTeam)
                {
                    case Teams.Neutral:
                        colorRegion.GetComponent<Renderer>().material.color = new Color(127, 0, 204); // Color = Purple
                        break;
                    case Teams.Red:
                        colorRegion.GetComponent<Renderer>().material.color = Color.red;
                        break;
                    case Teams.Blue:
                        colorRegion.GetComponent<Renderer>().material.color = Color.blue;
                        break;
                    case Teams.Purple:
                        colorRegion.GetComponent<Renderer>().material.color = Color.magenta;
                        break;
                    case Teams.Green:
                        colorRegion.GetComponent<Renderer>().material.color = Color.green;
                        break;
                }
            }
    }
}