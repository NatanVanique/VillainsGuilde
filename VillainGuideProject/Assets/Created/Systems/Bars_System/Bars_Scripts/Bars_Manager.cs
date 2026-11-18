using UnityEngine;
using System.Collections;

public class Bars_Manager : MonoBehaviour
{
    [Header("System Variables")]
    private Coroutine movementBars;
    
    public void ShowBars()
    {
        StopCoroutine(movementBars);
        movementBars = StartCoroutine(MoveBars(1));
    }

    public void HideBars()
    {
        StopCoroutine(movementBars);
        movementBars = StartCoroutine(MoveBars(0));
    }

    private IEnumerator MoveBars(float targetScale)
    {
        while (Main_Referencer.Instance.visual.top_Bars.transform.localScale.y != targetScale)
        {
            Vector3 scale = Main_Referencer.Instance.visual.top_Bars.transform.localScale;
            scale.y = Mathf.MoveTowards(scale.y, targetScale, Main_Referencer.Instance.value.velocity_Bars * Time.deltaTime);

            Main_Referencer.Instance.visual.top_Bars.transform.localScale = scale;
            Main_Referencer.Instance.visual.bottom_Bars.transform.localScale = scale;

            yield return null;
        }
    }
}
