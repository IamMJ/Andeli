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
    

    //state
    [SerializeField] string currentWord = "";
    [SerializeField] char currentTargetChar;
    int currentPower = 0;

    void Start()
    {
        wv = FindObjectOfType<WordValidater>();
        mb = GetComponent<MoveBrain_NPC>();
        ltd = FindObjectOfType<LetterTileDropper>();
        ltd.OnLetterListModified += DetermineBestTargetLetter;
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
            DetermineValueOfCurrentWord();
        }

    }

    private void DetermineValueOfCurrentWord()
    {
        if (wv.CheckWordValidity(currentWord))
        {
            Debug.Log("simplest word made - fire it off!");

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
            currentTargetChar = TargetLetterTile.Letter;

            return;
        }
        if (wasLetterAdded) //Since a new letter was added...
        {
            // Decide if the new letter is a better choice for target letter
            if (currentWord.Length == 0 && !TargetLetterTile)
            {
                TargetLetterTile = changedLetterTile;
                currentTargetChar = TargetLetterTile.Letter;
                Debug.Log($"targeting {TargetLetterTile.Letter} by default");
                return;
            }
            else
            {
                TargetLetterTile = FindBestLetterFromAllOnBoard();
                currentTargetChar = TargetLetterTile.Letter;
            }
        }
    }

    private LetterTile FindBestLetterFromAllOnBoard()
    {
        List<LetterTile> letterTilesToEvaluate = ltd.FindAllReachableLetterTiles(transform.position, mb.moveSpeed);
        Debug.Log($"evaluating {letterTilesToEvaluate.Count} letters");
        LetterTile currentBestOption = null;
        float currentBestValue = 0;
        foreach (var letterTile in letterTilesToEvaluate)
        {
            float hValue = CalculateHValue(letterTile);
            if (hValue > currentBestValue)
            {
                currentBestOption = letterTile;
                currentBestValue = hValue;
            }
        }
        Debug.Log($"best Option: {TargetLetterTile.Letter} at hValue: {currentBestValue}");
        return currentBestOption;

    }

    private float CalculateHValue(LetterTile letterTile)
    {
        float dist = (letterTile.transform.position - transform.position).magnitude * 1.5f;
        string hypotheticalWord;
        if (currentWord.Length > 0)
        {
            hypotheticalWord = currentWord + letterTile.Letter;
        }
        else
        {
            hypotheticalWord = letterTile.Letter.ToString();
        }
        //float value = (letterTile.Power / dist) + (wv.FindPossibleWordCountWithStubWord(hypotheticalWord) / 10000f);
        float value = (wv.FindPossibleWordCountWithStubWord(hypotheticalWord));

        return value;
    }
}
