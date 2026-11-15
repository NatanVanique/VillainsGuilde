using UnityEngine;

public sealed class QuestManager : MonoBehaviour
{
    public void OpenVoiceMail()
    {
        Main_Referencer.Instance.visual.voiceMailCanvas_voiceMail.SetActive(true);
    }
    
    public void CloseVoiceMail()
    {
        Main_Referencer.Instance.visual.voiceMailCanvas_voiceMail.SetActive(false);
    }
    
    public void AddQuest(QuestType type)
    {
        GameObject mailObject = Instantiate(
            Main_Referencer.Instance.visual.questMail_VoiceMail,
            Main_Referencer.Instance.visual.questMailContainer_VoiceMail.transform
        );
        
        CreateQuest_Data data;
        
        if (type == QuestType.Single)
        {
            data = Main_Referencer.Instance.quest.singleQuests[
                Random.Range(0, Main_Referencer.Instance.quest.singleQuests.Length)].quest;
            
        }else if (type == QuestType.Route)
        {
            //data = Main_Referencer.Instance.quest.routeQuests[];
        }
        
        //mailObject.GetComponent<QuestMail_Info>().setInfo(data);
    }
    
    public enum QuestType
    {
        Single,
        Route
    }
}
