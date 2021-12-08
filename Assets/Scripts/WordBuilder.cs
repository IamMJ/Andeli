using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[RequireComponent(typeof(WordWeaponizer), typeof(WordMakerMemory))]

public class WordBuilder : MonoBehaviour
{
    PlayerInput pi;
    protected WordWeaponizer wwz;
    WordValidater wv;
    protected WordMakerMemory memory;
    CombatPanel cp;
    GameController gc;
    BagManager bagman;
    AudioSource auso;
    [SerializeField] AudioClip addLetterToSwordClip = null;
    [SerializeField] AudioClip addLetterToBagClip = null;
    [SerializeField] AudioClip destroyLetterClip = null;
    public Action<LetterTile> OnAddLetterToSword;
    
    // modifiable param
    int maxWordLength = 8;
    int powerModifierForWordCount = 0;

    //state
    public WordPack CurrentWordPack = new WordPack();
    bool hasUI = false;
    [SerializeField] List<LetterTile> lettersOnSword = new List<LetterTile>();
    [SerializeField] protected string currentWord = "";
    //int modifiedWordLength = 0;
    public int CurrentPower = 0;
    bool shouldLettersGoToSwordFirst = true;

    protected virtual void Start()
    {
        Librarian lib = Librarian.GetLibrarian();
        gc = lib.gameController;
        wwz = GetComponent<WordWeaponizer>();
        pi = GetComponent<PlayerInput>();
        auso = GetComponent<AudioSource>();
        memory = GetComponent<WordMakerMemory>();
        wv = lib.wordValidater;
        if (pi)
        {
            hasUI = true;
            cp = lib.ui_Controller.combatPanel;
            cp.SetPlayerObject(this, wwz, bagman);
            bagman = lib.bagManager;
        }
    }

    private void Update()
    {
        CurrentPower = CurrentWordPack.Power;
    }
    protected virtual void AddLetterToSword(LetterTile newLetter)
    {    
        lettersOnSword.Add(newLetter);
        OnAddLetterToSword?.Invoke(newLetter);
        //RewriteCurrentWordFromLettersOnSword();
        CurrentWordPack = CreateWordPackFromCurrentLettersOnSword(hasUI) ;
        if (hasUI)
        { 
            auso?.PlayOneShot(addLetterToSwordClip);
            cp.UpdatePanelWithNewWordPack(CurrentWordPack);
        }      

    }

    private int CalculateWordLength()
    {
        int value = currentWord.Length;
        foreach(var letter in lettersOnSword)
        {
            if (letter.Ability_Player == TrueLetter.Ability.Lucky)
            {
                value += letter.Power_Player;
                value = Mathf.Clamp(value, 0, 20);
            }
            if (gc.debug_AlwaysIgniteLetters)
            {
                value = 20;
            }
        }

        return value;
    }   

    //private void RewriteCurrentWordFromLettersOnSword()
    //{
    //    currentWord = "";
    //    foreach (var letter in lettersOnSword)
    //    {
    //        currentWord += letter.Letter;
    //    }
    //}


    private WordPack CreateWordPackFromCurrentLettersOnSword(bool isPlayer)
    {
        WordPack newWordPack = new WordPack(lettersOnSword.Count, 0, "", 0, false);
        if (lettersOnSword.Count == 0)
        {
            currentWord = newWordPack.Word;
            return newWordPack;
        }        
        newWordPack.Power = (powerModifierForWordCount * memory.GetCurrentArenaData().wordsSpelled);
        for (int i = 0; i < lettersOnSword.Count; i++)
        {
            newWordPack.letterSprites[i] = lettersOnSword[i].GetComponent<SpriteRenderer>().sprite;
            newWordPack.letterColors[i] = lettersOnSword[i].GetComponent<SpriteRenderer>().color;
            newWordPack.letterColors[i].a = 1;
            newWordPack.letterLetters[i] = lettersOnSword[i].Letter.ToString();
            if (isPlayer)
            {
                newWordPack.Power += lettersOnSword[i].Power_Player;
            }
            else
            {
                newWordPack.Power += lettersOnSword[i].Power_Enemy;
            }
            newWordPack.Word += lettersOnSword[i].Letter;
        }
        newWordPack.IsValid = wv.CheckWordValidity(newWordPack.Word);
        currentWord = newWordPack.Word;
        newWordPack.ModifiedWordLength = CalculateWordLength();

        return newWordPack;
    }
    #region Public Methods

    public bool AttemptToAddLetterToSword(LetterTile incomingLT)
    {
        if (currentWord.Length < maxWordLength)
        {
            AddLetterToSword(incomingLT);
            //WordPack swordWord = CreateWordPackFromCurrentLettersOnSword();
            //cp.UpdatePanelWithNewWordPack(swordWord);
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
            CurrentWordPack = CreateWordPackFromCurrentLettersOnSword(hasUI);
            cp.UpdatePanelWithNewWordPack(CurrentWordPack);
        }
    }
    public void RemoveLetterFromSwordAndDestroy(int index)
    {
        LetterTile letterToRemove = lettersOnSword[index];
        lettersOnSword.RemoveAt(index);
        letterToRemove.DestroyLetterTile(false);

        CurrentWordPack = CreateWordPackFromCurrentLettersOnSword(hasUI);
        cp.UpdatePanelWithNewWordPack(CurrentWordPack);
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

    public string GetCurrentWord()
    {
        return currentWord;
    }
    //public int GetEnhancedWordLength()
    //{
    //    return currentWord.Length + wordLengthBonus;
    //}

    public virtual void ClearLettersOnSword()
    {
        if (lettersOnSword.Count > 0)
        {
            for (int i = 0; i < lettersOnSword.Count; i++)
            {
                lettersOnSword[i].DestroyLetterTile(true);
            }
        }

        lettersOnSword.Clear();
        CurrentWordPack = CreateWordPackFromCurrentLettersOnSword(hasUI);
        Debug.Log("just tried to clear out the current word");
        if (hasUI)
        {
            cp.UpdatePanelWithNewWordPack(CurrentWordPack);
        }

    }

    public void ClearLastLetterInWord()
    {
        if (lettersOnSword.Count == 0) { return; }
        //Debug.Log($"letters on sword: {lettersOnSword.Count}, removing at {lettersOnSword.Count - 1}");
        lettersOnSword.RemoveAt(lettersOnSword.Count - 1);
        CurrentWordPack = CreateWordPackFromCurrentLettersOnSword(hasUI);
    }

    public List<LetterTile> GetLettersCollected()
    {
        return lettersOnSword;
    }
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
            cp.HideLetterTilesOverMaxLetterLimit(maxWordLength);
        }
    }
    #endregion


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
                letterTile.InactivateLetterTile(false);
                return true;
            }
            if (bagman.AttemptToReceiveLetter(letterTile))
            {
                letterTile.InactivateLetterTile(false);
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
                letterTile.InactivateLetterTile(false);
                return true;
            }
            if (AttemptToAddLetterToSword(letterTile))
            {
                letterTile.InactivateLetterTile(false);
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
        //NPCs don't have a word length limit to check against
        AddLetterToSword(letterTile);
        letterTile.InactivateLetterTile(false);
    }
}
