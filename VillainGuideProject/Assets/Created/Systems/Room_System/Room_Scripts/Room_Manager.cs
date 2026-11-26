using DG.Tweening;
using UnityEngine;

public sealed class RoomManager : MonoBehaviour
{
    [Header("Screen Animation")]
    [SerializeField, Min(0f)] private float screenTransitionDuration = 0.45f;
    [SerializeField] private Ease screenTransitionEase = Ease.InOutCubic;
    
    private VisualReferencer visual;
    private RectTransform backgroundContent;
    private float initialContentPositionX;
    private int currentScreen;
    private Transform navigationContent;
    private bool navigationHidden;

    internal int CurrentScreen => currentScreen;
    internal bool NavigationHidden => navigationHidden;

    internal void RestoreScreen(int position, bool hideNavigation)
    {
        if (backgroundContent == null || !TryGetRoomScreen(position, out RectTransform screen))
            return;

        currentScreen = position;
        backgroundContent.DOKill();
        Vector2 offset = backgroundContent.anchoredPosition;
        offset.x = initialContentPositionX - screen.anchoredPosition.x;
        backgroundContent.anchoredPosition = offset;
        SetNavigationContent(!hideNavigation);
    }
    
    private void Start()
    {
        visual = Main_Referencer.Instance.visual;
        if (!TryGetRoomScreen(0, out RectTransform initialScreen))
        {
            return;
        }

        backgroundContent = initialScreen.parent.GetComponent<RectTransform>();
        initialContentPositionX = backgroundContent.anchoredPosition.x;
        navigationContent = visual.previousScreenButton_Room.transform.parent;
        visual.previousScreenButton_Room.onClick.AddListener(PreviousScreen);
        visual.nextScreenButton_Room.onClick.AddListener(NextScreen);
        UpdateNavigationButtons();
    }

    private void Update()
    {
        visual.colorImage_Room.color =
            Main_Referencer.Instance.value.dayGradient_HourValues.Evaluate(
                Main_Referencer.Instance.manager.dayCicleManager.NormalizedDayProgress);
    }

    public void PreviousScreen()
    {
        GoToScreen(PreviousScreenPosition(), false);
    }

    public void NextScreen()
    {
        GoToScreen(NextScreenPosition(), false);
    }

    public void GoToScreen(int position)
    {
        GoToScreen(position, true);
    }

    public void GoToScreen(int position, bool hideNavigation)
    {
        if (!TryGetRoomScreen(position, out RectTransform screen))
        {
            return;
        }

        currentScreen = position;
        DoTweenEffects_Commands.DoTweenAnchorPosX(
            backgroundContent,
            initialContentPositionX - screen.anchoredPosition.x,
            screenTransitionDuration, screenTransitionEase);
        UpdateNavigationButtons();
        if (hideNavigation)
        {
            SetNavigationContent(false);
        }
    }

    public void ReactivateNavigationButtons()
    {
        SetNavigationContent(true);
    }

    private void SetNavigationContent(bool active)
    {
        navigationHidden = !active;
        UpdateNavigationButtons();

        if (navigationContent == null ||
            navigationContent != visual.nextScreenButton_Room.transform.parent ||
            backgroundContent.IsChildOf(navigationContent))
        {
            return;
        }

        foreach (VisualReferencer.RoomScreenReference roomScreen in visual.screensRoom)
        {
            if (roomScreen.screen != null && roomScreen.screen.IsChildOf(navigationContent))
            {
                return;
            }
        }

        navigationContent.gameObject.SetActive(active);
    }

    public bool TryGetRoomScreen(int position, out RectTransform screen)
    {
        foreach (VisualReferencer.RoomScreenReference roomScreen in visual.screensRoom)
        {
            if (roomScreen.position != position)
                continue;

            screen = roomScreen.screen;
            return screen != null;
        }

        screen = null;
        return false;
    }

    private int PreviousScreenPosition()
    {
        int previousPosition = int.MinValue;

        foreach (VisualReferencer.RoomScreenReference screen in visual.screensRoom)
            if (screen.position < currentScreen && screen.position > previousPosition)
            {
                previousPosition = screen.position;
            }
        return previousPosition;
    }

    private int NextScreenPosition()
    {
        int nextPosition = int.MaxValue;

        foreach (VisualReferencer.RoomScreenReference screen in visual.screensRoom)
            if (screen.position > currentScreen && screen.position < nextPosition)
            {
                nextPosition = screen.position;
            }

        return nextPosition;
    }

    private void UpdateNavigationButtons()
    {
        visual.previousScreenButton_Room.gameObject.SetActive(!navigationHidden && PreviousScreenPosition() != int.MinValue);
        visual.nextScreenButton_Room.gameObject.SetActive(!navigationHidden && NextScreenPosition() != int.MaxValue);
    }
}
