using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Spartans.GameMode{
    public class TDM : GameModeBase
    {
        [SerializeField] private GameObject _gamemodeUI;
        //[SerializeField] private int _numberOfTeams;

        // Start is called before the first frame update
        void Awake()
        {
            for(int i=0; i<base.numberOfTeams; i++)
            {
                NetworkVariable<int> newVar = new NetworkVariable<int>();
                base.teamScoresDict.Add(i, newVar);
            }
            
        }

        public override void OnNetworkSpawn(){
            base.OnNetworkSpawn();

            base.gamemodeUI = _gamemodeUI;
            //base.numberOfTeams = _numberOfTeams;
            base.winConditions = new List<System.Action>();
            base.currentGameState = States.Starting;

        }

        protected override States GetGameState()
        {
            return base.currentGameState;
            //throw new System.NotImplementedException();
        }

        public override bool WinConditionsMet()
        {
            
            //throw new System.NotImplementedException();
            foreach(System.Action pred in base.winConditions){
                print("found pred " + pred.ToString());
            }
            return false;
        }
    }
}
