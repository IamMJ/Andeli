using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
[RequireComponent (typeof (SpellMaker), typeof(TailPieceManager), typeof(LetterCollector))]


public class WordBuilder : MonoBehaviour
{
    DebugHelper dh;
    PlayerInput pi;
    TailPieceManager tpm;
    SpellMaker sm;
    PowerMeter pm;
    UIDriver uid;
    WordValidater wv;
    ArenaBuilder ab;
    ArenaLetterEffectsHandler aleh;

    //state

    List<LetterTile> lettersCollected = new List<LetterTile>();
    public bool HasLetters { get; private set; } = false;
    string currentWord;
    int currentWordLength;
    public int CurrentPower { get; private set; } = 0;


    private void Start()
    {
        dh = FindObjectOfType<DebugHelper>();
        uid = FindObjectOfType<UIDriver>();
        wv = FindObjectOfType<WordValidater>();
        uid.SetPlayerObject(this);
        pi = GetComponent<PlayerInput>();
        tpm = GetComponent<TailPieceManager>();
        sm = GetComponent<SpellMaker>();
    }

    public void AddLetter(LetterTile newLetter)
    {
        if (!tpm)
        {
            pi = FindObjectOfType<PlayerInput>();
            tpm = pi.GetComponent<TailPieceManager>();
        }
        lettersCollected.Add(newLetter);
        currentWord += newLetter.Letter;
        currentWordLength = currentWord.Length;
        IncreasePower(newLetter.Power);
        HasLetters = true;
        tpm.AddNewTailPiece(newLetter.Letter);
        TestAllLetterLatentAbilities();
    }

    private void TestAllLetterLatentAbilities()
    {
        for (int i =0; i < lettersCollected.Count; i++)
        {
            if (lettersCollected[i].GetLatentAbilityStatus() == false)
            {
                TestLetterLatentAbility(lettersCollected[i], i);
            }
        }
    }

    private void TestLetterLatentAbility(LetterTile newLetter, int index)
    {
        int roll = 21 - UnityEngine.Random.Range(1, 21);
        if (currentWordLength < roll)
        {
            return;
        }

        newLetter.SetLatentAbilityStatus(true);

        if (!aleh)
        {
            ab = FindObjectOfType<ArenaBuilder>();
            aleh = ab.GetComponent<ArenaLetterEffectsHandler>();
        }
        aleh.ApplyLetterEffectOnPickup(newLetter, gameObject, index);
    }


    public string GetCurrentWord()
    {
        return currentWord;
    }
    public void ClearCurrentWord()
    {
        currentWord = "";
        HasLetters = false;
        currentWordLength = 0;
        lettersCollected.Clear();
        tpm.DestroyEntireTail();
    }

    public void FireCurrentWord()
    {
        if (sm.FireCurrentWordIfValid())
        {
            foreach (var letter in lettersCollected)
            {
                aleh.ApplyLetterEffectOnFiring(letter, gameObject);
            }
        }

    }

    public void IncreasePower(int amount)
    {
        CurrentPower += amount;
        uid.ModifyPowerMeterTMP(CurrentPower);
    }

    public void ClearPowerLevel()
    {
        CurrentPower = 0;
        uid.ModifyPowerMeterTMP(CurrentPower);
    }

    public int GetCurrentWordLength()
    {
        return currentWordLength;
    }




}
