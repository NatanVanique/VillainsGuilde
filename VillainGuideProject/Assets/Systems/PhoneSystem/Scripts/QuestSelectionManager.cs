using System.Collections.Generic;
using UnityEngine;

public class QuestSelectionManager : MonoBehaviour
{

    [Header("Mission Selection")]
    [SerializeField, Min(0)] private int maxAcceptedMissions = 1;

    private readonly List<QuestCardSelection> selectedCards = new();
    public int MaxAcceptedMissions => maxAcceptedMissions;
    public IReadOnlyList<QuestCardSelection> SelectedCards => selectedCards.AsReadOnly();

    public void Setup()
    {

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

        TrySelect(card);
    }

    public bool TrySelect(QuestCardSelection card)
    {
        if (card == null || !card.isActiveAndEnabled || !isActiveAndEnabled)
            return false;

        if (selectedCards.Contains(card))
            return true;

        if (selectedCards.Count >= maxAcceptedMissions)
            return false;

        selectedCards.Add(card);
        card.SetSelected(true);
        return true;
    }

    public void Deselect(QuestCardSelection card)
    {
        if (selectedCards.Remove(card))
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
