using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Spartans.GameMode{
    public class TDM : GameModeBase
    {
        private int _requiredEliminations = 0;

        public TDM(int teams, int reqKills, int maxTimeSeconds)
        {
            NumTeams = teams;
            _requiredEliminations = reqKills;
            MaxGameTime = maxTimeSeconds;
            Scores = new int[teams];
        }
        
        protected override States GetGameState()
        {
            return base.currentGameState;
        }

        public override bool WinConditionsMet()
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
    }
}
