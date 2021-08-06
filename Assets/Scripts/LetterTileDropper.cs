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
    ArenaBuilder ab;
    SpeedKeeper sk;
    List<LetterTile> spawnedLetterTiles = new List<LetterTile>();

    public Action<LetterTile, bool> OnLetterListModified;  //True means letter was added, false means letter was removed.

    //param
    float timeBetweenDrops = 1f;
    float universalMinDistanceToWordMaker = 2f;
    float minDistanceBetweenLetters = 2f;
    int layerMask_Letter = 1 << 9;

    //state
    float timeForNextDrop;
    int currentProbabilityCount = 0;

    private void Start()
    {
        sk = FindObjectOfType<SpeedKeeper>();
        ab = FindObjectOfType<ArenaBuilder>();
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
            timeForNextDrop = Time.time + timeBetweenDrops + UnityEngine.Random.Range(-timeBetweenDrops/4f, timeBetweenDrops/4f);
        }
        
    }

    private void DropLetterTile()
    {
        Vector2 randomPos = GetRandomPositionOutsideOfMinRangeAndInsideArena();

        GameObject newTile = Instantiate(letterTilePrefab, randomPos, Quaternion.identity) as GameObject;

        TrueLetter randomLetter = ReturnWeightedRandomTrueLetter();
        LetterTile letterTile = newTile.GetComponent<LetterTile>();
        letterTile.Letter = randomLetter.GetLetter();
        letterTile.Power = randomLetter.GetPower();
        letterTile.Lifetime = 10f + UnityEngine.Random.Range(-2f, 2f);
        letterTile.SetLetterTileDropper(this);
        newTile.GetComponentInChildren<TextMeshPro>().text = randomLetter.GetLetter().ToString();
        AddLetterToSpawnedLetterList(letterTile);

    }

    private Vector2 GetRandomPositionOutsideOfMinRangeAndInsideArena()
    {
        if (!ab)
        {
            ab = FindObjectOfType<ArenaBuilder>();
        }
        Vector2 randomPos;
        int attempts = 0;
        do
        {
            randomPos = ab.CreateRandomPointWithinArena();
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
        if (!sk)
        {
            sk = FindObjectOfType<SpeedKeeper>();
        }
        float currentMinDistance = Mathf.RoundToInt(sk.CurrentSpeed);
        currentMinDistance = Mathf.Clamp(currentMinDistance, universalMinDistanceToWordMaker, 10f);
        bool isOutsideMinRange = true;
        foreach (var wordmaker in wordMakers)
        {
            if ((wordmaker.transform.position - (Vector3)randomPos).magnitude < currentMinDistance)
            {
                isOutsideMinRange = false;
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
            return false;
        }
        else
        {
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

    private void AddLetterToSpawnedLetterList(LetterTile newLetterTile)
    {
        OnLetterListModified?.Invoke(newLetterTile, true);
        spawnedLetterTiles.Add(newLetterTile);
    }

    public void RemoveLetterFromSpawnedLetterList(LetterTile letterTileToRemove)
    {
        OnLetterListModified?.Invoke(letterTileToRemove, false);
        spawnedLetterTiles.Remove(letterTileToRemove);
    }

    public void DestroyAllLetters()
    {
        for (int i = 0; i < spawnedLetterTiles.Count; i++)
        {
            Destroy(spawnedLetterTiles[i]);
        }
        spawnedLetterTiles.Clear();
    }
}
