using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class CombatPanel : UI_Panel
{
    //init
    //[SerializeField] GameObject mainMenu = null;
    //[SerializeField] GameObject optionsMenu = null;
    //[SerializeField] GameObject optionsMenuButton = null;
    //[SerializeField] GameObject letterPowersMenu = null;
    //[SerializeField] GameObject letterPowersMenuButton = null;

    [SerializeField] GameObject[] topPanelElements = null;

    [SerializeField] Slider victoryBarSlider = null;

    [SerializeField] Slider wordEraseSliderBG = null;
    [SerializeField] Image attackButtonMain = null;
    [SerializeField] Image attackButtonRunes = null;
    [SerializeField] Image attackButtonGlow = null;
    [SerializeField] TextMeshProUGUI powerMeterTMP = null;

    [SerializeField] SpriteRenderer blankTileDefault = null;
    [SerializeField] Image[] wordboxImages = null;
    [SerializeField] TextMeshProUGUI[] wordboxTMPs = null;

    [SerializeField] GameObject ignitionChancePanel = null;
    [SerializeField] TextMeshProUGUI ignitionChanceTMP = null;

    [SerializeField] GameObject debugMenuPanel = null;

    WordBuilder playerWB;
    BagManager bagman;
    WordWeaponizer playerWWZ;
    GameController gc;
    SceneLoader sl;
    Tutor tutor;
    SwordGlowDriver sgd;
    WordValidater wv;

    //state
    [SerializeField] bool wasLongPress = false;
    int selectedSwordLetterIndex = -1;
    bool isFireWeaponButtonPressed = false;
    bool isEraseWeaponButtonPressed = false;
    float timeButtonDepressed = 0;
    [SerializeField] bool canPressAttackButton = false;
    
    public float timeSpentLongPressing { get; private set; }

    private void Start()
    {
        Librarian lib = FindObjectOfType<Librarian>();
        sgd = GetComponent<SwordGlowDriver>();
        wv = lib.wordValidater;
        bagman = FindObjectOfType<BagManager>();

        ClearWordBar();
        ResetWordTilesToMax();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerWB)
        {
            HandleFireWordButtonPressed();
            HandleEraseWordButtonPressed();
            HandleSwordButtonBeingPressed();
        }
    }

    public void SetPlayerObject(WordBuilder newPlayerWB, WordWeaponizer newPlayerWWZ, BagManager newPlayerBagman)
    {
        playerWB = newPlayerWB;
        playerWWZ = newPlayerWWZ;
        bagman = newPlayerBagman;
    }

    #region Initial Button Press Handlers
    public void OnPressDownFireWord()
    {
        //if (playerWB.GetCurrentWord().Length == 0) { return; }
        if (!canPressAttackButton) { return; }

        if (isEraseWeaponButtonPressed == false)
        {
            isFireWeaponButtonPressed = true;
        }

    }

    public void OnReleaseFireWord()
    {
        IncompleteLongPress_WordBoxActions();
    }

    public void OnPressDownEraseWord()
    {
        if (playerWB.GetCurrentWord().Length == 0) { return; }

        if (isFireWeaponButtonPressed == false)
        {
            isEraseWeaponButtonPressed = true;
        }
    }

    public void OnReleaseEraseWord()
    {
        IncompleteLongPress_WordBoxActions();
    }

    public void OnPressDownLetterOnSword(int index)
    {

        selectedSwordLetterIndex = index;

    }

    public void OnReleaseLetterOnSword(int index)
    {
        if (selectedSwordLetterIndex == -1) { return; }
        if (wasLongPress)
        {
            NotifyWordBuilderToDestroyLetterAtIndex(index);
        }
        else
        {
            NotifyWordBuilderToPassLetterToBagAtIndex(index);
        }
        wasLongPress = false;
        selectedSwordLetterIndex = -1;
        timeButtonDepressed = 0;
    }


    public void PressTutorialOkayButton()
    {
        tutor.AdvanceToNextStepViaClick();
    }
    #endregion

    #region Button Helpers

    private void HandleSwordButtonBeingPressed()
    {

        if (selectedSwordLetterIndex != -1)
        {
            timeButtonDepressed += Time.unscaledDeltaTime;
            if (timeButtonDepressed >= UIParameters.LongPressTime)
            {
                wasLongPress = true;
                OnReleaseLetterOnSword(selectedSwordLetterIndex);
            }
        }
    }
    private void HandleEraseWordButtonPressed()
    {
        if (isEraseWeaponButtonPressed)
        {
            timeButtonDepressed += Time.unscaledDeltaTime;
            Time.timeScale = 0;
            FillWordEraseSlider(timeButtonDepressed / UIParameters.LongPressTime);
            if (timeButtonDepressed >= UIParameters.LongPressTime)
            {
                //Placeholder for anything related to erasing the word
                CompleteLongPress_WordBoxActions();
            }
        }
    }

    private void HandleFireWordButtonPressed()
    {
        if (isFireWeaponButtonPressed)
        {
            timeButtonDepressed += Time.unscaledDeltaTime;
            Time.timeScale = 0;
            UpdateAttackButtonPressGlow(timeButtonDepressed / UIParameters.LongPressTime);
            if (timeButtonDepressed >= UIParameters.LongPressTime)
            {
                if (playerWWZ.AttemptToFireWordAsPlayer())
                {
                    CompleteLongPress_WordBoxActions();
                }                
            }
        }
    }

    private void FillWordEraseSlider(float amount)
    {
        wordEraseSliderBG.value = amount;
    }

    private void ClearWordEraseSlider()
    {
        wordEraseSliderBG.value = 0f;
    }

    private void UpdateAttackButtonPressGlow(float amount)
    {
        attackButtonGlow.fillAmount = amount;
    }

    private void CompleteLongPress_WordBoxActions()
    {
        playerWB.ClearCurrentWord();
        //playerWB.ClearPowerLevel();
        IncompleteLongPress_WordBoxActions();
    }
    private void IncompleteLongPress_WordBoxActions()
    {
        ClearWordEraseSlider();
        UpdateAttackButtonPressGlow(0);
        timeButtonDepressed = 0;
        Time.timeScale = 1f;
        isFireWeaponButtonPressed = false;
        isEraseWeaponButtonPressed = false;

    }
    #endregion

    #region Letter Tile Helpers

    private void NotifyWordBuilderToPassLetterToBagAtIndex(int index)
    {
        playerWB.RemoveLetterFromSwordAndSendToBag(index);
    }

    private void NotifyWordBuilderToDestroyLetterAtIndex(int index)
    {
        ParticleSystem.MainModule newMod = wordboxImages[index].GetComponent<ParticleSystem>().main;
        newMod.startColor = wordboxImages[index].color;
        wordboxImages[index].GetComponent<ParticleSystem>().Play();
        playerWB.RemoveLetterFromSwordAndDestroy(index);
        // Get bool back from wb. if true, show a smoke animation of letter being destroyed
    }

    //private void DestroyLetterFromSpecificTile(int indexInTiles)
    //{

    //    RemoveParticleEffectsAtIndexInWord(indexInTiles + wordbarScroll);

    //    for (int i = indexInTiles; i < playerWB.GetCurrentWordLength()-1; i++)
    //    {
    //        wordboxTMPs[indexInTiles].text = wordboxTMPs[indexInTiles + 1].text;
    //        wordboxImages[indexInTiles].sprite = wordboxImages[indexInTiles + 1].sprite;
    //        if (wordboxImages[indexInTiles + 1].gameObject.transform.childCount > 0)
    //        {
    //            GameObject particleGO = wordboxImages[indexInTiles + 1].gameObject.transform.GetChild(0).gameObject;
    //            particleGO.transform.parent = wordboxImages[indexInTiles].gameObject.transform;
    //            particleGO.transform.localPosition = Vector3.zero;
    //        }
    //    }

    //    int lastIndex = playerWB.GetCurrentWordLength() - 1 - wordbarScroll;
    //    wordboxTMPs[lastIndex].text = "";
    //    wordboxImages[lastIndex].sprite = blankTileDefault.sprite;
    //    wordboxImages[lastIndex].color = Color.white;
    //    if (wordboxImages[lastIndex].gameObject.transform.childCount > 0)
    //    {
    //        Destroy(wordboxImages[lastIndex].gameObject.transform.GetChild(0).gameObject);
    //    }

    //    playerWB.DestroySpecificLetterFromCurrentWord(indexInTiles + wordbarScroll);
    //    if (wordbarScroll > 0)
    //    {
    //        wordbarScroll--;
    //    }
    //    playerWB.RebuildCurrentWordForUI();
    //}

    #endregion
    

    //public void RemoveParticleEffectsAtIndexInWord(int indexInWord)
    //{
    //    GameObject go = wordboxImages[indexInWord - wordbarScroll].gameObject;
    //    int children = go.transform.childCount;
    //    for (int i = 0; i < children; i++)
    //    {
    //        Destroy(go.gameObject.transform.GetChild(i).gameObject);
    //    }
    //}

    //public void ModifyPowerMeterTMP(int valueToShow)
    //{
    //    //powerMeterTMP.text = valueToShow.ToString();
    //    UpdateSpellSwordGlow(valueToShow);
    //}



    //public void ShowHideMainMenu(bool shouldBeShown)
    //{
    //    mainMenu.gameObject.SetActive(shouldBeShown);
    //}

    //public void HideAllOverworldUIElements()
    //{
    //    //ShowHideTutorialPanel(false);
    //    ShowHideBottomPanel(false);
    //    ShowHideTopPanel(false);
    //    ShowHideVictoryMeter(false);
    //    ShowHideLetterPowerButton(false);
    //    ShowHideOptionMenuButton(false);
    //    ShowHideOptionsMenu(false);
    //    ShowHideLetterPowersMenu(false);
    //}
    //public void ShowOverworldUIElements()
    //{
    //    ShowHideBottomPanel(false);
    //    ShowHideTopPanel(false);
    //    ShowHideVictoryMeter(false);
    //    ShowHideLetterPowerButton(false);
    //    ShowHideOptionMenuButton(true);
    //}

    //public void ShowHideDebugMenu(bool shouldBeShown)
    //{
    //    debugMenuPanel.SetActive(shouldBeShown);
    //}

    private void ModifyAttackButtonWithWordValidationUponNewSwordWord(bool isValid)
    {
        if (isValid)
        {
            canPressAttackButton = true;
            attackButtonMain.color = new Color(.37f, .70f, .34f);
            attackButtonRunes.color = new Color(.09f, .56f, 0.0f, 1);
        }
        else
        {
            canPressAttackButton = false;
            attackButtonMain.color = new Color(.70f, .70f, .70f);
            attackButtonRunes.color = new Color(.09f, .56f, 0.0f, 0);
        }
    }

    //private void ShowHideLetterPowerButton(bool shouldBeShown)
    //{
    //    letterPowersMenuButton.gameObject.SetActive(shouldBeShown);
    //}

    //private void ShowHideOptionMenuButton(bool shouldBeShown)
    //{
    //    optionsMenuButton.gameObject.SetActive(shouldBeShown);
    //}

    private void ResetWordTilesToMax()
    {
        foreach (var image in wordboxImages)
        {
            image.gameObject.SetActive(true);
        }
        foreach (var tmp in wordboxTMPs)
        {
            tmp.gameObject.SetActive(true);
        }
    }

    public void ShowHideTopPanel(bool shouldBeShown)
    {
        //StartCoroutine(ShowHideTopPanel_Coroutine(shouldBeShown));
        foreach (var element in topPanelElements)
        {
            element.SetActive(shouldBeShown);
        }

    }

    //private void ShowHideBottomPanel(bool shouldBeShown)
    //{
    //    bottomBarPanel.SetActive(shouldBeShown);
    //}

    private void ShowHideVictoryMeter(bool shouldBeShown)
    {
        victoryBarSlider.gameObject.SetActive(shouldBeShown);
    }

    //public void ShowHideTopBelt(bool shouldBeShown)
    //{
    //    topBeltPanel.SetActive(shouldBeShown);
    //    if (!bagman)
    //    {
    //        bagman = FindObjectOfType<BagManager>();
    //    }
    //    if (shouldBeShown)
    //    {
    //        bagman.ModifyBagsEnabled(2);
    //    }
    //    else
    //    {
    //        bagman.ModifyBagsEnabled(0);
    //    }
    //}

    //public void ShowHideBottomBelt(bool shouldBeShown)
    //{
    //    bottomBeltPanel.SetActive(shouldBeShown);
    //    if (!bagman)
    //    {
    //        bagman = FindObjectOfType<BagManager>();
    //    }
    //    if (shouldBeShown)
    //    {
    //        bagman.ModifyBagsEnabled(4);
    //    }
    //    else
    //    {
    //        bagman.ModifyBagsEnabled(2);
    //    }
    //}
    //public void ShowHideOptionsMenu(bool shouldBeShown)
    //{
    //    if (!gc)
    //    {
    //        gc = FindObjectOfType<GameController>();
    //    }
    //    gc.PauseGame();

    //    optionsMenu.SetActive(shouldBeShown);

    //}

    //public void ShowHideLetterPowersMenu(bool shouldBeShown)
    //{
    //    if (!gc)
    //    {
    //        gc = FindObjectOfType<GameController>();
    //    }
    //    gc.PauseGame();
    //    letterPowersMenu.SetActive(shouldBeShown);

    //}

    //public TextMeshProUGUI GetTutorialTMP()
    //{
    //    return tutorialTMP;
    //}

    //public void ShowHideTutorialPanel(bool isTutorialSupposedToBeVisible)
    //{
    //    tutorialPanel.SetActive(isTutorialSupposedToBeVisible);
    //}

    public void SetTutorRef(Tutor tutorRef)
    {
        tutor = tutorRef;
    }

    public void UpdateLettersOnSwordAndPower(WordBuilder.SwordWordPower newSwordWord)
    {
        powerMeterTMP.text = newSwordWord.Power.ToString();
        sgd.UpdateTargetSpellswordGlow(newSwordWord.Power);

        bool isNewWordValid = wv.CheckWordValidity(newSwordWord.word);
        ModifyAttackButtonWithWordValidationUponNewSwordWord(isNewWordValid);
        for (int i = 0; i < wordboxTMPs.Length; i++)
        {
            if (i < newSwordWord.letterSprites.Length)
            {
                wordboxTMPs[i].text = newSwordWord.letterLetters[i];
                wordboxImages[i].sprite = newSwordWord.letterSprites[i];
                wordboxImages[i].color = newSwordWord.letterColors[i];
            }
            else
            {
                wordboxTMPs[i].text = "";
                wordboxImages[i].sprite = blankTileDefault.sprite;
                wordboxImages[i].color = blankTileDefault.color;
            }
        }
    }

    public void AddLetterToWordBar(LetterTile letterTile, char letter, int indexInWord)
    {
        SpriteRenderer sr = letterTile.GetComponent<SpriteRenderer>();
        LetterTile.SpriteColorYMod sc = letterTile.GetSpriteColorFromAbility(letterTile.Ability_Player);
        wordboxImages[indexInWord].sprite = sc.Sprite;
        wordboxImages[indexInWord].color = sc.Color;
        wordboxTMPs[indexInWord].text = letter.ToString();
        //wordboxTMPs[indexInWord - wordbarScroll].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, sc.YMod);
    }   

    /// <summary>
    /// This resets the entire wordbar, erasing all chars and particle effects, and resets the 
    /// coin image back to the default.
    /// </summary>
    public void ClearWordBar()
    {
        foreach(var image in wordboxImages)
        {
            image.sprite = blankTileDefault.sprite;
            image.color = blankTileDefault.color;
            ModifyAttackButtonWithWordValidationUponNewSwordWord(false);
            powerMeterTMP.text = 0.ToString();
            //if (image.gameObject.transform.childCount > 0)
            //{
            //    Destroy(image.gameObject.transform.GetChild(0).gameObject);
            //}
        }
        foreach(var TMP in wordboxTMPs)
        {
            TMP.text = "";
        }
        sgd.UpdateTargetSpellswordGlow(0);
    }

    public void ShowHideIgnitionChancePanel(bool shouldBeShown)
    {
        ignitionChancePanel.SetActive(shouldBeShown);
    }

    public void UpdateIgnitionChanceTMP(float value)
    {
        ignitionChanceTMP.text = (value).ToString() + "%";
    }
    //public GameObject GetTileForLetterBasedOnIndexInWord(int indexInWord)
    //{
    //    return wordboxImages[indexInWord].gameObject;
    //}

    //public void UpdateSpellEnergySlider( float currentEnergy)
    //{
    //    float factor = currentEnergy / 100f;
    //    if (factor >= 1f)
    //    {
    //        spellEnergySlider_0.value = spellEnergySlider_0.maxValue;
    //        energySliderFill_0.color = fullBar;
    //        spellEnergySlider_1.value = spellEnergySlider_1.maxValue;
    //        energySliderFill_1.color = fullBar;
    //        spellEnergySlider_2.value = spellEnergySlider_2.maxValue;
    //        energySliderFill_2.color = fullBar;
    //        return;
    //    }
    //    if (factor >= 0.66f)
    //    {
    //        spellEnergySlider_0.value = spellEnergySlider_0.maxValue;
    //        energySliderFill_0.color = fullBar;
    //        spellEnergySlider_1.value = spellEnergySlider_1.maxValue;
    //        energySliderFill_1.color = fullBar;
    //        spellEnergySlider_2.value = factor - .66f;
    //        energySliderFill_2.color = partialBar;
    //        return;
    //    }
    //    if (factor >= 0.33f)
    //    {
    //        spellEnergySlider_0.value = spellEnergySlider_0.maxValue;
    //        energySliderFill_0.color = fullBar;
    //        spellEnergySlider_1.value = factor - .33f;
    //        energySliderFill_1.color = partialBar;
    //        spellEnergySlider_2.value = 0;
    //        return;
    //    }
    //    if (factor < 0.33f)
    //    {
    //        spellEnergySlider_0.value = factor;
    //        energySliderFill_0.color = partialBar;
    //        spellEnergySlider_1.value = 0;
    //        spellEnergySlider_2.value = 0;
    //        return;
    //    }

    //}

    public void HideLetterTilesOverMaxLetterLimit(int maxLetters)
    {
        maxLetters = Mathf.Clamp(maxLetters, 2, wordboxImages.Length);
        for (int i = maxLetters; i < wordboxImages.Length; i++)
        {
            wordboxImages[i].gameObject.SetActive(false);
            wordboxTMPs[i].gameObject.SetActive(false);
        }
    }


    //IEnumerator ShowHideTopPanel_Coroutine(bool shouldBeShown)
    //{
    //    float value = topBarPanel.anchoredPosition.y;
    //    Debug.Log($"value: {value}");
    //    if (shouldBeShown)
    //    {
    //        while (topBarPanel.anchoredPosition.y > topPanelShown_Y)
    //        {
    //            Debug.Log($"value: {value}, target is {topPanelShown_Y}");
    //            value = Mathf.MoveTowards(value, topPanelShown_Y, panelDeployRate * Time.unscaledDeltaTime);
    //            topBarPanel.position = new Vector2(0, value);
    //            yield return new WaitForEndOfFrame();
    //        }
    //    }
    //    else
    //    {
    //        while (topBarPanel.anchoredPosition.y < topPanelHidden_Y)
    //        {
    //            Debug.Log($"value: {value}, target is {topPanelHidden_Y}");
    //            value = Mathf.MoveTowards(value, topPanelHidden_Y, panelDeployRate * Time.unscaledDeltaTime);
    //            topBarPanel.position = new Vector2(0, value);
    //            yield return new WaitForEndOfFrame();
    //        }
    //    }
    //}

}
