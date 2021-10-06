using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
[RequireComponent(typeof(WordWeaponizer), typeof(WordMakerMemory))]
public class WordBuilder : MonoBehaviour
{
    PlayerInput pi;
    protected WordWeaponizer wwz;
    protected WordMakerMemory memory;
    UIDriver uid;
    WordValidater wv;
    ArenaBuilder ab;
    ArenaLetterEffectsHandler aleh;
    GameController gc;

    // modifiable param
    int maxWordLength = 7;
    int powerModifierForWordCount = 0;

    //state
    bool hasUI = false;
    [SerializeField] List<LetterTile> lettersCollected = new List<LetterTile>();
    [SerializeField] protected string currentWord = "";
    int wordLengthBonus = 0;
    TrueLetter.Ability abilityToAutoIgnite;
    public int CurrentPower = 0;

    protected virtual void Start()
    {
        gc = FindObjectOfType<GameController>();
        wwz = GetComponent<WordWeaponizer>();
        pi = GetComponent<PlayerInput>();
        memory = GetComponent<WordMakerMemory>();
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
        IncreasePower(newLetter.Power_Player);
        if (hasUI)
        {
            if (memory.CheckIfWordHasBeenPlayedByPlayerAlready(currentWord))
            {
                Debug.Log("already played this word...");
            }
            else
            {
                Debug.Log("still a novel word");
            }
            uid.AddLetterToWordBar(newLetter, newLetter.Letter, currentWord.Length - 1);
        }
        if (gc.debug_IgniteAll)
        {
            newLetter.SetLatentAbilityStatus(true);
            int index = currentWord.Length - 1;
            if (!aleh)
            {
                ab = FindObjectOfType<ArenaBuilder>();
                aleh = ab.GetComponent<ArenaLetterEffectsHandler>();
            }
            aleh.ApplyLetterEffectOnPickup(newLetter, gameObject, index, hasUI);
        }
        else
        {
            TestAllLetterLatentAbilities();
        }

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
        if (newLetter.Ability_Player != abilityToAutoIgnite)
        {
            int roll = 21 - UnityEngine.Random.Range(1, 21);
            if (currentWord.Length + wordLengthBonus < roll)
            {
                return;
            }
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
        aleh.RemoveLetterParticleEffect(indexInWord, hasUI);

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
        CurrentPower -= letterToRemove.Power_Player;

        // Reverse any activated latent power
        if (letterToRemove.GetLatentAbilityStatus() == true)
        {
            if (letterToRemove.Ability_Player == TrueLetter.Ability.Lucky)
            {
                UndoRandomActivatedAbilityAsPenalty();
            }
            if (letterToRemove.Ability_Player == TrueLetter.Ability.Shiny)
            {
                CurrentPower -= letterToRemove.Power_Player;
            }
            aleh.RemoveLetterParticleEffect(indexWithinWord, hasUI);
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
            uid.AddLetterToWordBar(letter, letter.Letter, index);
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
        if (CurrentPower == 0)
        {
            CurrentPower = (powerModifierForWordCount * memory.GetCurrentArenaData().wordsSpelled);
        }
        CurrentPower += amount;
        if (hasUI)
        {
            uid.ModifyPowerMeterTMP(CurrentPower);
        }

    }

    public void ClearPowerLevel()
    {
        CurrentPower = (powerModifierForWordCount * memory.GetCurrentArenaData().wordsSpelled);

        if (hasUI)
        {
            uid.ModifyPowerMeterTMP(CurrentPower);
        }

    }

    #endregion

    #region Public Arena Parameter Setting

    public void SetupArenaParameters_AbilityToAutoIgnite(TrueLetter.Ability abilityToIgnite)
    {
        abilityToAutoIgnite = abilityToIgnite;
    }
    public void SetupArenaParameters_PowerModifierForWordCount(int powerMod)
    {
        powerModifierForWordCount = powerMod;
    }
    public void SetupArenaParameters_MaxLettersInWord(int maxLetters)
    {
        maxWordLength = maxLetters;
        if (hasUI)
        {
            uid.HideLetterTilesOverMaxLetterLimit(maxWordLength);
        }
    }
    #endregion
    private void ResetWordLengthBonus()
    {
        wordLengthBonus = 0;
    }
    public void IncreaseWordLengthBonus(int amount)
    {
        wordLengthBonus += amount;
    }



    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            LetterTile letterTile = collision.gameObject.GetComponent<LetterTile>();
            if (letterTile.IsMystic)
            {
                if (hasUI)
                {
                    AddLetter(letterTile);
                    letterTile.InactivateLetterTile();
                }
                else
                {
                    return;
                }
            }
            else
            {
                AddLetter(letterTile);
                letterTile.InactivateLetterTile();
            }
        }
    }

}
