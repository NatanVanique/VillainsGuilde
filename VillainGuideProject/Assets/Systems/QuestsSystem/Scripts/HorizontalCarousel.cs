using UnityEngine;
using UnityEngine.UI;

public class HorizontalCarousel : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private int maxVisibleItems = 3;

    private int currentIndex;

    private void Awake()
    {
        leftButton.onClick.AddListener(Previous);
        rightButton.onClick.AddListener(Next);

        UpdateItems();
    }

    private void Next()
    {
        if (currentIndex + maxVisibleItems >= content.childCount)
            return;

        currentIndex++;
        UpdateItems();
    }

    private void Previous()
    {
        if (currentIndex <= 0)
            return;

        currentIndex--;
        UpdateItems();
    }

    private void UpdateItems()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            bool visible = i >= currentIndex &&
                           i < currentIndex + maxVisibleItems;

            content.GetChild(i).gameObject.SetActive(visible);
        }

        leftButton.interactable = currentIndex > 0;
        rightButton.interactable =
            currentIndex + maxVisibleItems < content.childCount;
    }
}