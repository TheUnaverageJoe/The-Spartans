using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spartans.GameMode{
    public abstract class GameModeBase
    {
        protected enum States{
            Starting,
            InProgress,
            Finished
        }
        //**PROPERTIES**
        protected States currentGameState;

        protected int MaxGameTime;
        protected int CurrentGameTime;
        protected int NumTeams;
        protected int[] Scores;

        //**METHODS**
        //public abstract void Init();
        public abstract bool WinConditionsMet();
        protected abstract States GetGameState();
    }
}
