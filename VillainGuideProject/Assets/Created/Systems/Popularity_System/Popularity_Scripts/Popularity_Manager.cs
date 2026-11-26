using UnityEngine;

public sealed class PopularityManager : MonoBehaviour
{
    public const int ExperiencePerLevel = 100;

    public void GrantExperience(long amount)
    {
        if (amount <= 0)
        {
            return;
        }

        ValuesReferencer values = Main_Referencer.Instance.value;
        int maxLevel = Mathf.Max(1, values.maxPopularityLevel_Popularity);
        int currentLevel = Mathf.Clamp(values.popularityLevel_Popularity, 1, maxLevel);
        int currentXp = Mathf.Clamp(values.popularityXp_Popularity, 0, ExperiencePerLevel);
        long capacity = (long)(maxLevel - currentLevel) * ExperiencePerLevel;
        long experience = currentXp + System.Math.Min(amount, capacity);
        int levelsGained = (int)System.Math.Min(maxLevel - currentLevel, experience / ExperiencePerLevel);
        values.popularityLevel_Popularity = currentLevel + levelsGained;
        values.popularityXp_Popularity = values.popularityLevel_Popularity == maxLevel
            ? ExperiencePerLevel
            : (int)(experience % ExperiencePerLevel);
        GameSaveSystem.RequestAutosave();
    }
}
