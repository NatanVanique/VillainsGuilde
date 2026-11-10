using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable's/New MonsterData")]
public class CreateMonster_Data : ScriptableObject
{
    [Header("Infos")]
    public LocalizedString monsterName;
    public LocalizedString monsterDescription;
    
    [Header("Images")]
    public Sprite bigImage;
    public Sprite smallImage;
    public Sprite icon;

    [Header("Stats")]
    [Range(0f, 1f)] public float strength;
    [Range(0f, 1f)] public float speed;
    [Range(0f, 1f)] public float intelligence;
    [Range(0f, 1f)] public float charisma;
    [Range(0f, 1f)] public float endurance;
    [Range(0f, 1f)] public float delicacy;
    [Range(0f, 1f)] public float luck;

    [Header("Power")] 
    public MonstersPowers mainPower; 

    public enum MonstersPowers
    {
        Fly,
        Magic,
        Metamorf,
        PhysicalProwess
    }
}
