using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LetterPowerMenuDriver : MonoBehaviour
{
    GameController gc;
    VictoryMeter vm;
    string topBand = "ACEGIKMOQSUWY";
    string bottomBand = "BDFHJLNPRTVXZ";
    List<PlayerLetterMod> topBandLetters = new List<PlayerLetterMod>();
    List<PlayerLetterMod> bottomBandLetters = new List<PlayerLetterMod>();

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
    PlayerLetterMod selectedLetterMod = null;
    int scroll_current = 0;
    int scroll_max;
    int selectedButton = 0;
    PlayerLetterMod[] displayedLetterMod_Top = new PlayerLetterMod[5];
    PlayerLetterMod[] displayedLetterMod_Bottom = new PlayerLetterMod[5];
    private void Start()
    {
        gc = FindObjectOfType<GameController>();
        scroll_max = topBand.Length - topTMPs.Length;
        PrepLetterMods();
        AssignInitialLettersModsToUI();
        PrepAbilityButtons();
    }

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
        List<PlayerLetterMod> letters = new List<PlayerLetterMod>();
        letters = gc.GetPlayer().GetComponent<LetterModHolder>().GetLetterMods();
        foreach (var letter in letters)
        {
            if (topBand.Contains(letter.GetLetter().ToString()))
            {
                topBandLetters.Add(letter);
            }
            else
            {
                bottomBandLetters.Add(letter);
            }
        }

        for (int i = 0; i < topTMPs.Length; i++)
        {
            displayedLetterMod_Top[i] = topBandLetters[i];
            displayedLetterMod_Bottom[i] = bottomBandLetters[i];
        }
    }

    private void AssignInitialLettersModsToUI()
    {
        for (int i = 0; i < topTMPs.Length; i++)
        {
            topTMPs[i].text = topBandLetters[i].GetLetter().ToString();
        }
        for (int j = 0; j < topTMPs.Length; j++)
        {
            bottomTMPs[j].text = bottomBandLetters[j].GetLetter().ToString();
        }
    }

    public void HideMenu()
    {
        if (!gc)
        {
            gc = FindObjectOfType<GameController>();
        }
        gc.ResumeGameSpeed(false);
        gameObject.SetActive(false);
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

    private void DisplaySelectedLetter()
    {
        selectedLetterTMP.text = selectedLetterMod.GetLetter().ToString();
        float rarity = Mathf.Round(selectedLetterMod.GetRarity());
        selectedRarityTMP.text = rarity.ToString() + "%";
        selectedBlurbTMP.text = selectedLetterMod.GetBlurb();
        selectedAbilityTMP.text = selectedLetterMod.GetAbilityDescription();
        selectedPowerTMP.text = selectedLetterMod.GetPower().ToString();
        selectedExperienceTMP.text = selectedLetterMod.GetExperienceString();
    }

    public void ScrollLettersLeft()
    {
        if (scroll_current <= 0) { return; }
        scroll_current--;
        scrollSlider.value = scroll_current;
        for (int i = 0; i < topTMPs.Length; i++)
        {
            displayedLetterMod_Top[i] = topBandLetters[i + scroll_current];
            topTMPs[i].text = displayedLetterMod_Top[i].GetLetter().ToString();
        }
        for (int j = 0; j < topTMPs.Length; j++)
        {
            displayedLetterMod_Bottom[j] = bottomBandLetters[j + scroll_current];
            bottomTMPs[j].text = displayedLetterMod_Bottom[j].GetLetter().ToString();
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
            displayedLetterMod_Top[i] = topBandLetters[i + scroll_current];
            topTMPs[i].text = displayedLetterMod_Top[i].GetLetter().ToString();
        }
        for (int j = 0; j < topTMPs.Length; j++)
        {
            displayedLetterMod_Bottom[j] = bottomBandLetters[j + scroll_current];
            bottomTMPs[j].text = displayedLetterMod_Bottom[j].GetLetter().ToString();
        }
        SelectLetterToInspect(selectedButton);
    }
}
