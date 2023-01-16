using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spartans.GameMode
{
    public class CTF : GameModeBase
    {
        public CTF(int teams, int reqCaptures, int maxTimeSeconds)
        {
            NumTeams = teams;
            _requiredScore = reqCaptures;
            MaxGameTime = maxTimeSeconds;
            Scores = new int[teams];
        }

        public override void ChangeScoreForTeam(int index, int value)
        {
            Scores[index] = value;
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
                return (Teams)(0);
            }else{
                return (Teams)IndexOfHighestScore+1; //If we have a winner return their team index
            }
        }

        public override bool EndConditionsMet()
        { 
            for(int i=0; i<NumTeams; i++)
            {
                if(Scores[i] >= _requiredScore)
                {
                    //Debug.Log("Win condition was met by team: " + i);
                    return true;
                }
            }

            return false;  
        }
    }
}