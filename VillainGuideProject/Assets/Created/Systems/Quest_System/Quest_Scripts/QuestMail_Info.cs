using UnityEngine;
using UnityEngine.Localization.Components;

public class QuestMail_Info : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent mailName;

    public void setInfo(CreateQuest_Data data)
    {
        mailName.StringReference = data.questName;
        mailName.RefreshString();
    }
}
