using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LetterTileDropper : MonoBehaviour
{
    [SerializeField] GameObject letterTilePrefab = null;
    [SerializeField] TrueLetter[] trueLetters = null;
    GameObject[] wordMakers;


    //param
    float timeBetweenDrops = 0.5f;
    float distanceFromOrigin = 15f;
    float minDistanceToWordMaker = 5f;
    float minDistanceBetweenLetters = 2f;
    int layerMask_Letter = 1 << 9;

    //state
    float timeForNextDrop;
    int currentProbabilityCount = 0;

    private void Start()
    {
        wordMakers = GameObject.FindGameObjectsWithTag("Wordmaker");
        timeForNextDrop = Time.time + timeBetweenDrops;
        GenerateProbabilityStarts();
    }

    private void GenerateProbabilityStarts()
    {
        foreach (TrueLetter tl in trueLetters)
        {
            currentProbabilityCount += tl.GetWeight();
            tl.ProbabilityTop = currentProbabilityCount;
        }
        //Debug.Log("prob weight total" + currentProbabilityCount);
    }

    private void Update()
    {
        if (Time.time >= timeForNextDrop)
        {
            DropLetterTile();
            timeForNextDrop = Time.time + timeBetweenDrops;
        }
        
    }

    private void DropLetterTile()
    {
        Vector2 randomPos = GetRandomValidPositionOutsideOfMinRange();

        GameObject newTile = Instantiate(letterTilePrefab, randomPos, Quaternion.identity) as GameObject;

        TrueLetter randomLetter = ReturnWeightedRandomTrueLetter();
        LetterTile letterTile = newTile.GetComponent<LetterTile>();
        letterTile.Letter = randomLetter.GetLetter();
        letterTile.Power = randomLetter.GetPower();
        newTile.GetComponentInChildren<TextMeshPro>().text = randomLetter.GetLetter().ToString();
    }

    private Vector2 GetRandomValidPositionOutsideOfMinRange()
    {
        Vector2 randomPos;
        int attempts = 0;
        do
        {
            float randomX = UnityEngine.Random.Range(-distanceFromOrigin, distanceFromOrigin);
            float randomY = UnityEngine.Random.Range(-distanceFromOrigin, distanceFromOrigin);
            randomPos = new Vector2(randomX, randomY);
            attempts++;
            if (attempts > 100)
            {
                break;
            }
        }
        while (!VerifyAllWordMakersOutsideMinDistance(randomPos) || !VerifyAllLettersOutsideMinDistance(randomPos));
        randomPos = GridHelper.SnapToGrid(randomPos, 1);
        return randomPos;
    }

    private bool VerifyAllWordMakersOutsideMinDistance(Vector2 randomPos)
    {
        bool isOutsideMinRange = true;
        foreach (var wordmaker in wordMakers)
        {
            if ((wordmaker.transform.position - (Vector3)randomPos).magnitude < minDistanceToWordMaker)
            {
                isOutsideMinRange = false;
                Debug.Log("isOutsideMinRange: " + isOutsideMinRange);
                break;
            }
        }
        return isOutsideMinRange;
    }
    private bool VerifyAllLettersOutsideMinDistance(Vector2 randomPos)
    {
        var coll = Physics2D.OverlapCircle(randomPos, minDistanceBetweenLetters, layerMask_Letter);
        if (coll)
        {
            Debug.Log("too close!");
            return false;
        }
        else
        {
            Debug.Log("not too close.");
            return true;
        }
    }

    private TrueLetter ReturnWeightedRandomTrueLetter()
    {
        int rand = UnityEngine.Random.Range(0, currentProbabilityCount);

        foreach (var tl in trueLetters)
        {
            if (rand <= tl.ProbabilityTop)
            {
                //Debug.Log($"saw {rand}, choosing {tl.Letter} with top of {tl.ProbabilityTop}");
                return tl;
            }
        }

        return null;
    }
}
