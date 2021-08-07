using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordBrain_NPC : MonoBehaviour
{
    //init
    MoveBrain_NPC mb;
    public LetterTile TargetLetterTile { get; private set; }
    WordValidater wv;
    LetterTileDropper ltd;
    DebugHelper dh;

    //param
    int minWordOptionsToContinue = 200;

    //state
    [SerializeField] string currentWord = "";
    [SerializeField] char currentTargetChar;
    int currentPower = 0;

    void Start()
    {
        dh = FindObjectOfType<DebugHelper>();
        wv = FindObjectOfType<WordValidater>();
        mb = GetComponent<MoveBrain_NPC>();
        ltd = FindObjectOfType<LetterTileDropper>();
        ltd.OnLetterListModified += DetermineBestTargetLetter;
    }

    private void Update()
    {
        if (TargetLetterTile)
        {
            currentTargetChar = TargetLetterTile.Letter;
        }
        else { currentTargetChar = '?'; }
    }

    #region Simple Tasks
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        LetterTile letterTile;
        if (collision.gameObject.TryGetComponent<LetterTile>(out letterTile))
        {
            AddLetter(letterTile.Letter);
            IncreasePower(letterTile.Power);
            Destroy(collision.gameObject);
            FireOffCurrentWordIfPossible();
            EraseWordIfLowChanceOfFinishing();
        }

    }

    private void EraseWordIfLowChanceOfFinishing()
    {
        int count = wv.FindWordBandWithStubWord(currentWord).Range;
        if (count < minWordOptionsToContinue)
        {
            //erase word;
            dh.DisplayDebugLog($"erasing {currentWord} with only {count} options");
            ClearCurrentWord();
        }
    }

    private void FireOffCurrentWordIfPossible()
    {
        if (wv.CheckWordValidity(currentWord,gameObject))
        {
            Debug.Log($"firing off {currentWord}");
            dh.DisplayDebugLog(currentWord);
            //Fire the word
            ClearCurrentWord();
        }
    }


    private void DetermineBestTargetLetter(LetterTile changedLetterTile, bool wasLetterAdded)
    {
        if (ltd.doesBoardHaveLettersAvailable == false)
        {
            TargetLetterTile = null;
            return;
        }
        if (!wasLetterAdded && TargetLetterTile == changedLetterTile) //if a letter was removed, and it was the target letter...
        {
 
            TargetLetterTile = FindBestLetterFromAllOnBoard();

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
                TargetLetterTile = FindBestLetterFromAllOnBoard();
            }
        }
    }

    private LetterTile FindBestLetterFromAllOnBoard()
    {
        List<LetterTile> letterTilesToEvaluate = ltd.FindAllReachableLetterTiles(transform.position, mb.moveSpeed);
        //Debug.Log($"evaluating {letterTilesToEvaluate.Count} letters");
        LetterTile currentBestOption = null;
        float currentBestValue = 0;
        foreach (var letterTile in letterTilesToEvaluate)
        {
            float hValue = CalculatePowerDistanceValue(letterTile) * CalculateFollowOnWordPotential(letterTile);
            //Debug.Log($"adding a {letterTile.Letter} to {currentWord} is worth {hValue} hValue. Follow-on Words: {possibleRefinedWordBand.Range}");
            if (hValue > currentBestValue)
            {
                currentBestOption = letterTile;
                currentBestValue = hValue;
                Debug.Log($"best Option: {letterTile.Letter} at hValue: {currentBestValue}");
                //Debug.Log($"updated current word band to {currentWordBandToSearch.StartIndex}, {currentWordBandToSearch.Range}");
            }
            else
            {
                Debug.Log("couldn't find a better option for TargetLetter");
            }
        }
        return currentBestOption;

    }
    private float CalculatePowerDistanceValue(LetterTile letterTile)
    {
        float dist = (letterTile.transform.position - transform.position).magnitude * 1.5f;
        float value = (letterTile.Power / dist);

        return value;
    }
    private float CalculateFollowOnWordPotential(LetterTile letterTile)
    {
        string hypotheticalWord;
        if (currentWord.Length == 0)
        {
            hypotheticalWord = letterTile.Letter.ToString();
        }
        else
        {
            hypotheticalWord = currentWord + letterTile.Letter;
        }

        int count = wv.FindWordBandWithStubWord(hypotheticalWord).Range;
        //Debug.Log($"{letterTile.Letter} hypothetical option: {hypotheticalWordBand.StartIndex}, {hypotheticalWordBand.Range}");

        return count/10000f;
    }
}
