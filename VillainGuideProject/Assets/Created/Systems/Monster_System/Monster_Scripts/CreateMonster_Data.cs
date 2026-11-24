using System;
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
    public Sprite journalImage;
    
    [System.Serializable]
    public class statsInfos
    {
        public StatsTypes statType;
        [Range(0f, 1f)] public float statValue;
    }
    
    [Header("Stats")]
    public statsInfos[] baseStats = Array.Empty<statsInfos>();

    [Header("Power")] 
    public MonstersPowers mainPower; 

    public enum MonstersPowers
    {
        None,
        Fly,
        Magic,
        Metamorf,
        PhysicalProwess
    }
    public enum StatsTypes
    {
        strength,
        speed,
        intelligence,
        charisma,
        endurance,
        delicacy,
        luck,
        enthusiasm
    }
}
