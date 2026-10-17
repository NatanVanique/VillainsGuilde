using System.Collections.Generic;
using UnityEngine;

public class MonstersManager : MonoBehaviour
{
    #region Variables

    public List<MonsterInfo> ListOfRegisteredMonsters = new();

    #endregion

    #region Setup

    public void Setup() {
        SubscribeToEvents();
    }

    #endregion

    #region Handle Events

    void SubscribeToEvents() {

    }

    void HandleMonsterSelected(MonsterInfo info) {
        RegisterMonster(info);
    }

    #endregion

    #region Monster Register

    public void RegisterMonster(MonsterInfo info) {
        if (ListOfRegisteredMonsters.Contains(info)) return;

        ListOfRegisteredMonsters.Add(info);
    }

    #endregion

    #region End

    private void OnDestroy() {
        UnsubscribeToEvents();
    }

    void UnsubscribeToEvents() {

    }

    #endregion
}
