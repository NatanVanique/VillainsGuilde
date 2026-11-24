using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;

public class JournalNew_Info : MonoBehaviour
{
    public LocalizeStringEvent  newTitle;
    public LocalizeStringEvent  newDescription;
    [SerializeField] private UnityEngine.UI.Image monsterImage;

    public void setInfo(CreateQuest_Data data,  LocalizedString result)
    {
        newTitle.StringReference = data.questName;
        newDescription.StringReference = result;
    }

    public void SetMonsterImage(CreateMonster_Data monster)
    {
        if (monsterImage == null)
        {
            return;
        }

        Sprite artwork = monster == null ? null : monster.journalImage;
        if (monster != null && artwork == null)
        {
            artwork = monster.bigImage != null ? monster.bigImage : monster.icon;
        }

        monsterImage.sprite = artwork;
        monsterImage.gameObject.SetActive(artwork != null);
        if (artwork == null)
        {
            ExpandText(newTitle);
            ExpandText(newDescription);
        }
    }

    private static void ExpandText(LocalizeStringEvent label)
    {
        var rect = (RectTransform)label.transform;
        Vector2 left = rect.offsetMin;
        left.x = -rect.offsetMax.x;
        rect.offsetMin = left;
    }
}
