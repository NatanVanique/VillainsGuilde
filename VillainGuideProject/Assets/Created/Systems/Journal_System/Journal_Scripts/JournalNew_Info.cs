using UnityEngine;
using UnityEngine.Localization;

public class JournalNew_Info : MonoBehaviour
{
    public LocalizedReference newTitle;
    public LocalizedReference newDescription;

    public void setInfo(CreateQuest_Data data,  LocalizedString result)
    {
        newTitle = data.questName;
        newDescription = result;
    }
}
