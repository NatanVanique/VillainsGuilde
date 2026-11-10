using System;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class InitialStoryManager : MonoBehaviour
{
    [Header("Quads")]
    [SerializeField] private GameObject[] storyQuads;
    private int curQuad = 0;

    [Header("Ui")] 
    [SerializeField] private GameObject advanceText;
    [SerializeField] private GameObject continueButton;

    private void Start()
    {
        ShowQuad(curQuad);
    }

    private void Update()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame)NextQuad();
    }

    public void NextQuad()
    {
        if (curQuad >= storyQuads.Length - 1) return;
        
        curQuad++;
        ShowQuad(curQuad);
        
        if (curQuad == storyQuads.Length - 1)
        {
            advanceText.SetActive(false);
            continueButton.SetActive(true);
        }
    }

    public void ShowQuad(int quadIndex)
    {
        storyQuads[quadIndex].SetActive(true);
        DoTweenEffects_Commands.DoTweenPop(storyQuads[quadIndex].transform, 0.5f, 0.6f);
    }
}
