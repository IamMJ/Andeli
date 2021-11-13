using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradesPanel : UI_Panel
{
    Librarian lib;
    UpgradePanelDescriptionHelper updh;

    string topBand = "ACEGIKMOQSUWY";
    string bottomBand = "BDFHJLNPRTVXZ";
    List<LetterMask> topBandLetterMasks = new List<LetterMask>();
    List<LetterMask> bottomBandLetterMasks = new List<LetterMask>();

    [SerializeField] TextMeshProUGUI[] topTMPs = null;
    [SerializeField] TextMeshProUGUI[] bottomTMPs = null;
    [SerializeField] Slider scrollSlider = null;

    [SerializeField] TextMeshProUGUI selectedLetterTMP = null;
    [SerializeField] TextMeshProUGUI selectedRarityTMP = null;
    [SerializeField] TextMeshProUGUI selectedBlurbTMP = null;
    [SerializeField] TextMeshProUGUI selectedPowerTMP = null;
    [SerializeField] TextMeshProUGUI selectedExperienceTMP = null;
    [SerializeField] TextMeshProUGUI selectedAbilityTMP = null;

    [SerializeField] LetterTile sourceLetterTile = null;
    [SerializeField] Image[] abilityPurchaseButtonsImages = null;
    [SerializeField] TextMeshProUGUI[] abilityPurchaseButtonsTMPs = null;

    [SerializeField] RectTransform selectionFrame = null;


    //state
    LetterMask selectedLetterMod = null;
    int scroll_current = 0;
    int scroll_max;
    int selectedButton = 0;
    LetterMask[] displayedLetterMod_Top = new LetterMask[5];
    LetterMask[] displayedLetterMod_Bottom = new LetterMask[5];

    private void Start()
    {
        lib = Librarian.GetLibrarian();
        scroll_max = topBand.Length - topTMPs.Length;
        //lib.gameController.OnGameStart += HandleOnGameStart;
        updh = GetComponent<UpgradePanelDescriptionHelper>();
    }

    public override void ShowHideElements(bool shouldBeShown)
    {
        base.ShowHideElements(shouldBeShown);
        if (shouldBeShown)
        {
            InitializeUpgradePanel();
        }
    }

    private void InitializeUpgradePanel()
    {
        PrepLetterMods();
        AssignInitialLettersModsToUI();
        PrepAbilityButtons();
    }

    #region Helpers
    private void PrepAbilityButtons()
    {
        for (int i = 0; i < abilityPurchaseButtonsImages.Length; i++)
        {
            LetterTile.SpriteColorYMod sc = sourceLetterTile.GetSpriteColorFromAbility((TrueLetter.Ability)i);
            abilityPurchaseButtonsImages[i].sprite = sc.Sprite;
            abilityPurchaseButtonsImages[i].color = sc.Color;
        }
    }

    private void PrepLetterMods()
    {
        List<LetterMask> letterMasks = new List<LetterMask>();
        letterMasks = lib.gameController.GetPlayer().GetComponent<LetterMaskHolder>().GetLetterMasks();
        //Debug.Log($"received {letterMasks.Count} letter masks, and #3 is {letterMasks[2].letter}");
        
        foreach (LetterMask letterMask in letterMasks)
        {
            if (topBand.Contains(letterMask.letter.ToString()))
            {
                topBandLetterMasks.Add(letterMask);
            }
            else
            {
                bottomBandLetterMasks.Add(letterMask);
            }
        }

        for (int i = 0; i < topTMPs.Length; i++)
        {
            displayedLetterMod_Top[i] = topBandLetterMasks[i];
            displayedLetterMod_Bottom[i] = bottomBandLetterMasks[i];
        }
    }
    private void DisplaySelectedLetter()
    {
        selectedLetterTMP.text = "asdf"; //selectedLetterMod.letter.ToString();
        float rarity = Mathf.Round(selectedLetterMod.rarity);
        selectedRarityTMP.text = rarity.ToString() + "%";
        selectedAbilityTMP.text = updh.GetDescriptionForAbility(selectedLetterMod.ability);
        selectedPowerTMP.text = selectedLetterMod.PowerMod.ToString();
        selectedExperienceTMP.text = $"{selectedLetterMod.experience_Current} / " +
            $"{selectedLetterMod.experience_NextLevel}";
    }
    private void AssignInitialLettersModsToUI()
    {
        for (int i = 0; i < topTMPs.Length; i++)
        {
            topTMPs[i].text = topBandLetterMasks[i].letter.ToString();
        }
        for (int j = 0; j < topTMPs.Length; j++)
        {
            bottomTMPs[j].text = bottomBandLetterMasks[j].letter.ToString();
        }
    }

    #endregion


    #region Public Button Handlers
    public void ReturnToPreviousContext()
    {
        lib.ui_Controller.ReturnToPreviousContext();
    }

    public void SelectLetterToInspect(int buttonIndex)
    {
        selectedButton = buttonIndex;
        // move selection frame to that button Index
        if (buttonIndex < topTMPs.Length)
        {
            selectionFrame.position = topTMPs[buttonIndex].GetComponent<RectTransform>().position;
            selectedLetterMod = displayedLetterMod_Top[buttonIndex];
        }
        if (buttonIndex >= topTMPs.Length)
        {
            selectionFrame.position = bottomTMPs[buttonIndex - topTMPs.Length].GetComponent<RectTransform>().position;
            selectedLetterMod = displayedLetterMod_Bottom[buttonIndex - topTMPs.Length];
        }

        // Display these selected PlayerLetterMod at the top.
        DisplaySelectedLetter();
    }

    public void ScrollLettersLeft()
    {
        if (scroll_current <= 0) { return; }
        scroll_current--;
        scrollSlider.value = scroll_current;
        for (int i = 0; i < topTMPs.Length; i++)
        {
            displayedLetterMod_Top[i] = topBandLetterMasks[i + scroll_current];
            topTMPs[i].text = displayedLetterMod_Top[i].letter.ToString();
        }
        for (int j = 0; j < topTMPs.Length; j++)
        {
            displayedLetterMod_Bottom[j] = bottomBandLetterMasks[j + scroll_current];
            bottomTMPs[j].text = displayedLetterMod_Bottom[j].letter.ToString();
        }
        SelectLetterToInspect(selectedButton);
    }

    public void ScrollLettersRight()
    {
        if (scroll_current >= scroll_max) { return; }
        scroll_current++;
        scrollSlider.value = scroll_current;
        for (int i = 0; i < topTMPs.Length; i++)
        {
            displayedLetterMod_Top[i] = topBandLetterMasks[i + scroll_current];
            topTMPs[i].text = displayedLetterMod_Top[i].letter.ToString();
        }
        for (int j = 0; j < topTMPs.Length; j++)
        {
            displayedLetterMod_Bottom[j] = bottomBandLetterMasks[j + scroll_current];
            bottomTMPs[j].text = displayedLetterMod_Bottom[j].letter.ToString();
        }
        SelectLetterToInspect(selectedButton);
    }
    #endregion


   
}
