using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpellingStrategy))]
public class WordBrain_NPC : MonoBehaviour
{
    //init
    MoveBrain_NPC mb;
    [SerializeField] GameObject wordPuffPrefab = null;
    public LetterTile TargetLetterTile { get; private set; }
    WordValidater wv;
    LetterTileDropper ltd;
    DebugHelper dh;
    SpellingStrategy ss;
    VictoryMeter vm;

    //param
    int minWordOptionsToContinue = 200;

    //state
    [SerializeField] string currentWord = "";
    [SerializeField] char currentTargetChar;
    int currentPower = 0;

    void Start()
    {
        ss = GetComponent<SpellingStrategy>();
        dh = FindObjectOfType<DebugHelper>();
        wv = FindObjectOfType<WordValidater>();
        mb = GetComponent<MoveBrain_NPC>();
        ltd = FindObjectOfType<LetterTileDropper>();
        ltd.OnLetterListModified += DetermineBestTargetLetter;
        vm = FindObjectOfType<VictoryMeter>();
        
    }

    private void Update()
    {
        if (TargetLetterTile)
        {
            currentTargetChar = TargetLetterTile.Letter;
        }
        else { currentTargetChar = '?'; }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        LetterTile letterTile;
        if (collision.gameObject.TryGetComponent<LetterTile>(out letterTile))
        {
            AddLetter(letterTile.Letter);
            IncreasePower(letterTile.Power);
            Destroy(collision.gameObject);
            ss.EvaluateWordAfterGainingALetter();
        }

    }

    #region Simple Private Tasks
    private void OnDestroy()
    {
        ltd.OnLetterListModified -= DetermineBestTargetLetter;
    }
    private void AddLetter(char newLetter)
    {
        currentWord += newLetter;
    }
    private void ClearCurrentWord()
    {
        currentWord = "";
    }
    private void IncreasePower(int amount)
    {
        currentPower += amount;
        
    }
    private void ClearPower()
    {
        currentPower = 0;
    }

    #endregion

    #region Hooks For Spelling Strategy

    public void FireOffWord()
    {
        vm.ModifyBalance(-currentPower);

        GameObject puff = Instantiate(wordPuffPrefab, transform.position, Quaternion.identity) as GameObject;
        WordPuff wordpuff = puff.GetComponent<WordPuff>();
        wordpuff.SetColorByPower(currentPower);
        wordpuff.SetText(currentWord);
        EraseWord();
    }

    public void EraseWord()
    {
        ClearCurrentWord();
        ClearPower();
    }

    public string GetCurrentWord()
    {
        return currentWord;
    }

    #endregion

    private void DetermineBestTargetLetter(LetterTile changedLetterTile, bool wasLetterAdded)
    {
        if (ltd.doesBoardHaveLettersAvailable == false)
        {
            TargetLetterTile = null;
            return;
        }
        if (!wasLetterAdded && TargetLetterTile == changedLetterTile) //if a letter was removed, and it was the target letter...
        {
 
            TargetLetterTile = ss.FindBestLetterFromAllOnBoard();

            return;
        }
        if (wasLetterAdded) //Since a new letter was added...
        {
            // Decide if the new letter is a better choice for target letter
            if (currentWord.Length == 0 && !TargetLetterTile)
            {
                TargetLetterTile = changedLetterTile;
                //Debug.Log($"targeting {TargetLetterTile.Letter} by default");
                return;
            }
            else
            {
                TargetLetterTile = ss.FindBestLetterFromAllOnBoard();
            }
        }
    }   
}
