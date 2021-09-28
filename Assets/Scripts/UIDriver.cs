using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class UIDriver : MonoBehaviour
{
    //init
    [SerializeField] GameObject pauseMenuPrefab = null;
    [SerializeField] GameObject letterPowersMenuPrefab = null;

    [SerializeField] Slider victoryBarSlider = null;
    [SerializeField] GameObject topBarPanel = null;
    [SerializeField] GameObject bottomBarPanel = null;

    [SerializeField] Slider wordEraseSliderBG = null;
    [SerializeField] Slider wordFiringSliderBG = null;
    [SerializeField] TextMeshProUGUI powerMeterTMP = null;

    [SerializeField] Sprite blankTileDefault = null;
    [SerializeField] Image[] wordboxImages = null;
    [SerializeField] TextMeshProUGUI[] wordboxTMPs = null;

    [SerializeField] Slider spellEnergySlider_0 = null;
    [SerializeField] Slider spellEnergySlider_1 = null;
    [SerializeField] Slider spellEnergySlider_2 = null;

    [SerializeField] Image energySliderFill_0 = null;
    [SerializeField] Image energySliderFill_1 = null;
    [SerializeField] Image energySliderFill_2 = null;

    [SerializeField] TextMeshProUGUI tutorialTMP = null;

    Color fullBar = new Color(1, 1, 0);
    Color partialBar = new Color(.7169f, .7169f, .3551f);

    WordBuilder playerWB;
    WordWeaponizer playerWWZ;
    GameController gc;
    SceneLoader sl;
    GameObject pauseMenu;
    GameObject letterPowersMenu;
    Tutor tutor;

    //param
    float panelDeployRate = 100f; // pixels per second
    float topPanelShown_Y = -77f;
    float topPanelHidden_Y = 100f;

    //state
    bool isFireWeaponButtonPressed = false;
    bool isEraseWeaponButtonPressed = false;
    float timeButtonDepressed = 0;
    public float timeSpentLongPressing { get; private set; }
    [SerializeField] int wordbarScroll = 0;


    private void Start()
    {
        ClearWordBar();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerWB)
        {
            HandleFireWordButtonPressed();
            HandleEraseWordButtonPressed();
        }
    }

    public void SetPlayerObject(WordBuilder newPlayerWB, WordWeaponizer newPlayerWWZ)
    {
        playerWB = newPlayerWB;
        playerWWZ = newPlayerWWZ;
    }

    #region Initial Button Press Handlers
    public void OnPressDownFireWord()
    {
        if (playerWB.GetCurrentWordLength() == 0) { return; }


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
        if (playerWB.GetCurrentWordLength() == 0) { return; }

        if (isFireWeaponButtonPressed == false)
        {
            isEraseWeaponButtonPressed = true;
        }
    }

    public void OnReleaseEraseWord()
    {
        IncompleteLongPress_WordBoxActions();
    }

    public void PressTutorialOkayButton()
    {
        tutor.AdvanceToNextStep();
    }
    #endregion

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
            FillWordFiringSlider(timeButtonDepressed / UIParameters.LongPressTime);
            if (timeButtonDepressed >= UIParameters.LongPressTime)
            {
                if (playerWWZ.AttemptToFireWordAsPlayer())
                {
                    CompleteLongPress_WordBoxActions();
                }                
            }
        }
    }

    public void FillWordEraseSlider(float amount)
    {
        wordEraseSliderBG.value = amount;
    }

    public void ClearWordEraseSlider()
    {
        wordEraseSliderBG.value = 0f;
    }

    public void FillWordFiringSlider(float amount)
    {
        wordFiringSliderBG.value = amount;
    }

    public void ClearWordFiringSlider()
    {
        wordFiringSliderBG.value = 0;
    }

    public void HandlePressOnLetterInWordBar(int index)
    {
        // if the index is first or last tile spot, check if the word is longer than tiles.
        // if longer than tiles, then the word must be overflowing, and should scroll left or right.
        

        ClearOutLetterFromSpecificTile(index);
    }

    private void ClearOutLetterFromSpecificTile(int indexInTiles)
    {
        RemoveParticleEffectsAtIndexInWord(indexInTiles + wordbarScroll);

        for (int i = indexInTiles; i < playerWB.GetCurrentWordLength()-1; i++)
        {
            wordboxTMPs[indexInTiles].text = wordboxTMPs[indexInTiles + 1].text;
            wordboxImages[indexInTiles].sprite = wordboxImages[indexInTiles + 1].sprite;
            if (wordboxImages[indexInTiles + 1].gameObject.transform.childCount > 0)
            {
                GameObject particleGO = wordboxImages[indexInTiles + 1].gameObject.transform.GetChild(0).gameObject;
                particleGO.transform.parent = wordboxImages[indexInTiles].gameObject.transform;
                particleGO.transform.localPosition = Vector3.zero;
            }
        }

        int lastIndex = playerWB.GetCurrentWordLength() - 1 - wordbarScroll;
        wordboxTMPs[lastIndex].text = "";
        wordboxImages[lastIndex].sprite = blankTileDefault;
        wordboxImages[lastIndex].color = Color.white;
        if (wordboxImages[lastIndex].gameObject.transform.childCount > 0)
        {
            Destroy(wordboxImages[lastIndex].gameObject.transform.GetChild(0).gameObject);
        }

        playerWB.RemoveSpecificLetterFromCurrentWord(indexInTiles + wordbarScroll);
        if (wordbarScroll > 0)
        {
            wordbarScroll--;
        }
        playerWB.RebuildCurrentWordForUI();
    }

    private void CompleteLongPress_WordBoxActions()
    {
        playerWB.ClearCurrentWord();
        playerWB.ClearPowerLevel();
        IncompleteLongPress_WordBoxActions();
    }
    private void IncompleteLongPress_WordBoxActions()
    {
        ClearWordEraseSlider();
        ClearWordFiringSlider();
        timeButtonDepressed = 0;
        Time.timeScale = 1f;
        isFireWeaponButtonPressed = false;
        isEraseWeaponButtonPressed = false;

    }

    public void RemoveParticleEffectsAtIndexInWord(int indexInWord)
    {
        GameObject go = wordboxImages[indexInWord - wordbarScroll].gameObject;
        int children = go.transform.childCount;
        for (int i = 0; i < children; i++)
        {
            Destroy(go.gameObject.transform.GetChild(i).gameObject);
        }
    }

    public void ModifyPowerMeterTMP(int valueToShow)
    {
        powerMeterTMP.text = valueToShow.ToString();
    }

    public void EnterOverworld()
    {
        ShowHideBottomPanel(false);
        ShowHideTopPanel(false);
        ShowHideVictoryMeter(false);
    }

    public void EnterArena()
    {
        ShowHideBottomPanel(true);
        ShowHideTopPanel(true);
        ShowHideVictoryMeter(true);
    }


    private void ShowHideTopPanel(bool shouldBeShown)
    {
        //StartCoroutine(ShowHideTopPanel_Coroutine(shouldBeShown));
        topBarPanel.SetActive(shouldBeShown);
    }

    private void ShowHideBottomPanel(bool shouldBeShown)
    {
        bottomBarPanel.SetActive(shouldBeShown);
    }

    private void ShowHideVictoryMeter(bool shouldBeShown)
    {
        victoryBarSlider.gameObject.SetActive(shouldBeShown);
    }

    public void ShowPauseMenu()
    {
        if (!gc)
        {
            gc = FindObjectOfType<GameController>();
        }
        gc.PauseGame();
        if (!pauseMenu)
        {
            pauseMenu = Instantiate(pauseMenuPrefab);
        }
        if (pauseMenu)
        {
            pauseMenu.SetActive(true);
        }
    }

    public void ShowLetterPowersMenu()
    {
        if (!gc)
        {
            gc = FindObjectOfType<GameController>();
        }
        gc.PauseGame();
        if (!letterPowersMenu)
        {
            letterPowersMenu = Instantiate(letterPowersMenuPrefab);
        }
        if (letterPowersMenu)
        {
            letterPowersMenu.SetActive(true);
        }
    }

    public TextMeshProUGUI GetTutorialTMP()
    {
        return tutorialTMP;
    }

    public void SetTutorRef(Tutor tutorRef)
    {
        tutor = tutorRef;
    }

    public void AddLetterToWordBar(LetterTile letterTile, char letter, int indexInWord)
    {
        if (indexInWord >= wordboxImages.Length)
        {
            ScrollLettersLeft();
        }
        SpriteRenderer sr = letterTile.GetComponent<SpriteRenderer>();
        wordboxImages[indexInWord - wordbarScroll].sprite = sr.sprite;
        wordboxImages[indexInWord - wordbarScroll].color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
        wordboxTMPs[indexInWord - wordbarScroll].text = letter.ToString();
    }

    private void ScrollLettersLeft()
    {
        wordbarScroll++;
        wordboxTMPs[0].text = "...";
        wordboxImages[0].sprite = blankTileDefault;
        wordboxImages[0].color = Color.white;
        RemoveParticleEffectsAtIndexInWord(0 + wordbarScroll);

        for (int i = 1; i < wordboxImages.Length-1; i++)
        {
            wordboxTMPs[i].text = wordboxTMPs[i+1].text;
            wordboxImages[i].sprite = wordboxImages[i+1].sprite;
            wordboxImages[i].color = wordboxImages[i+1].color;
            if (wordboxImages[i + 1].gameObject.transform.childCount > 0)
            {
                GameObject particleGO = wordboxImages[i + 1].gameObject.transform.GetChild(0).gameObject;
                particleGO.transform.parent = wordboxImages[i].gameObject.transform;
                particleGO.transform.localPosition = Vector3.zero;
            }

        }

    }

    /// <summary>
    /// This resets the entire wordbar, erasing all chars and particle effects, and resets the 
    /// coin image back to the default.
    /// </summary>
    public void ClearWordBar()
    {
        foreach(var image in wordboxImages)
        {
            image.sprite = blankTileDefault;
            image.color = Color.white;
            if (image.gameObject.transform.childCount > 0)
            {
                Destroy(image.gameObject.transform.GetChild(0).gameObject);
            }
        }
        foreach(var TMP in wordboxTMPs)
        {
            TMP.text = "";
        }
        wordbarScroll = 0;
    }

    public GameObject GetTileForLetterBasedOnIndexInWord(int indexInWord)
    {
        return wordboxImages[indexInWord - wordbarScroll].gameObject;
    }

    public void UpdateSpellEnergySlider( float currentEnergy)
    {
        float factor = currentEnergy / 100f;
        if (factor >= 1f)
        {
            spellEnergySlider_0.value = spellEnergySlider_0.maxValue;
            energySliderFill_0.color = fullBar;
            spellEnergySlider_1.value = spellEnergySlider_1.maxValue;
            energySliderFill_1.color = fullBar;
            spellEnergySlider_2.value = spellEnergySlider_2.maxValue;
            energySliderFill_2.color = fullBar;
            return;
        }
        if (factor >= 0.66f)
        {
            spellEnergySlider_0.value = spellEnergySlider_0.maxValue;
            energySliderFill_0.color = fullBar;
            spellEnergySlider_1.value = spellEnergySlider_1.maxValue;
            energySliderFill_1.color = fullBar;
            spellEnergySlider_2.value = factor - .66f;
            energySliderFill_2.color = partialBar;
            return;
        }
        if (factor >= 0.33f)
        {
            spellEnergySlider_0.value = spellEnergySlider_0.maxValue;
            energySliderFill_0.color = fullBar;
            spellEnergySlider_1.value = factor - .33f;
            energySliderFill_1.color = partialBar;
            spellEnergySlider_2.value = 0;
            return;
        }
        if (factor < 0.33f)
        {
            spellEnergySlider_0.value = factor;
            energySliderFill_0.color = partialBar;
            spellEnergySlider_1.value = 0;
            spellEnergySlider_2.value = 0;
            return;
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
