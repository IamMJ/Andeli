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

    //param
    int maxWordLength = 10;

    //state

    List<LetterTile> lettersCollected = new List<LetterTile>();
    public bool HasLetters { get; private set; } = false;
    string currentWord;
    int currentWordLength;
    int wordLengthBonus = 0;
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
        if (currentWordLength >= maxWordLength) { return; }
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
        //tpm.AddNewTailPiece(newLetter.Letter);
        Sprite newSprite = newLetter.GetComponent<SpriteRenderer>().sprite;
        uid.AddLetterToWordBar(newSprite, newLetter.Letter, currentWordLength - 1);
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
        if (currentWordLength + wordLengthBonus < roll)
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
    public int GetCurrentWordLength()
    {
        return currentWordLength;
    }

    public void ClearCurrentWord()
    {
        currentWord = "";
        HasLetters = false;
        currentWordLength = 0;
        ResetWordLengthBonus();
        lettersCollected.Clear();
        uid.ClearWordBar();
        //tpm.DestroyEntireTail();
    }

    public void FireCurrentWord()
    {
        if (sm.FireCurrentWordIfValid())
        {
            //Create the standard spell 
            if (!ab)
            {
                ab = FindObjectOfType<ArenaBuilder>();
            }
            GameObject enemy = ab.GetEnemiesInArena()[0];
            sm.CreateSpell(transform, enemy.transform, SpellMaker.SpellType.Normal);

            foreach (var letter in lettersCollected)
            {
                if (letter.GetLatentAbilityStatus() == false)
                {
                    continue;
                }
                if (!aleh)
                {
                    ab = FindObjectOfType<ArenaBuilder>();
                    aleh = ab.GetComponent<ArenaLetterEffectsHandler>();
                }
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

    public void IncreaseWordLengthBonus(int bonusAmount)
    {
        wordLengthBonus += bonusAmount;
    }

    private void ResetWordLengthBonus()
    {
        wordLengthBonus = 0;
    }


}
