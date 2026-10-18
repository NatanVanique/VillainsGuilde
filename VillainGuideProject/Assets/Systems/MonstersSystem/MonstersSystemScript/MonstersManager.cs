using System;
using System.Collections.Generic;
using UnityEngine;

public class MonstersManager : MonoBehaviour
{
    #region Variables

    [SerializeField] MonsterData initialMonster;
    public List<MonsterInfo> ListOfRegisteredMonsters = new();


    public static event Action<MonsterInfo> OnMonsterRegistered;
    #endregion

    #region Setup

    public void Setup() {
        SubscribeToEvents();
        RegisterFirstMonster();
    }

    #endregion

    #region Handle Events

    void SubscribeToEvents() {
        MonsterUiDisplay.OnMonsterCardSelected += HandleMonsterSelected;
    }

    void HandleMonsterSelected(MonsterCardPrefab card) {
        MonsterInfo newInfo = card.ReturnMonsterInfo();
        RegisterMonster(newInfo);
    }

    #endregion

    #region Monster Register

    void RegisterFirstMonster()
    {
        List<TipData> listOfTips = new();

        foreach (var pair in initialMonster.DictionaryOfTips)
        {
            List<TipData> listOsTips = pair.Value;
            int randomIndex = UnityEngine.Random.Range(0, listOsTips.Count);
            TipData newTip = listOsTips[randomIndex];
            switch (pair.Key)
            {
                case 1:
                    listOfTips.Insert(0, newTip);
                    break;
                case 2:
                    listOfTips.Insert(1, newTip);
                    break;
                case 3:
                    listOfTips.Insert(2, newTip);
                    break;
            }
        }

        ListOfTips monsterTips = new(listOfTips);

        MonsterInfo info = new(initialMonster, monsterTips);
        RegisterMonster(info);
    }

    public void RegisterMonster(MonsterInfo info) {
        if (ListOfRegisteredMonsters.Contains(info)) return;

        ListOfRegisteredMonsters.Add(info);

        OnMonsterRegistered?.Invoke(info);
    }

    #endregion

    #region End

    private void OnDestroy() {
        UnsubscribeToEvents();
    }

    void UnsubscribeToEvents() {
        MonsterUiDisplay.OnMonsterCardSelected -= HandleMonsterSelected;
    }

    #endregion
}
