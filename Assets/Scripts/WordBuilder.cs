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
    ArenaLetterEffectsHandler aleh;
    GameController gc;
    BagManager bagman;
    AudioSource auso;
    [SerializeField] AudioClip addLetterToSwordClip = null;
    [SerializeField] AudioClip addLetterToBagClip = null;
    [SerializeField] AudioClip destroyLetterClip = null;
    public Action OnAddLetterToSword;
    public struct SwordWordPower
    {
        public Sprite[] letterSprites;
        public Color[] letterColors;
        public string[] letterLetters;
        public string word;
        public int Power;

        public SwordWordPower(int wordLength, int power)
        {
            letterSprites = new Sprite[wordLength];
            letterColors = new Color[wordLength];
            letterLetters = new string[wordLength];
            Power = power;
            word = "";
        }
    }

    // modifiable param
    int maxWordLength = 8;
    int powerModifierForWordCount = 0;

    //state
    bool hasUI = false;
    [SerializeField] List<LetterTile> lettersOnSword = new List<LetterTile>();
    [SerializeField] protected string currentWord = "";
    int modifiedWordLength = 0;
    public int CurrentPower = 0;
    bool shouldLettersGoToSwordFirst = true;

    protected virtual void Start()
    {
        gc = FindObjectOfType<GameController>();
        wwz = GetComponent<WordWeaponizer>();
        pi = GetComponent<PlayerInput>();
        auso = GetComponent<AudioSource>();
        memory = GetComponent<WordMakerMemory>();
        if (pi)
        {
            hasUI = true;
            uid = FindObjectOfType<UIDriver>();
            uid.SetPlayerObject(this, wwz, bagman);
            bagman = FindObjectOfType<BagManager>();
            uid.UpdateIgnitionChanceTMP(0);
        }
    }

    protected virtual void AddLetterToSword(LetterTile newLetter)
    {
    
        lettersOnSword.Add(newLetter);
        OnAddLetterToSword?.Invoke();
        RewriteCurrentWordFromLettersOnSword();
        modifiedWordLength = CalculateWordLengthAndUpdateIgnitionChance();
        //IncreasePower(newLetter.Power_Player);
        SwordWordPower swordWord = CreateSwordWordPowerFromCurrentWord();
        CurrentPower = swordWord.Power;
        if (hasUI)
        {
            auso?.PlayOneShot(addLetterToSwordClip);
        }

        if (hasUI)
        {
            //if (memory.CheckIfWordHasBeenPlayedByPlayerAlready(currentWord))
            //{
            //    Debug.Log("already played this word...");
            //}
            //else
            //{
            //    Debug.Log("still a novel word");
            //}
            //uid.AddLetterToWordBar(newLetter, newLetter.Letter, currentWord.Length - 1);

            uid.UpdateLettersOnSwordAndPower(swordWord);
        }
        //if (gc.debug_IgniteAll)
        //{
        //    newLetter.SetLatentAbilityStatus(true);
        //    int index = currentWord.Length - 1;
        //    if (!aleh)
        //    {
        //        ab = FindObjectOfType<ArenaBuilder>();
        //        aleh = ab.GetComponent<ArenaLetterEffectsHandler>();
        //    }
        //    aleh.ApplyLetterEffectOnPickup(newLetter, gameObject, index, hasUI);
        //}
        //else
        //{
        //    TestAllLetterLatentAbilities();
        //}

    }

    private int CalculateWordLengthAndUpdateIgnitionChance()
    {
        int value = currentWord.Length;
        foreach(var letter in lettersOnSword)
        {
            if (letter.Ability_Player == TrueLetter.Ability.Lucky)
            {
                value += letter.Power_Player;
                value = Mathf.Clamp(value, 0, 20);
            }
            if (gc.debug_IgniteAll)
            {
                value = 20;
            }
        }
        if (hasUI)
        {
            uid.UpdateIgnitionChanceTMP(Mathf.Round(value * 5));
        }

        return value;
    }

    //private void TestAllLetterLatentAbilities()
    //{

    //    for (int i =0; i < lettersOnSword.Count; i++)
    //    {
    //        if (lettersOnSword[i].GetLatentAbilityStatus() == false)
    //        {
    //            TestLetterLatentAbility(lettersOnSword[i], i);
    //        }
    //    }
    //}



    //private void TestLetterLatentAbility(LetterTile newLetter, int index)
    //{

    //    newLetter.SetLatentAbilityStatus(true);

    //    if (!aleh)
    //    {
    //        ab = FindObjectOfType<ArenaBuilder>();
    //        aleh = ab.GetComponent<ArenaLetterEffectsHandler>();
    //    }
    //    aleh.ApplyLetterEffectOnPickup(newLetter, gameObject, index, hasUI);
    //}

    //private void InactivateLatentAbility(int indexInWord)
    //{
    //    LetterTile letterTile = lettersOnSword[indexInWord];
    //    letterTile.SetLatentAbilityStatus(false);
    //    aleh.RemoveLetterParticleEffect(indexInWord, hasUI);

    //}
    //private void UndoRandomActivatedAbilityAsPenalty()
    //{
    //    List<LetterTile> activatedLetters = new List<LetterTile>();
    //    foreach (var letter in lettersOnSword)
    //    {
    //        if (letter.GetLatentAbilityStatus() == true)
    //        {
    //            activatedLetters.Add(letter);
    //        }
    //    }
    //    int rand = UnityEngine.Random.Range(0, activatedLetters.Count);
    //    InactivateLatentAbility(rand);
    //}

    private void RewriteCurrentWordFromLettersOnSword()
    {
        currentWord = "";
        foreach (var letter in lettersOnSword)
        {
            currentWord += letter.Letter;
        }
    }

    private SwordWordPower CreateSwordWordPowerFromCurrentWord()
    {
        SwordWordPower newSwordWord = new SwordWordPower(currentWord.Length, 0);
        newSwordWord.word = currentWord;
        newSwordWord.Power = (powerModifierForWordCount * memory.GetCurrentArenaData().wordsSpelled);
        for (int i = 0; i < currentWord.Length; i++)
        {
            newSwordWord.letterSprites[i] = lettersOnSword[i].GetComponent<SpriteRenderer>().sprite;
            newSwordWord.letterColors[i] = lettersOnSword[i].GetComponent<SpriteRenderer>().color;
            newSwordWord.letterColors[i].a = 1;
            newSwordWord.letterLetters[i] = lettersOnSword[i].Letter.ToString();
            newSwordWord.Power += lettersOnSword[i].Power_Player;
        }
        CurrentPower = newSwordWord.Power;
        return newSwordWord;
    }
    #region Public Methods

    public bool AttemptToAddLetterToSword(LetterTile incomingLT)
    {
        if (currentWord.Length < maxWordLength)
        {
            AddLetterToSword(incomingLT);
            RewriteCurrentWordFromLettersOnSword();
            SwordWordPower swordWord = CreateSwordWordPowerFromCurrentWord();
            uid.UpdateLettersOnSwordAndPower(swordWord);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RemoveLetterFromSwordAndSendToBag(int index)
    {
        if (bagman.AttemptToReceiveLetter(lettersOnSword[index]))
        {
            auso?.PlayOneShot(addLetterToBagClip);
            lettersOnSword.RemoveAt(index);
            RewriteCurrentWordFromLettersOnSword();
            SwordWordPower swordWord = CreateSwordWordPowerFromCurrentWord();
            uid.UpdateLettersOnSwordAndPower(swordWord);
            modifiedWordLength = CalculateWordLengthAndUpdateIgnitionChance();
        }
    }
    public void RemoveLetterFromSwordAndDestroy(int index)
    {
        LetterTile letterToRemove = lettersOnSword[index];
        lettersOnSword.RemoveAt(index);
        letterToRemove.DestroyLetterTile();
        RewriteCurrentWordFromLettersOnSword();
        SwordWordPower swordWord = CreateSwordWordPowerFromCurrentWord();
        uid.UpdateLettersOnSwordAndPower(swordWord);
        modifiedWordLength = CalculateWordLengthAndUpdateIgnitionChance();
        auso?.PlayOneShot(destroyLetterClip);
        //// Subtract the base word power from current power
        //CurrentPower -= letterToRemove.Power_Player;

        // Reverse any activated latent power
        //if (letterToRemove.GetLatentAbilityStatus() == true)
        //{
        //    if (letterToRemove.Ability_Player == TrueLetter.Ability.Lucky)
        //    {
        //        UndoRandomActivatedAbilityAsPenalty();
        //    }
        //    if (letterToRemove.Ability_Player == TrueLetter.Ability.Shiny)
        //    {
        //        //CurrentPower -= letterToRemove.Power_Player;
        //    }
        //    //aleh.RemoveLetterParticleEffect(indexWithinWord, hasUI);
        //}

        // Remove letter from current word


    }

    public void PlayDestroyLetterClip()
    {
        auso?.PlayOneShot(destroyLetterClip);
    }

    public void RebuildCurrentWordForUI()
    {
        if (!hasUI) { return; }
        int index = 0;
        foreach (var letter in lettersOnSword)
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
    //public int GetEnhancedWordLength()
    //{
    //    return currentWord.Length + wordLengthBonus;
    //}

    public virtual void ClearCurrentWord()
    {
        currentWord = "";
        CurrentPower = 0;
        foreach (var letter in lettersOnSword)
        {
            letter.DestroyLetterTile();
        }

        lettersOnSword.Clear();
        if (hasUI)
        {
            uid.ClearWordBar();
        }
        modifiedWordLength = CalculateWordLengthAndUpdateIgnitionChance(); ;
        //tpm.DestroyEntireTail();
    }

    public void ClearCurrentWord(int numberOfLettersToRemoveFromEnd)
    {
        //TODO
    }

    public List<LetterTile> GetLettersCollected()
    {
        return lettersOnSword;
    }
    //public void ClearPowerLevel()
    //{
    //    CurrentPower = (powerModifierForWordCount * memory.GetCurrentArenaData().wordsSpelled);

    //    if (hasUI)
    //    {
    //        uid.ModifyPowerMeterTMP(CurrentPower);
    //    }

    //}

    public bool ToggleLetterRoutingMode()
    {
        shouldLettersGoToSwordFirst = !shouldLettersGoToSwordFirst;
        return shouldLettersGoToSwordFirst;
    }

    #endregion

    #region Public Arena Parameter Setting

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

    public int GetModifiedWordLength()
    {
        return modifiedWordLength;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            LetterTile letterTile = collision.gameObject.GetComponent<LetterTile>();
            if (hasUI)
            {
                if (AttemptToPickUpLetterTile_Player(letterTile))
                {
                    //TODO play a bump sound;
                }
            }
            else
            {
                AttemptToPickUpLetterTile_NPC(letterTile);
            }
        }
        if (collision.gameObject.layer == 16 && hasUI)
        {
            LetterTile letterTile = collision.gameObject.GetComponent<LetterTile>();
            if (AttemptToPickUpLetterTile_Player(letterTile))
            {
                //TODO play a bump sound;
            }


        }

    }

    private bool AttemptToPickUpLetterTile_Player(LetterTile letterTile)
    {
        if (shouldLettersGoToSwordFirst)
        {
            if (AttemptToAddLetterToSword(letterTile))
            {
                letterTile.InactivateLetterTile();
                return true;
            }
            if (bagman.AttemptToReceiveLetter(letterTile))
            {
                letterTile.InactivateLetterTile();
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (bagman.AttemptToReceiveLetter(letterTile))
            {
                letterTile.InactivateLetterTile();
                return true;
            }
            if (AttemptToAddLetterToSword(letterTile))
            {
                letterTile.InactivateLetterTile();
                return true;
            }
            else
            {
                return false;
            }
        }

    }
    private void AttemptToPickUpLetterTile_NPC(LetterTile letterTile)
    {
        AddLetterToSword(letterTile);
        letterTile.InactivateLetterTile();
    }
}
