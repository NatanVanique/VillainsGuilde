using UnityEngine;

public sealed class MonsterManager : MonoBehaviour
{
    [System.Serializable]
    public class Monsters
    {
        public CreateMonster_Data monstersProfile;
        public bool contracted = false;
        [Range(0f, 1f)] public float happiness;
    }
    
    public Monsters[] monsters;
}
