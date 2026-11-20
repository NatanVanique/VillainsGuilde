using UnityEngine;
using UnityEngine.UI;

public sealed class LifeManager : MonoBehaviour
{
    [Header("Life")]
    private int playerLives = 3;
    [SerializeField] private Sprite lostHealthSprite;

    [Header("Animation")]
    private float animationDuration = 0.4f;
    private float animationIntensity = 0.6f;

    public int CurrentLives { get; private set; }

    private VisualReferencer Visual => Main_Referencer.Instance.visual;

    private void Awake()
    {
        CurrentLives = playerLives;
    }

    private void Start()
    {
        RefreshHealth(Visual.healthImages_Life, false, CurrentLives);
        RefreshHealth(Visual.failedHealthImages_Life, false, CurrentLives);
        Visual.failedContainer_Life.SetActive(false);
    }
    
    public void LoseLife(int amount)
    {
        if (amount <= 0 || CurrentLives == 0) return;

        int previousLives = CurrentLives;
        CurrentLives = Mathf.Max(0, CurrentLives - amount);
        RefreshHealth(Visual.healthImages_Life, true, previousLives);
        RefreshHealth(Visual.failedHealthImages_Life, true, previousLives);
    }

    public void ShowFailedPanel()
    {
        Main_Referencer.Instance.manager.barsManager.ShowBars();
        RefreshHealth(Visual.failedHealthImages_Life, false, CurrentLives);
        DoTweenEffects_Commands.DoTweenShowPanel(Visual.failedContainer_Life, animationDuration, animationIntensity);
    }

    public void HideFailedPanel()
    {
        DoTweenEffects_Commands.DoTweenHidePanel(Visual.failedContainer_Life);
        Main_Referencer.Instance.manager.barsManager.HideBars();
    }

    private void RefreshHealth(Image[] images, bool animate, int previousLives)
    {
        if (images == null) return;

        for (int i = 0; i < images.Length; i++)
        {
            Image image = images[i];
            if (image == null || i < CurrentLives) continue;
            image.sprite = lostHealthSprite;

        }
    }

    //Teste!!
    //1 = perder uma vida e 2 = abrir o painel failed
    private void Update()
    {
        var keyboard = UnityEngine.InputSystem.Keyboard.current;
        if (keyboard == null) return;
        if (keyboard.digit1Key.wasPressedThisFrame) LoseLife(1);
        if (keyboard.digit2Key.wasPressedThisFrame) ShowFailedPanel();
    }
}