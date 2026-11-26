using UnityEngine;

public sealed class MoneyManager : MonoBehaviour
{
    [SerializeField] private long balance;

    public long Balance => balance;

    internal void RestoreBalance(long savedBalance)
    {
        balance = savedBalance;
    }

    public void Credit(long coins)
    {
        if (coins > 0)
        {
            balance = balance > long.MaxValue - coins ? long.MaxValue : balance + coins;
            GameSaveSystem.RequestAutosave();
        }
    }
}
