using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
[RequireComponent(typeof(WordWeaponizer))]
public class WordBuilder : MonoBehaviour
{
    PlayerInput pi;
    protected WordWeaponizer wwz;
    UIDriver uid;
    WordValidater wv;
    ArenaBuilder ab;
    ArenaLetterEffectsHandler aleh;

    //param
    int maxWordLength = 7; 

    //state
    bool hasUI = false;
    [SerializeField] List<LetterTile> lettersCollected = new List<LetterTile>();
    [SerializeField] protected string currentWord = "";
    int wordLengthBonus = 0;
    public int CurrentPower { get; private set; } = 0;

    private void Start()
    {
        wwz = GetComponent<WordWeaponizer>();
        pi = GetComponent<PlayerInput>();
        wv = FindObjectOfType<WordValidater>();
        if (pi)
        {
            hasUI = true;
            uid = FindObjectOfType<UIDriver>();
            uid.SetPlayerObject(this, wwz);
        }
    }

    protected virtual void AddLetter(LetterTile newLetter)
    {
        if (currentWord.Length >= maxWordLength) { return; }
        lettersCollected.Add(newLetter);
        currentWord += newLetter.Letter;
        IncreasePower(newLetter.Power);

        if (hasUI)
        {
            Sprite newSprite = newLetter.GetComponent<SpriteRenderer>().sprite;
            uid.AddLetterToWordBar(newSprite, newLetter.Letter, currentWord.Length - 1);
        }
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
        if (currentWord.Length + wordLengthBonus < roll)
        {
            return;
        }

        newLetter.SetLatentAbilityStatus(true);

        if (!aleh)
        {
            ab = FindObjectOfType<ArenaBuilder>();
            aleh = ab.GetComponent<ArenaLetterEffectsHandler>();
        }
        aleh.ApplyLetterEffectOnPickup(newLetter, gameObject, index, hasUI);
    }

    private void InactivateLatentAbility(int indexInWord)
    {
        LetterTile letterTile = lettersCollected[indexInWord];
        letterTile.SetLatentAbilityStatus(false);
        aleh.RemoveLetterEffect(indexInWord, hasUI);

    }
    private void UndoRandomActivatedAbilityAsPenalty()
    {
        List<LetterTile> activatedLetters = new List<LetterTile>();
        foreach (var letter in lettersCollected)
        {
            if (letter.GetLatentAbilityStatus() == true)
            {
                activatedLetters.Add(letter);
            }
        }
        int rand = UnityEngine.Random.Range(0, activatedLetters.Count);
        InactivateLatentAbility(rand);
    }

    #region Public Methods
    public void RemoveSpecificLetterFromCurrentWord(int indexWithinWord)
    {
        LetterTile letterToRemove = lettersCollected[indexWithinWord];

        // Subtract the base word power from current power
        CurrentPower -= letterToRemove.Power;

        // Reverse any activated latent power
        if (letterToRemove.GetLatentAbilityStatus() == true)
        {
            if (letterToRemove.Ability == TrueLetter.Ability.Lucky)
            {
                UndoRandomActivatedAbilityAsPenalty();
            }
            if (letterToRemove.Ability == TrueLetter.Ability.Shiny)
            {
                CurrentPower -= letterToRemove.Power;
            }
            aleh.RemoveLetterEffect(indexWithinWord, hasUI);
        }

        // Remove letter from current word
        lettersCollected.Remove(letterToRemove);
        currentWord = currentWord.Remove(indexWithinWord, 1);
        if (hasUI)
        {
            uid.ModifyPowerMeterTMP(CurrentPower);
        }
        wordLengthBonus--;

        letterToRemove.DestroyLetterTile();
    }

    public void RebuildCurrentWordForUI()
    {
        if (!hasUI) { return; }
        int index = 0;
        foreach (var letter in lettersCollected)
        {
            Sprite newSprite = letter.GetComponent<SpriteRenderer>().sprite;
            uid.AddLetterToWordBar(newSprite, letter.Letter, index);
            index++;
        }
    }


    public string GetCurrentWord()
    {
        return currentWord;
    }
    public int GetCurrentWordLength()
    {
        return currentWord.Length;
    }

    public virtual void ClearCurrentWord()
    {
        currentWord = "";
        ResetWordLengthBonus();
        foreach (var letter in lettersCollected)
        {
            letter.DestroyLetterTile();
        }

        lettersCollected.Clear();
        if (hasUI)
        {
            uid.ClearWordBar();
        }

        //tpm.DestroyEntireTail();
    }

    public List<LetterTile> GetLettersCollected()
    {
        return lettersCollected;
    }
    public void IncreasePower(int amount)
    {
        CurrentPower += amount;
        if (hasUI)
        {
            uid.ModifyPowerMeterTMP(CurrentPower);
        }

    }

    public void ClearPowerLevel()
    {
        CurrentPower = 0;
        if (hasUI)
        {
            uid.ModifyPowerMeterTMP(CurrentPower);
        }

    }

    public void IncreaseWordLengthBonus(int bonusAmount)
    {
        wordLengthBonus += bonusAmount;
    }

    #endregion
    private void ResetWordLengthBonus()
    {
        wordLengthBonus = 0;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        LetterTile letterTile;
        if (collision.gameObject.TryGetComponent<LetterTile>(out letterTile))
        {
            AddLetter(letterTile);
            
            letterTile.InactivateLetterTile();

        }
    }

}
