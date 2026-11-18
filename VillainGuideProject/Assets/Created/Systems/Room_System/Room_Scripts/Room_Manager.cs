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
    
    private void Start()
    {
        visual = Main_Referencer.Instance.visual;
        if (!TryGetRoomScreen(0, out RectTransform initialScreen))
        {
            return;
        }

        backgroundContent = initialScreen.parent.GetComponent<RectTransform>();
        initialContentPositionX = backgroundContent.anchoredPosition.x;
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
        GoToScreen(PreviousScreenPosition());
    }

    public void NextScreen()
    {
        GoToScreen(NextScreenPosition());
    }

    public void GoToScreen(int position)
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
        visual.previousScreenButton_Room.gameObject.SetActive(PreviousScreenPosition() != int.MinValue);
        visual.nextScreenButton_Room.gameObject.SetActive(NextScreenPosition() != int.MaxValue);
    }
}
