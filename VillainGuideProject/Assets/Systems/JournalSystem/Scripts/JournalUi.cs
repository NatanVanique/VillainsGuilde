using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalUi : MonoBehaviour
{
    private enum Pages
    {
        Abstract,
        Results
    }

    private Pages pageType;
    private JournalRegister rewardsRegister;

    [SerializeField] private GridLayoutGroup newsGrid;
    [SerializeField] private RectTransform bigNewObject;
    [SerializeField] private float bigNewSpacing = 16f;

    public void Setup()
    {
        rewardsRegister = gameObject.GetComponent<JournalRegister>();
        RefreshLayout();
    }

    private void Awake()
    {
        if (rewardsRegister == null)
            rewardsRegister = gameObject.GetComponent<JournalRegister>();

        RefreshLayout();
    }

    public void RefreshLayout()
    {
        if (newsGrid == null)
            return;

        bool hasBigNews = rewardsRegister != null && rewardsRegister.JournalPage.BigNew != null;
        if (bigNewObject != null)
            bigNewObject.gameObject.SetActive(hasBigNews);

        RectOffset padding = newsGrid.padding;
        padding.top = 0;

        if (hasBigNews && bigNewObject != null)
        {
            padding.top = Mathf.CeilToInt(bigNewObject.rect.height + bigNewSpacing);
        }

        newsGrid.padding = padding;
        newsGrid.SetLayoutVertical();
    }

    public List<QuestData> GetSmallNewsToShow(int amount)
    {
        if (rewardsRegister == null || amount <= 0)
            return new List<QuestData>();

        int count = Mathf.Min(amount, rewardsRegister.SmallNews.Count);
        return rewardsRegister.SmallNews.GetRange(0, count);
    }

    private void ResultsVisual()
    {
        //Caso trocar sprite
    }
}
