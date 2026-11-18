using UnityEngine;
using System;
using TriInspector;

[DeclareBoxGroup("Monsters", Title = "Monstros")]
public class MonsterReferencer : MonoBehaviour
{
    public const int MaxMonsterLevel = 10;

    [System.Serializable]
    [DeclareBoxGroup("Upgrade", Title = "Upgrade")]
    public class statusUpgradeValues
    {
        [Group("Upgrade")]
        public CreateMonster_Data.StatsTypes upgradeStatType;

        [Group("Upgrade")]
        [Range(0f, 1f)] public float upgradeValue;

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
        public int monsterLevel = 0;

        [Group("Progression")]
        [Range(0f, 1f)] public float happiness;

        [Group("Progression")]
        public statusUpgradeValues[] statusUpgrade = Array.Empty<statusUpgradeValues>();

        public float GetStatValue(CreateMonster_Data.StatsTypes type)
        {
            statusUpgradeValues upgrade = statusUpgrade == null
                ? null
                : Array.Find(statusUpgrade, stat => stat != null && stat.upgradeStatType == type);
            return Mathf.Clamp01(GetBaseStatValue(type) + (upgrade?.upgradeValue ?? 0f));
        }

        private float GetBaseStatValue(CreateMonster_Data.StatsTypes type)
        {
            if (monstersProfile.baseStats == null)
            {
                return 0f;
            }

            var stat = Array.Find(monstersProfile.baseStats, entry => entry != null && entry.statType == type);
            return Mathf.Clamp01(stat?.statValue ?? 0f);
        }
    }

    [Group("Monsters")]
    public Monsters[] monsters = Array.Empty<Monsters>();
}
