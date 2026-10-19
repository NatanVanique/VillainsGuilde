using System;
using UnityEngine;

public class JournalCloseButton : MonoBehaviour
{
    public static event Action OnCloseJournalButtonPreesed;

    public void ClickButton() {
        OnCloseJournalButtonPreesed?.Invoke();
    }
}
