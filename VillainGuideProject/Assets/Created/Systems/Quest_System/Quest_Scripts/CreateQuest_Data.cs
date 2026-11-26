    using UnityEngine;
    using UnityEngine.Localization;


    [CreateAssetMenu(fileName = "QuestData", menuName = "Scriptable's/New QuestData")]
    public class CreateQuest_Data : SavedGameDefinition
    {
        
        [Header("Infos")]
        public LocalizedString questName;
        public LocalizedString questDescription;
        public QuestType type;

        [Header("Results")] 
        public LocalizedString goodResult;
        public LocalizedString badResult;

        
        [Header("Requirements")] 
        public int minPopularity;
        [Range(0f, 1f)] public float winMargin;
        public float timeToDoneSeconds;
        [Min(0f)] public float timeToAcceptSeconds;
        public CreateMonster_Data.MonstersPowers[] powersToWin;
        public int statsToWin;
        public statusRequirementsValues[]  statusRequirements;

        [Header("Rewards")] 
        [UnityEngine.Serialization.FormerlySerializedAs("hasReputationXpReward")]
        public bool haspopularityXpReward;
        [UnityEngine.Serialization.FormerlySerializedAs("reputationXpEarned")]
        public int popularityXpEarned;
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
