using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Spartans{
    [CreateAssetMenu(fileName = "GameModes", menuName = "ScriptableObjects/GameMode")]
    public class GameModeData : ScriptableObject
    {
        public int numberOfTeams;
        public string modeName;
        public Sprite verticalBanner;

    }
}
