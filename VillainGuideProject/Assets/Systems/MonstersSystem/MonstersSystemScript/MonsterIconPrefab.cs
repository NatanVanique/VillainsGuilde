using UnityEngine;
using UnityEngine.UI;

public class MonsterIconPrefab : MonoBehaviour
{
    #region Variables
    [SerializeField] Image icon;

    public MonsterInfo Info;

    #endregion

    #region Setup

    public void Setup(MonsterInfo info)
    {
        Info = info;
        SetVisuals();
    }

    #endregion

    #region Set Visuals

    void SetVisuals()
    {
        icon.sprite = Info.SelectedData.Icon;
    }

    #endregion
}
