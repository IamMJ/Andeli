﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class UIDriver : MonoBehaviour
{
    //init
    [SerializeField] GameObject pauseMenuPrefab = null;

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

    Color fullBar = new Color(1, 1, 0);
    Color partialBar = new Color(.7169f, .7169f, .3551f);

    WordBuilder playerWB;
    WordWeaponizer playerWWZ;
    GameController gc;
    SceneLoader sl;
    GameObject pauseMenu;

    //param
    float panelDeployRate = 100f; // pixels per second
    float topPanelShown_Y = -77f;
    float topPanelHidden_Y = 100f;

    //state
    bool isFireWeaponButtonPressed = false;
    bool isEraseWeaponButtonPressed = false;
    float timeButtonDepressed = 0;
    public float timeSpentLongPressing { get; private set; }

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

    public void ClearOutSpecificLetterFromWordStub(int index)
    {
        RemoveParticleEffectsAtLetter(index);

        for (int i = index; i < playerWB.GetCurrentWordLength()-1; i++)
        {
            wordboxTMPs[index].text = wordboxTMPs[index + 1].text;
            wordboxImages[index].sprite = wordboxImages[index + 1].sprite;
            if (wordboxImages[index + 1].gameObject.transform.childCount > 0)
            {
                GameObject particleGO = wordboxImages[index + 1].gameObject.transform.GetChild(0).gameObject;
                particleGO.transform.parent = wordboxTMPs[index].gameObject.transform;
            }
        }

        int lastIndex = playerWB.GetCurrentWordLength() - 1;
        wordboxTMPs[lastIndex].text = "";
        wordboxImages[lastIndex].sprite = blankTileDefault;
        if (wordboxImages[lastIndex].gameObject.transform.childCount > 0)
        {
            Destroy(wordboxImages[lastIndex].gameObject.transform.GetChild(0).gameObject);
        }

        playerWB.RemoveSpecificLetterFromCurrentWord(index);
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

    public void RemoveParticleEffectsAtLetter(int index)
    {
        GameObject go = GetGameObjectAt(index);
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

    public void AddLetterToWordBar(Sprite letterTileSprite, char letter, int indexInWord)
    {
        wordboxImages[indexInWord].sprite = letterTileSprite;
        wordboxTMPs[indexInWord].text = letter.ToString();
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
            if (image.gameObject.transform.childCount > 0)
            {
                Destroy(image.gameObject.transform.GetChild(0).gameObject);
            }
        }
        foreach(var TMP in wordboxTMPs)
        {
            TMP.text = "";
        }
    }

    public GameObject GetGameObjectAt(int index)
    {
        return wordboxImages[index].gameObject;
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
