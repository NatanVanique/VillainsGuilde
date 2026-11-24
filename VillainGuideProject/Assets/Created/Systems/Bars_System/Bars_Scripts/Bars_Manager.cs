using System;
using System.Collections.Generic;
using UnityEngine;

public class Bars_Manager : MonoBehaviour
{
    private readonly HashSet<GameObject> openPopups = new();

    public event Action<bool> VisibilityChanged;

    public void SetPopupOpen(GameObject popup, bool isOpen)
    {
        if (popup == null)
        {
            return;
        }

        if (isOpen)
        {
            openPopups.Add(popup);
        }
        else
        {
            openPopups.Remove(popup);
        }

        Main_Referencer references = Main_Referencer.Instance;
        if (references == null || references.visual == null)
        {
            return;
        }

        GameObject canvas = references.visual.barsCanvas_Bars;
        bool visible = openPopups.Count > 0;
        if (canvas == null || canvas.activeSelf == visible)
        {
            return;
        }

        canvas.SetActive(visible);
        VisibilityChanged?.Invoke(visible);
    }

    public void ShowBars() => SetPopupOpen(gameObject, true);

    public void HideBars() => SetPopupOpen(gameObject, false);
}
