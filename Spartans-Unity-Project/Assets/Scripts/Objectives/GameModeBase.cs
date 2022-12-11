using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spartans.GameMode{
    public abstract class GameModeBase
    {
        protected int MaxGameTime;
        protected int NumTeams;
        protected int _requiredScore;
        protected int[] Scores;
        //protected List<System.Func<int, bool>> predicates;

        //**METHODS**
        //public abstract void Init();
        public abstract bool EndConditionsMet();
        public abstract Teams CheckWinner();
        public abstract void ChangeScoreForTeam(int index, int value);
    }
}
