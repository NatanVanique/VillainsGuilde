using UnityEngine;
using System;

public class MonsterReferencer : MonoBehaviour
{
    [System.Serializable]
    public class statusUpgradeValues
    {
        public CreateMonster_Data.StatsTypes upgradeStatType;
        [Range(0f, 1f)] public float upgradeValue;
        public float upgradeValueperLevel;
    }
    
    [System.Serializable]
    public class Monsters
    {
        public CreateMonster_Data monstersProfile;
        public bool contracted = false;
        public int monsterLevel = 0; // max 10
        [Range(0f, 1f)] public float happiness;
        public statusUpgradeValues[] statusUpgrade;
    }
    
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
