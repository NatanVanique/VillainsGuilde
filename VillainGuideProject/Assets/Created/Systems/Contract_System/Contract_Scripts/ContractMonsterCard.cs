using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public sealed class ContractMonsterCard : MonoBehaviour
{
    [SerializeField] private TMP_Text monsterName;
    [SerializeField] private Image portrait;
    [SerializeField] private Button infoButton;

    private LocalizedString localizedName;

    internal void Bind(CreateMonster_Data profile, Action onSelected)
    {
        portrait.sprite = profile.icon != null ? profile.icon : profile.bigImage;
        monsterName.text = profile.name.EndsWith("Data", StringComparison.Ordinal)
            ? profile.name[..^4] : profile.name;
        localizedName = profile.monsterName;
        if (localizedName != null && !localizedName.IsEmpty)
            localizedName.StringChanged += UpdateName;

        infoButton.onClick.AddListener(() => onSelected());
    }

    private void UpdateName(string value) => monsterName.text = value;

    private void OnDestroy()
    {
        if (localizedName != null && !localizedName.IsEmpty)
            localizedName.StringChanged -= UpdateName;
    }
}
