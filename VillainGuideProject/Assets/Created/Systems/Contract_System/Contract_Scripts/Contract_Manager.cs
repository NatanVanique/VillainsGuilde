using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public sealed class ContractManager : MonoBehaviour
{
    public void ChangeTab()
    {
        Main_Referencer.Instance.visual.lookingForJobCanvas_LookingForJob.enabled =
            (!Main_Referencer.Instance.visual.lookingForJobCanvas_LookingForJob.enabled);
        Main_Referencer.Instance.visual.contractTooltipCanvas_ContractTolltip.enabled =
            (!Main_Referencer.Instance.visual.contractTooltipCanvas_ContractTolltip.enabled);
        UpdateTab();
    }
    private void UpdateTab()
    {
    }
    
    private void UpdateTexts(LocalizedString nameSearch)
    {
        Main_Referencer.Instance.visual.nameText_ContractTolltip.StringReference = 
            Main_Referencer.Instance.monster.monsters[nameSearch.TableEntryReference].monstersProfile.monsterName;
        Main_Referencer.Instance.visual.descriptionText_ContractTolltip.StringReference = 
            Main_Referencer.Instance.monster.monsters[nameSearch.TableEntryReference].monstersProfile.monsterDescription;
    }
    
    private void GenerateStats(LocalizedString nameSearch)
    {
        foreach (var monster in Main_Referencer.Instance.monster.monsters)
        {
            if (monster.monstersProfile.monsterName.TableEntryReference.Equals(nameSearch.TableEntryReference))
            {
                CreateMonster_Data data = monster.monstersProfile;

                foreach (var stat in Main_Referencer.Instance.visual.statsSlider_ContractTolltip)
                {
                    foreach (var monsterStat in data.baseStats)
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
        foreach (var monster in Main_Referencer.Instance.monster.monsters)
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
        foreach (var monster in Main_Referencer.Instance.monster.monsters)
        {
            if (monster.monstersProfile.monsterName.TableEntryReference.Equals(nameSearch.TableEntryReference))
            {
                monster.contracted = false;
                break;
            }
        }
    }
}
