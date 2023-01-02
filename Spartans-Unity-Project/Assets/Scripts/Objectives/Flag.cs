using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;
using System;

namespace Spartans.GameMode
{
    public class Flag : NetworkBehaviour
    {
        [SerializeField] private float MaxIslandTime;
        
        private bool held;
        private float currentIslandTime;
        private NetworkVariable<Teams> TeamAssociation = new NetworkVariable<Teams>(Teams.Neutral);
        public FlagSpawner Home{get; private set;}


        public void Init(FlagSpawner spawn)
        {
            if(Home == null)
            {
                Home = spawn;
                TeamAssociation.Value = Home.GetTeamAssociation();
            }
            else
            {
                Debug.LogWarning("Home is already set, Should only set 1 time");
            }
        }
        public override void OnNetworkSpawn()
        {
            if(IsServer)
            {
                currentIslandTime = 0;
            }
            //if(IsClient)
            {
                TeamAssociation.OnValueChanged += ChangeFlagColor;
            }
        }


        void Update()
        {
            if(!IsServer) return;
            if(held) return;

            if(currentIslandTime < MaxIslandTime)
            {
                currentIslandTime += Time.deltaTime;
            }
            else
            {
                Home.ResetFlag();
            }
        
        }
        

        public bool PickUp(Teams team)
        {
            if(held)
            {
                Debug.LogWarning($"Flag {team} already held");
                return false;
            }
            if(team == TeamAssociation.Value)
            {
                currentIslandTime = 0;
                Home.ResetFlag();
                return false;
            }
            else
            {
                currentIslandTime = 0;
                held = true;
                return true;
            }
        }
        public void DroppedFlag()
        {
            held = false;
        }

        private void ChangeFlagColor(Teams previousValue, Teams newValue)
        {
            
            switch (newValue)
            {
                case Teams.Red:
                    this.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.red;
                    this.transform.GetChild(1).GetComponent<Renderer>().material.color = Color.red;
                    break;
                case Teams.Blue:
                    this.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.blue;
                    this.transform.GetChild(1).GetComponent<Renderer>().material.color = Color.blue;
                    break;
                case Teams.Purple:
                    this.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.magenta;
                    this.transform.GetChild(1).GetComponent<Renderer>().material.color = Color.magenta;
                    break;
                case Teams.Green:
                    this.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.green;
                    this.transform.GetChild(1).GetComponent<Renderer>().material.color = Color.green;
                    break;
            }
        }

    }
}
