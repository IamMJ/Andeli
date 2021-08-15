using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


public class WordBuilder : MonoBehaviour
{
    [SerializeField] Slider wordEraseSliderBG = null;
    [SerializeField] Slider wordFiringSliderBG = null;
    DebugHelper dh;
    [SerializeField] PlayerInput pi;
    [SerializeField]  TailPieceManager playerTPM;
    [SerializeField] SpellMaker sm;
    [SerializeField] PowerMeter pm;


    //state

    List<LetterTile> lettersCollected = new List<LetterTile>();
    public bool HasLetters { get; private set; } = false;
    string currentWord;
    int currentWordLength;
    bool isFireWeaponButtonPressed = false;
    bool isEraseWeaponButtonPressed = false;
    float timeButtonDepressed = 0;
    float longPressTime;


    private void Start()
    {
        pi = FindObjectOfType<PlayerInput>();
        longPressTime = pi.LongPressTime;
        playerTPM = pi.GetComponent<TailPieceManager>();
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
        if (!pi)
        {
            Start();
        }
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

    public void AddLetter(LetterTile newLetter)
    {
        if (!playerTPM)
        {
            pi = FindObjectOfType<PlayerInput>();
            playerTPM = pi.GetComponent<TailPieceManager>();
        }
        lettersCollected.Add(newLetter);
        currentWord += newLetter.Letter;
        currentWordLength = currentWord.Length;
        HasLetters = true;
        playerTPM.AddNewTailPiece(newLetter.Letter);
        TestAllLetterLatentAbilities();
    }

    private void TestAllLetterLatentAbilities()
    {
        for (int i =0; i < lettersCollected.Count; i++)
        {
            TestLetterLatentAbility(lettersCollected[i], i);
        }
    }

    private void TestLetterLatentAbility(LetterTile newLetter, int index)
    {
        int power = newLetter.Power;
        int roll = 21 - UnityEngine.Random.Range(1, 21);
        //Debug.Log($"roll: {roll} vs wordLength: {currentWordLength}.");
        switch (newLetter.Ability)
        {
            case TrueLetter.Ability.Nothing:
                //
                break;

            case TrueLetter.Ability.Shiny:
                if (currentWordLength >= roll)
                {
                    power *= 2;
                    playerTPM.AddFXToSelectedTailPiece(newLetter.Ability, index);
                }
                break ;

            case TrueLetter.Ability.Frozen:
                //
                break;

            case TrueLetter.Ability.Fiery:
                //
                break;

        }

        pm.IncreasePower(power);
    }



    public string GetCurrentWord()
    {
        return currentWord;
    }

    public void ClearOutWordBox()
    {
        currentWord = "";
        HasLetters = false;
        currentWordLength = 0;
        lettersCollected.Clear();
        playerTPM.DestroyEntireTail();
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
