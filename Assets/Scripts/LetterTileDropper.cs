using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LetterTileDropper : MonoBehaviour
{
    [SerializeField] GameObject letterTilePrefab = null;
    [SerializeField] TrueLetter[] trueLetters = null;


    //param
    float timeBetweenDrops = 2f;
    float distanceFromOrigin = 10f;

    //state
    float timeForNextDrop;
    int currentProbabilityCount = 0;

    private void Start()
    {
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
        Debug.Log("prob weight total" + currentProbabilityCount);
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
        float randomX = UnityEngine.Random.Range(-distanceFromOrigin, distanceFromOrigin);
        float randomY = UnityEngine.Random.Range(-distanceFromOrigin, distanceFromOrigin);
        Vector2 randomPos = new Vector2(randomX, randomY);
        randomPos = GridHelper.SnapToGrid(randomPos, 1);

        GameObject newTile = Instantiate(letterTilePrefab, randomPos, Quaternion.identity) as GameObject;

        TrueLetter randomLetter = ReturnWeightedRandomTrueLetter();
        LetterTile letterTile = newTile.GetComponent<LetterTile>();
        letterTile.Letter = randomLetter.GetLetter();
        letterTile.Power = randomLetter.GetPower();
        newTile.GetComponentInChildren<TextMeshPro>().text = randomLetter.GetLetter().ToString();
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
