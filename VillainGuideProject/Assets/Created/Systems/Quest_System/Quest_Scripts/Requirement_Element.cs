using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class Requirement_Element : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text requirementText;
    [SerializeField] private Sprite timeToDoneIcon;

    public enum RequirementType
    {
        TimeToDone
    }

    public void SetRequirement(RequirementType type, string text)
    {
        switch (type)
        {
            case RequirementType.TimeToDone:
                iconImage.sprite = timeToDoneIcon;
                break;
        }

        requirementText.text = text;
    }
}
