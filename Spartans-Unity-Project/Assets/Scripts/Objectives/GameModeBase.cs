using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


namespace Spartans.GameMode{
    public abstract class GameModeBase : NetworkBehaviour
    {
        protected enum States{
            Starting,
            InProgress,
            WinConditionsMet
        }
        //**PROPERTIES**
        [SerializeField] protected GameObject gamemodeUI;
        [SerializeField] protected int numberOfTeams;
        //protected NetworkDictionary
        protected Dictionary<int, NetworkVariable<int>> teamScoresDict;
        //private NetworkVariable<int>[] TeamScores;
        [SerializeField] protected List<System.Action> winConditions;
        [SerializeField] protected States currentGameState;
        protected System.Action gameStateChanged;

        //**METHODS**
        //public abstract void Init();
        public abstract bool WinConditionsMet();
        protected abstract States GetGameState();
    }
}
