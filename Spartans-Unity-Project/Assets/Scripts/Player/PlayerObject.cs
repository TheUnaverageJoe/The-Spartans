using UnityEngine;

[CreateAssetMenu(fileName = "PlayerObject", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class SpawnManagerScriptableObject : ScriptableObject
{
    public string prefabName;
    public GameObject prefab;

    public int health;
    public int damage;
}
