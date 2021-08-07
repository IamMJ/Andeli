using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


public class WordBuilder : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI wordBoxTMP = null;
    [SerializeField] Slider wordEraseSliderBG = null;
    [SerializeField] Slider wordFiringSliderBG = null;
    DebugHelper dh;
    PlayerInput pi;
    SpellMaker sm;
    PowerMeter pm;

    //state

    public bool HasLetters { get; private set; } = false;
    string currentWord;
    bool isFireWeaponButtonPressed = false;
    bool isEraseWeaponButtonPressed = false;
    float timeButtonDepressed = 0;
    float longPressTime;

    private void Start()
    {
        pi = FindObjectOfType<PlayerInput>();
        longPressTime = pi.LongPressTime;
        dh = FindObjectOfType<DebugHelper>();
        sm = FindObjectOfType<SpellMaker>();
        pm = FindObjectOfType<PowerMeter>();
    }

    #region Initial Button Press Handlers
    public void OnPressDownFireWord()
    {
        if (!pi)
        {
            Start();
        }
        if (!HasLetters) { return; }

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
        if (!pi)
        {
            Start();
        }
        if (!HasLetters) { return; }

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

    private void Update()
    {
        HandleFireWordButtonPressed();
        HandleEraseWordButtonPressed();

    }

    private void HandleEraseWordButtonPressed()
    {
        if (isEraseWeaponButtonPressed)
        {
            timeButtonDepressed += Time.unscaledDeltaTime;
            Time.timeScale = 0;
            FillWordEraseSlider(timeButtonDepressed / longPressTime);
            if (timeButtonDepressed >= longPressTime)
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
            FillWordFiringSlider(timeButtonDepressed / longPressTime);
            if (timeButtonDepressed >= longPressTime)
            {
                sm.FireCurrentWord();
                CompleteLongPress_WordBoxActions();
            }
        }
        
    }

    public void AddLetter(char newLetter)
    {
        //Debug.Log("adding: " + newLetter);
        currentWord += newLetter;
        //Debug.Log("Current word: " + currentWord);
        wordBoxTMP.text = currentWord;
        HasLetters = true;
    }

    public string GetCurrentWord()
    {
        return currentWord;
    }

    public void ClearOutWordBox()
    {
        currentWord = "";
        wordBoxTMP.text = currentWord;
        HasLetters = false;
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
        ClearOutWordBox();
        pm.ClearPowerLevel();
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
}
