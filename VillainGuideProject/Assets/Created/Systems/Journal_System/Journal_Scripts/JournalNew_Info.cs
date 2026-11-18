using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;

public class JournalNew_Info : MonoBehaviour
{
    public LocalizeStringEvent  newTitle;
    public LocalizeStringEvent  newDescription;

    public void setInfo(CreateQuest_Data data,  LocalizedString result)
    {
        newTitle.StringReference = data.questName;
        newDescription.StringReference = result;
    }
}
