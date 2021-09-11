using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
[RequireComponent (typeof (WordWeaponizer))]
public class WordBuilder : MonoBehaviour
{
    PlayerInput pi;
    protected WordWeaponizer wwz;
    PowerMeter pm;
    UIDriver uid;
    WordValidater wv;
    ArenaBuilder ab;
    ArenaLetterEffectsHandler aleh;

    //param
    int maxWordLength = 10;

    //state
    bool hasUI = false;
    List<LetterTile> lettersCollected = new List<LetterTile>();
    protected string currentWord = "";
    int wordLengthBonus = 0;
    public int CurrentPower { get; private set; } = 0;

    private void Start()
    {
        wwz = GetComponent<WordWeaponizer>();
        wv = FindObjectOfType<WordValidater>();
        pi = GetComponent<PlayerInput>();
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

    #region Public Methods
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
            letterTile.PickupLetterTile();

        }
    }

}
