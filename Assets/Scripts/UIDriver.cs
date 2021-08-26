﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class UIDriver : MonoBehaviour
{
    //init
    [SerializeField] Slider victoryBarSlider = null;
    [SerializeField] GameObject topBarPanel = null;
    [SerializeField] GameObject bottomBarPanel = null;

    [SerializeField] Slider wordEraseSliderBG = null;
    [SerializeField] Slider wordFiringSliderBG = null;
    [SerializeField] TextMeshProUGUI powerMeterTMP = null;
    WordBuilder playerWB;

    //param
    float panelDeployRate = 100f; // pixels per second
    float topPanelShown_Y = -77f;
    float topPanelHidden_Y = 100f;

    //state
    bool isFireWeaponButtonPressed = false;
    bool isEraseWeaponButtonPressed = false;
    float timeButtonDepressed = 0;
    public float timeSpentLongPressing { get; private set; }


    // Update is called once per frame
    void Update()
    {
        if (playerWB)
        {
            HandleFireWordButtonPressed();
            HandleEraseWordButtonPressed();
        }
    }

    public void SetPlayerObject(WordBuilder newPlayerWB)
    {
        playerWB = newPlayerWB;
    }

    #region Initial Button Press Handlers
    public void OnPressDownFireWord()
    {
        if (!playerWB.HasLetters) { return; }

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
        if (!playerWB.HasLetters) { return; }

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
                playerWB.FireCurrentWord();
                CompleteLongPress_WordBoxActions();
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