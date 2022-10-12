using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spartans.GameMode{
    public abstract class GameModeBase
    {
        protected int MaxGameTime;
        protected int CurrentGameTime;
        protected int NumTeams;
        protected int[] Scores;

        //**METHODS**
        //public abstract void Init();
        public abstract bool EndConditionsMet();
        public abstract int CheckWinner();
    }
}
