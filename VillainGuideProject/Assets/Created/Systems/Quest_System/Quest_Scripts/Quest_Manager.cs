using UnityEngine;

public sealed class QuestManager : MonoBehaviour
{
    public void OpenVoiceMail()
    {
        Main_Referencer.Instance.visual.voiceMailCanvas_voiceMail.enabled = true;
    }
    
    public void CloseVoiceMail()
    {
        Main_Referencer.Instance.visual.voiceMailCanvas_voiceMail.enabled = false;
    }
}
