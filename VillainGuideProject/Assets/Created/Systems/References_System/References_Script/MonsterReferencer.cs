using UnityEngine;
using System;
using TriInspector;

[DeclareBoxGroup("Monsters", Title = "Monstros")]
public class MonsterReferencer : MonoBehaviour
{
    [System.Serializable]
    [DeclareBoxGroup("Upgrade", Title = "Upgrade")]
    public class statusUpgradeValues
    {
        [Group("Upgrade")]
        public CreateMonster_Data.StatsTypes upgradeStatType;

        [Group("Upgrade")]
        [Range(0f, 1f)] public float upgradeValue = 1;

        [Group("Upgrade")]
        public float upgradeValueperLevel;
    }
    
    [System.Serializable]
    [DeclareBoxGroup("Profile", Title = "Perfil")]
    [DeclareBoxGroup("Progression", Title = "Progressão")]
    public class Monsters
    {
        [Group("Profile")]
        public CreateMonster_Data monstersProfile;

        [Group("Progression")]
        public bool contracted = false;

        [Group("Progression")]
        public int monsterLevel = 0; // max 10

        [Group("Progression")]
        [Range(0f, 1f)] public float happiness;

        [Group("Progression")]
        public statusUpgradeValues[] statusUpgrade;
    }
    
    [Group("Monsters")]
    public Monsters[] monsters;
    
    private void OnValidate()
    {
        var types = (CreateMonster_Data.StatsTypes[])
            Enum.GetValues(typeof(CreateMonster_Data.StatsTypes));

        if (monsters == null) return;

        foreach (var monster in monsters)
        {
            if (monster == null) continue;

            if (monster.statusUpgrade == null ||
                monster.statusUpgrade.Length != types.Length)
                Array.Resize(ref monster.statusUpgrade, types.Length);

            for (int i = 0; i < types.Length; i++)
            {
                monster.statusUpgrade[i] ??= new statusUpgradeValues();
                monster.statusUpgrade[i].upgradeStatType = types[i];

                if (monster.monstersProfile == null ||
                    monster.monstersProfile.baseStats == null ||
                    i >= monster.monstersProfile.baseStats.Length ||
                    monster.monstersProfile.baseStats[i] == null)
                    continue;

                float baseValue = monster.monstersProfile.baseStats[i].statValue;
                float maxUpgrade = 1f - baseValue;

                monster.statusUpgrade[i].upgradeValueperLevel = maxUpgrade / 10f;
            }
        }
    }
}
