using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Spartans.GameMode{
    public class TDM : GameModeBase
    {
        private int _requiredEliminations;

        public TDM(int teams, int reqKills, int maxTimeSeconds)
        {
            NumTeams = teams;
            _requiredEliminations = reqKills;
            MaxGameTime = maxTimeSeconds;
            Scores = new int[teams];
        }
        
        public override bool EndConditionsMet()
        { 
            for(int i=0; i<NumTeams; i++)
            {
                if(Scores[i] >= _requiredEliminations)
                {
                    //Debug.Log("Win condition was met by team: " + i);
                    return true;
                }
            }
            if(CurrentGameTime >= MaxGameTime)
            {
                Debug.Log("Time limit reached");
                return true;
            }

            return false;
            
        }

        public override Teams CheckWinner()
        {
            int IndexOfHighestScore = 0;
            bool isTie = false;

            for(int i=0; i<NumTeams; i++)
            {
                //Debug.Log("Checking team " + i + $"({(Teams)i})");
                //Debug.Log("Highest score so far: " + Scores[IndexOfHighestScore] + " at index " + IndexOfHighestScore);
                if(Scores[i] == Scores[IndexOfHighestScore] && i != 0)
                {
                    //Debug.Log($"team {i} score {Scores[i]} \n High score {Scores[IndexOfHighestScore]}");
                    isTie = true;
                }
                else if(Scores[i] > Scores[IndexOfHighestScore])
                {
                    isTie = false;
                    IndexOfHighestScore = i;
                }
                //Debug.Log("bool state " + isTie);
            }
            
            if(isTie){
                return (Teams)(-1);
            }else{
                return (Teams)IndexOfHighestScore; //If we have a winner return their team index
            }
        }

        public override void ChangeScoreForTeam(int index, int value)
        {
            Scores[index] = value;
        }
    }
}
