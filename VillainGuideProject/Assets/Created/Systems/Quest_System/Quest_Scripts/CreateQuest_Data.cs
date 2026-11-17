    using UnityEngine;
    using UnityEngine.Localization;


    [CreateAssetMenu(fileName = "QuestData", menuName = "Scriptable's/New QuestData")]
    public class CreateQuest_Data : ScriptableObject
    {
        
        [Header("Infos")]
        public LocalizedString questName;
        public LocalizedString questDescription;
        public QuestType type;

        [Header("Requirements")] 
        public int minPopularity;
        [Range(0f, 1f)] public float winMargin;
        public float timeToDoneSeconds;
        public CreateMonster_Data.MonstersPowers[] powersToWin;
        public int statsToWin;
        public statusRequirementsValues[]  statusRequirements;

        [Header("Rewards")] 
        public bool hasReputationXpReward;
        public int reputationXpEarned;
        public bool hasCharacterXpReward;
        public int characterXpEarned;
        public bool hasCoinsReward;
        public int coinsEarned;
        public bool hasCharacterReward;
        public string[] characterEarned = System.Array.Empty<string>();
        
        public enum QuestType
        {
            Single,
            Route
        }
        
        [System.Serializable]
        public class statusRequirementsValues
        {
            public CreateMonster_Data.StatsTypes requerimenttype;
            [Range(0f, 1f)] public float requerimentValue;
        }
    }
