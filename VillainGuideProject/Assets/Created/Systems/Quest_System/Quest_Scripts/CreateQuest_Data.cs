    using UnityEngine;
    using UnityEngine.Localization;


    [CreateAssetMenu(fileName = "QuestData", menuName = "Scriptable's/New QuestData")]
    public class CreateQuest_Data : ScriptableObject
    {
        
        [Header("Infos")]
        public LocalizedString questName;
        public LocalizedString questDescription;

        [Header("Requirements")] 
        public int minPopularity;
        [Range(0f, 1f)] public float winMargin;
        public float timeToDoneSeconds;
        public CreateMonster_Data.MonstersPowers[] powersToWin;
        public int statsToWin;
        public statusRequirementsValues[]  statusRequirements;

        [Header("Rewards")] 
        public int reputationXpEarned;
        public int characterXpEarned;
        public int coinsEarned;
        public string[] characterEarned;
        
        [System.Serializable]
        public class statusRequirementsValues
        {
            public CreateMonster_Data.StatsTypes requerimenttype;
            [Range(0f, 1f)] public float requerimentValue;
        }
    }
