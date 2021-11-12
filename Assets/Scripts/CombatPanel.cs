using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class CombatPanel : UI_Panel
{
    //init

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


    [SerializeField] WordBuilder playerWB;
    WordWeaponizer playerWWZ;
    GameController gc;

    BagManager bagman;
    SwordGlowDriver sgd;
    IgnitionChanceDriver igd;
    WordPack emptyWordPack = new WordPack(0, 0, "init", 0, false);

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
        sgd = GetComponent<SwordGlowDriver>();
        igd = GetComponent<IgnitionChanceDriver>();
        bagman = Librarian.GetLibrarian().bagManager;

        UpdatePanelWithNewWordPack(emptyWordPack);
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

    #region Public Methods

    public void UpdatePanelWithNewWordPack(WordPack newWordPack)
    {
        powerMeterTMP.text = newWordPack.Power.ToString();
        ModifyAttackButtonWithWordValidation(newWordPack.IsValid);
        sgd.UpdateTargetSpellswordGlow(newWordPack.Power);
        igd.SetIgnitionChance(newWordPack.ModifiedWordLength);

        for (int i = 0; i < wordboxTMPs.Length; i++)
        {
            if (i < newWordPack.letterSprites.Length)
            {
                wordboxTMPs[i].text = newWordPack.letterLetters[i];
                wordboxImages[i].sprite = newWordPack.letterSprites[i];
                wordboxImages[i].color = newWordPack.letterColors[i];
            }
            else
            {
                wordboxTMPs[i].text = "";
                wordboxImages[i].sprite = blankTileDefault.sprite;
                wordboxImages[i].color = blankTileDefault.color;
            }
        }
    }

    public void HideLetterTilesOverMaxLetterLimit(int maxLetters)
    {
        maxLetters = Mathf.Clamp(maxLetters, 2, wordboxImages.Length);
        for (int i = maxLetters; i < wordboxImages.Length; i++)
        {
            wordboxImages[i].gameObject.SetActive(false);
            wordboxTMPs[i].gameObject.SetActive(false);
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

    public void ShowHideIgnitionChancePanel(bool shouldBeShown)
    {
        igd.ShowHideElements(shouldBeShown);
    }

    #endregion

    #region Initial Button Press Handlers
    public void OnPressDownFireWord()
    {
        //if (playerWB.GetCurrentWord().Length == 0) { return; }
        if (!canPressAttackButton) { return; }

        if (isEraseWeaponButtonPressed == false)
        {
            isFireWeaponButtonPressed = true;
        }
        Debug.Log("Fire word pressed");

    }

    public void OnReleaseFireWord()
    {
        Debug.Log("Fire word released");
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
    #endregion

    #region Button Helpers

    private void ModifyAttackButtonWithWordValidation(bool isValid)
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
        playerWB.ClearLettersOnSword();
        //playerWB.ClearPowerLevel();
        IncompleteLongPress_WordBoxActions();
    }
    private void IncompleteLongPress_WordBoxActions()
    {
        ClearWordEraseSlider();
        UpdateAttackButtonPressGlow(0);
        timeButtonDepressed = 0;
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

    #region Bag Helpers

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

    #endregion

}
