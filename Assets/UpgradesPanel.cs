using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesPanel : UI_Panel
{
    Librarian lib;
    UpgradePanelDescriptionHelper updh;
    PlayerMemory pm;

    string topBand = "ACEGIKMOQSUWY";
    string bottomBand = "BDFHJLNPRTVXZ";
    List<LetterMask> topBandLetterMasks = new List<LetterMask>();
    List<LetterMask> bottomBandLetterMasks = new List<LetterMask>();

    [SerializeField] Image[] topImages = null;
    [SerializeField] Image[] bottomImages = null;

    [SerializeField] TextMeshProUGUI[] topTMPs = null;
    [SerializeField] TextMeshProUGUI[] bottomTMPs = null;
    [SerializeField] Slider scrollSlider = null;

    [SerializeField] Image selectedLetterImage = null;
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
    [SerializeField] RectTransform unselectionSpot = null;

    [SerializeField] TextMeshProUGUI costTMP = null;

    [SerializeField] TextMeshProUGUI moneyOnHandTMP = null;


    //state
    LetterMask selectedLetterMask = null;
    TrueLetter.Ability abilityInCart;
    int costOfCart = 0;

    int scroll_current = 0;
    int scroll_max;
    int selectedButton = -1;
    LetterMask[] displayedLetterMod_Top = new LetterMask[5];
    LetterMask[] displayedLetterMod_Bottom = new LetterMask[5];
    bool[] isAbilityButtonActivated = new bool[10];


    private void Start()
    {
        lib = Librarian.GetLibrarian();
        scroll_max = topBand.Length - topTMPs.Length;
        //lib.gameController.OnGameStart += HandleOnGameStart;
        updh = GetComponent<UpgradePanelDescriptionHelper>();
        for (int i = 0; i < isAbilityButtonActivated.Length; i++)
        {
            isAbilityButtonActivated[i] = false;
        }
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
        pm = lib.gameController.GetPlayer().GetComponent<PlayerMemory>();
        UnselectLetter();
        PrepLetterMasks();
        AssignLettersMasksToUI();
        PrepAbilityButtons();
        UpdateMoneyOnHand();
    }


    #region Helpers

    private void UpdateMoneyOnHand()
    {
        moneyOnHandTMP.text = pm.GetMoneyOnHand().ToString();
    }
    private void AdjustSelectedLetterDuringScrollLeft()
    {
        if (selectedButton <= 4 && selectedButton >= 0)
        {
            selectedButton++;
            if (selectedButton < 0 || selectedButton > 4)
            {
                UnselectLetter();
            }
        }
        if (selectedButton >= 5)
        {
            selectedButton++;
            if (selectedButton < 5 || selectedButton > 9)
            {
                UnselectLetter();
            }
        }
    }

    private void AdjustSelectedLetterDuringScrollRight()
    {
        if (selectedButton <= 4 && selectedButton >= 0)
        {
            selectedButton--;
            if (selectedButton < 0 || selectedButton > 4)
            {
                UnselectLetter();
            }
        }
        if (selectedButton >= 5)
        {
            selectedButton--;
            if (selectedButton < 5 || selectedButton > 9)
            {
                UnselectLetter();
            }
        }
    }

    private void UnselectLetter()
    {
        selectedButton = -1;
        selectionFrame.position = unselectionSpot.position;
        ResetSelectionPanel();
    }
    private void ResetSelectionPanel()
    {
        selectedLetterTMP.text = "-";
        selectedRarityTMP.text = "-";
        selectedAbilityTMP.text = "-";
        selectedPowerTMP.text = "-";
        selectedExperienceTMP.text = "-/-";
        selectedLetterImage.sprite = sourceLetterTile.GetComponent<SpriteRenderer>().sprite;
        selectedLetterImage.color = sourceLetterTile.GetComponent<SpriteRenderer>().color;
        costOfCart = 0;
        abilityInCart = TrueLetter.Ability.Normal;
    }

    private void PrepAbilityButtons()
    {
        List<TrueLetter.Ability> knownAbilities = pm.GetAllKnownAbilities();
        for (int i = 0; i < abilityPurchaseButtonsImages.Length; i++)
        {
            if (knownAbilities.Contains((TrueLetter.Ability)i))
            {
                LetterTile.SpriteColorYMod sc = sourceLetterTile.GetSpriteColorFromAbility((TrueLetter.Ability)i);
                abilityPurchaseButtonsImages[i].sprite = sc.Sprite;
                abilityPurchaseButtonsImages[i].color = sc.Color;
                isAbilityButtonActivated[i] = true;
            }
        }

    }

    private void PrepLetterMasks()
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
        selectedLetterTMP.text = selectedLetterMask.letter.ToString();
        float rarity = Mathf.Round(selectedLetterMask.rarity);
        if (rarity < 1f)
        {
            selectedRarityTMP.text = "<1%";
        }
        else
        {
            selectedRarityTMP.text = rarity.ToString() + "%";
        }

        selectedAbilityTMP.text = updh.GetDescriptionForAbility(selectedLetterMask.ability);
        selectedPowerTMP.text = selectedLetterMask.PowerMod.ToString();
        selectedExperienceTMP.text = $"{selectedLetterMask.experience_Current} / " +
            $"{selectedLetterMask.experience_NextLevel}";
        selectedLetterImage.sprite = sourceLetterTile.GetSpriteColorFromAbility(selectedLetterMask.ability).Sprite;
        selectedLetterImage.color = sourceLetterTile.GetSpriteColorFromAbility(selectedLetterMask.ability).Color;
        costOfCart = 0;
        abilityInCart = selectedLetterMask.ability;
    }
    private void AssignLettersMasksToUI()
    {
        for (int i = 0; i < topTMPs.Length; i++)
        {
            topTMPs[i].text = displayedLetterMod_Top[i].letter.ToString();
            topImages[i].sprite = sourceLetterTile.GetSpriteColorFromAbility(displayedLetterMod_Top[i].ability).Sprite;
            topImages[i].color = sourceLetterTile.GetSpriteColorFromAbility(displayedLetterMod_Top[i].ability).Color;
        }
        for (int j = 0; j < topTMPs.Length; j++)
        {
            bottomTMPs[j].text = displayedLetterMod_Bottom[j].letter.ToString();
            bottomImages[j].sprite = sourceLetterTile.GetSpriteColorFromAbility(displayedLetterMod_Bottom[j].ability).Sprite;
            bottomImages[j].color = sourceLetterTile.GetSpriteColorFromAbility(displayedLetterMod_Bottom[j].ability).Color;
        }
    }

    #endregion

    #region Public Button Handlers
    public void ReturnToPreviousContext()
    {
        lib.ui_Controller.SetContext(UI_Controller.Context.Overworld);
    }

    public void SelectLetterToInspect(int buttonIndex)
    {
        selectedButton = buttonIndex;
        if (selectedButton == -1) { return; }
        // move selection frame to that button Index
        if (buttonIndex < topTMPs.Length)
        {
            selectionFrame.position = topTMPs[buttonIndex].GetComponent<RectTransform>().position;
            selectedLetterMask = displayedLetterMod_Top[buttonIndex];
        }
        if (buttonIndex >= topTMPs.Length)
        {
            selectionFrame.position = bottomTMPs[buttonIndex - topTMPs.Length].GetComponent<RectTransform>().position;
            selectedLetterMask = displayedLetterMod_Bottom[buttonIndex - topTMPs.Length];
        }

        // Display these selected PlayerLetterMod at the top.
        DisplaySelectedLetter();
    }

    public void ScrollLettersLeft()
    {
        if (scroll_current <= 0) { return; }
        scroll_current--;
        AdjustSelectedLetterDuringScrollLeft();
        scrollSlider.value = scroll_current;
        for (int i = 0; i < topTMPs.Length; i++)
        {
            displayedLetterMod_Top[i] = topBandLetterMasks[i + scroll_current];           
        }
        for (int j = 0; j < topTMPs.Length; j++)
        {
            displayedLetterMod_Bottom[j] = bottomBandLetterMasks[j + scroll_current];            
        }
        AssignLettersMasksToUI();
        SelectLetterToInspect(selectedButton);
    }

    public void ScrollLettersRight()
    {
        if (scroll_current >= scroll_max) { return; }
        scroll_current++;
        AdjustSelectedLetterDuringScrollRight();
        scrollSlider.value = scroll_current;
        for (int i = 0; i < topTMPs.Length; i++)
        {
            displayedLetterMod_Top[i] = topBandLetterMasks[i + scroll_current];
        }
        for (int j = 0; j < topTMPs.Length; j++)
        {
            displayedLetterMod_Bottom[j] = bottomBandLetterMasks[j + scroll_current];
        }
        AssignLettersMasksToUI();
        SelectLetterToInspect(selectedButton);
    }

    

    public void SelectLetterPower(int buttonIndex)
    {
        if (isAbilityButtonActivated[buttonIndex] == false) { return; }

        abilityInCart = (TrueLetter.Ability)buttonIndex;
        selectedAbilityTMP.text = updh.GetDescriptionForAbility(abilityInCart);
        if (abilityInCart == selectedLetterMask.ability)
        {
            selectedLetterImage.sprite = sourceLetterTile.GetSpriteColorFromAbility(selectedLetterMask.ability).Sprite;
            selectedLetterImage.color = sourceLetterTile.GetSpriteColorFromAbility(selectedLetterMask.ability).Color;
        }
        else
        {
            selectedLetterImage.sprite = sourceLetterTile.GetSpriteColorFromAbility(abilityInCart).Sprite;
            Color newColor = sourceLetterTile.GetSpriteColorFromAbility(abilityInCart).Color;
            newColor.a = 0.5f;
            selectedLetterImage.color = newColor;
        }
        // Update cost of cart
        if (selectedLetterMask.ability == abilityInCart)
        {
            costOfCart = 0;
        }
        else
        {
            costOfCart = Mathf.RoundToInt(selectedLetterMask.rarity) * updh.GetCostForAbility(abilityInCart);
        }      

        UpdateCostTMPwithCostofCart();
    }

    private void UpdateCostTMPwithCostofCart()
    {
        costTMP.text = costOfCart.ToString();
        if (pm.CheckMoney(costOfCart))
        {
            costTMP.color = Color.black;
        }
        else
        {
            costTMP.color = Color.red;
        }
    }

    public void HandleBuyButtonPress()
    {
        if (pm.CheckSpendMoney(costOfCart))
        {
            UpdateMoneyOnHand();
            selectedLetterMask.ability = abilityInCart;
            Debug.Log($"{selectedLetterMask.letter} now is {abilityInCart}");
            AssignLettersMasksToUI();
            DisplaySelectedLetter();
            costOfCart = 0;
            UpdateCostTMPwithCostofCart();
        }
        else
        {
            //play "insufficient funds" sound
        }

    }
    #endregion




}
