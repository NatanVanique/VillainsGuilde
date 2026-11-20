
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class Bars_Manager : MonoBehaviour
{
    private Coroutine movementBars;

    private VisualReferencer Visual => Main_Referencer.Instance.visual;

    private void Start()
    {
        SetBarsScale(0);
        SetBackgroundAlpha(0);
        SetBackgroundActive(false);
    }

    public void ShowBars()
    {
        if (movementBars != null) StopCoroutine(movementBars);
        movementBars = StartCoroutine(MoveBars(1, 218));
    }

    public void HideBars()
    {
        if (movementBars != null) StopCoroutine(movementBars);
        movementBars = StartCoroutine(MoveBars(0, 0));
    }

    private IEnumerator MoveBars(float targetScale, float targetAlpha)
    {
        if (targetAlpha > 0)
        {
            SetBackgroundActive(true);
        }

        float currentAlpha = Visual.background_Bars.color.a * 255;

        while (!Mathf.Approximately(Visual.top_Bars.transform.localScale.y, targetScale) ||
               !Mathf.Approximately(currentAlpha, targetAlpha))
        {
            float velocity = Main_Referencer.Instance.value.velocity_Bars * Time.deltaTime;
            float scaleY = Mathf.MoveTowards(Visual.top_Bars.transform.localScale.y, targetScale, velocity);
            SetBarsScale(scaleY);
            
            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, velocity * 255);
            SetBackgroundAlpha(currentAlpha);

            yield return null;
        }

        SetBackgroundAlpha(targetAlpha);

        if (targetAlpha == 0)
        {
            SetBackgroundActive(false);
        }

        movementBars = null;
    }

    private void SetBarsScale(float scaleY)
    {
        Vector3 topScale = Visual.top_Bars.transform.localScale;
        Vector3 bottomScale = Visual.bottom_Bars.transform.localScale;

        topScale.y = scaleY;
        bottomScale.y = scaleY;

        Visual.top_Bars.transform.localScale = topScale;
        Visual.bottom_Bars.transform.localScale = bottomScale;
    }

    private void SetBackgroundAlpha(float alpha)
    {
        Color color = Visual.background_Bars.color;
        color.a = alpha / 255f;

        Visual.background_Bars.color = color;
    }

    private void SetBackgroundActive(bool isActive)
    {
        Visual.background_Bars.gameObject.SetActive(isActive);
    }
}
