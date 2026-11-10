using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public sealed class ContractManager : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private MonsterManager _monsterManager;
    
    /*[Header("LookingForWork")]
    [SerializeField] private Image bigImage_LookingForWork;
    [SerializeField] private TextMeshProUGUI nameText_LookingForWork;*/
    
    [Header("ContractTolltip")]
    [SerializeField] private Image bigImage_ContractTolltip;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText_ContractTolltip;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Slider[] stats;

    private void GenerateStats(LocalizedString nameSearch)
    {
        foreach (var monster in _monsterManager.monsters)
        {
            if (monster.monstersProfile.monsterName.TableEntryReference.Equals(nameSearch.TableEntryReference))
            {
                CreateMonster_Data data = monster.monstersProfile;

                float[] statsValues =
                {
                    data.strength,
                    data.speed,
                    data.intelligence,
                    data.charisma,
                    data.endurance,
                    data.delicacy,
                    data.luck,
                    data.enthusiasm
                };

                for (int i = 0; i < stats.Length; i++)
                {
                    stats[i].value = statsValues[i];
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
