using System.Collections.Generic;
using UnityEngine;

public class QuestSelectionManager : MonoBehaviour
{
    private readonly List<QuestCardSelection> selectedCards = new();
    public List<QuestCardSelection> SelectedCards => selectedCards;

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
