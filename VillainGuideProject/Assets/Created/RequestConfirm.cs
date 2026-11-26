using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RequestConfirm : MonoBehaviour
{
    private Coroutine closeRoutine;
    [SerializeField, Min(0f)] private float optionSwitchGraceTime = 0.12f;

    public void OpenConfirm(GameObject confirmObject)
    {
        gameObject.SetActive(false);
        confirmObject.SetActive(true);   
    }
    
    public void CloseConfirm(GameObject button)
    {
        if (closeRoutine != null)
            StopCoroutine(closeRoutine);

        closeRoutine = StartCoroutine(CloseAfterLeavingConfirmation(button));
    }

    public void CancelConfirm(GameObject button)
    {
        closeRoutine = null;
        gameObject.SetActive(false);
        button.SetActive(true);
    }

    private IEnumerator CloseAfterLeavingConfirmation(GameObject button)
    {
        yield return new WaitForSeconds(optionSwitchGraceTime);

        if (PointerIsOverAnOption())
        {
            closeRoutine = null;
            yield break;
        }

        CancelConfirm(button);
    }

    private bool PointerIsOverAnOption()
    {
        if (!TryGetMousePosition(out Vector2 mousePosition))
            return false;

        foreach (Transform child in transform)
        {
            if (child.TryGetComponent<Button>(out _) && child is RectTransform option && RectTransformUtility.RectangleContainsScreenPoint(
                    option,
                    mousePosition,
                    GetEventCamera()))
                return true;
        }

        return false;
    }

    private bool TryGetMousePosition(out Vector2 mousePosition)
    {
        if (Mouse.current == null)
        {
            mousePosition = default;
            return false;
        }

        mousePosition = Mouse.current.position.ReadValue();
        return true;
    }

    private Camera GetEventCamera()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        return canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? canvas.worldCamera
            : null;
    }
}
