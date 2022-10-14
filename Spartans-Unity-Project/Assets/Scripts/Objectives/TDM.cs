using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Spartans.GameMode{
    public class TDM : GameModeBase
    {
        private int _requiredEliminations = 10;

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
                    System.Console.WriteLine("Win condition was met by team: " + i);
                    return true;
                }
            }
            if(CurrentGameTime >= MaxGameTime)
            {
                System.Console.WriteLine("Time limit reached");
                return true;
            }

            return false;
        }

        public override int CheckWinner()
        {
            int IndexOfHighestScore = 0;
            bool isTie = false;

            for(int i=0; i<NumTeams; i++)
            {
                if(Scores[i] == Scores[IndexOfHighestScore])
                {
                    isTie = true;
                }
                else if(Scores[i] > Scores[IndexOfHighestScore])
                {
                    if(isTie)
                    {
                        isTie = false;
                    }
                    IndexOfHighestScore = i;
                    
                }
            }
            
            if(isTie){
                return -1;
            }else{
                return IndexOfHighestScore; //If we have a winner return their team index
            }
        }
    }
}
