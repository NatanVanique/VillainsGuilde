using System.Collections.Generic;
using UnityEngine;

public class QuestSelectionManager : MonoBehaviour
{
    public static QuestSelectionManager Instance { get; private set; }

    [Header("Mission Selection")]
    [SerializeField, Min(0)] private int maxAcceptedMissions = 1;

    private readonly List<QuestCardSelection> selectedCards = new();
    public int MaxAcceptedMissions => maxAcceptedMissions;
    public IReadOnlyList<QuestCardSelection> SelectedCards => selectedCards.AsReadOnly();

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void ToggleSelection(QuestCardSelection card)
    {
        if (card == null || !card.isActiveAndEnabled || !isActiveAndEnabled)
            return;

        if (selectedCards.Contains(card))
        {
            Deselect(card);
            return;
        }

        if (selectedCards.Count >= maxAcceptedMissions)
            return;

        selectedCards.Add(card);
        card.SetSelected(true);
    }

    public void Deselect(QuestCardSelection card)
    {
        if (selectedCards.Remove(card) && card != null)
        {
            card.SetSelected(false);
        }
    }

    public void ClearSelection()
    {
        foreach (QuestCardSelection card in selectedCards)
        {
            card.SetSelected(false);
        }

        selectedCards.Clear();
    }

    private void OnDisable()
    {
        ClearSelection();
    }
}