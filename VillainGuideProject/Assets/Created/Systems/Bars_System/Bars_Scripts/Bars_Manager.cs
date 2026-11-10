using UnityEngine;
using System.Collections;

public class Bars_Manager : MonoBehaviour
{
    [Header("Bar Objects")]
    [SerializeField] private GameObject topBar;
    [SerializeField] private GameObject bottomBar;

    [Header("System Variables")] 
    [SerializeField] private float moveVelocity;
    
    [Header("Coroutine")]
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
        while (topBar.transform.localScale.y != targetScale)
        {
            Vector3 scale = topBar.transform.localScale;
            scale.y = Mathf.MoveTowards(scale.y, targetScale, moveVelocity * Time.deltaTime);

            topBar.transform.localScale = scale;
            bottomBar.transform.localScale = scale;

            yield return null;
        }
    }
}
