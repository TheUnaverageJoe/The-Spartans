using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Spartans{
    [CreateAssetMenu(fileName = "GameModes", menuName = "ScriptableObjects/GameMode")]
    public class GameModeData : ScriptableObject
    {
        public int numberOfTeams;
        //public int maxTimeSeconds;
        public int targetValue;
        public string modeName;
        public Sprite verticalBanner;

    }
}
