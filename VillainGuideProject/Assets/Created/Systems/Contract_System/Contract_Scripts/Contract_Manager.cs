using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public sealed class ContractManager : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private MonsterManager _monsterManager;
    
    [System.Serializable]
    public class StatSlider
    {
        public CreateMonster_Data.StatsTypes statType;
        public Slider slider;
    }

    [Header("Stats")]
    [SerializeField] private StatSlider[] stats;
    
    [Header("ContractTolltip")]
    [SerializeField] private Image bigImage_ContractTolltip;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText_ContractTolltip;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private void GenerateStats(LocalizedString nameSearch)
    {
        foreach (var monster in _monsterManager.monsters)
        {
            if (monster.monstersProfile.monsterName.TableEntryReference.Equals(nameSearch.TableEntryReference))
            {
                CreateMonster_Data data = monster.monstersProfile;

                foreach (var stat in stats)
                {
                    foreach (var monsterStat in data.stats)
                    {
                        if (stat.statType == monsterStat.statType)
                        {
                            stat.slider.value = monsterStat.statValue;
                            break;
                        }
                    }
                }

                break;
            }
        }
    }

    public void AddContracted(LocalizedString nameSearch)
    {
        foreach (var monster in _monsterManager.monsters)
        {
            if (monster.monstersProfile.monsterName.TableEntryReference.Equals(nameSearch.TableEntryReference))
            {
                monster.contracted = true;
                monster.happiness = 1f;
                break;
            }
        }
    }
    public void RemoveContracted(LocalizedString nameSearch)
    {
        foreach (var monster in _monsterManager.monsters)
        {
            if (monster.monstersProfile.monsterName.TableEntryReference.Equals(nameSearch.TableEntryReference))
            {
                monster.contracted = false;
                break;
            }
        }
    }
}
