using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class LetterTileDropper : MonoBehaviour
{
    [SerializeField] GameObject letterTilePrefab = null;
    [SerializeField] TrueLetter[] trueLetters = null;

    public List<TrueLetter> consonants = new List<TrueLetter>();
    public List<TrueLetter> vowels = new List<TrueLetter>();
    WordValidater wv;
    GameObject[] wordMakers;
    ArenaBuilder ab;
    SpeedKeeper sk;
    int layerMask_Impassable = 1 << 13;
    public List<LetterTile> letterTilesOnBoard = new List<LetterTile>();

    public Action<LetterTile, bool> OnLetterListModified;  //True means letter was added, false means letter was removed.

    //param
    float timeBetweenDrops = 1f;
    float universalMinDistanceToWordMaker = 4f;
    float minDistanceBetweenLetters = 2f;
    int layerMask_Letter = 1 << 9;
    public int consonantsBetweenVowels = 2;

    //state
    public int dropsSinceLastVowel = 0;
    float timeForNextDrop;
    public int currentProbabilityCount_Consonants = 0;
    public int currentProbabilityCount_Vowels = 0;
    public bool doesBoardHaveLettersAvailable { get; private set; } = true;
    [SerializeField] Vector2 nextTileDropPosition = Vector2.zero;
    [SerializeField] Vector2 previousTileDropPosition = Vector2.zero;
    [SerializeField] Vector2 randomPosition = Vector2.zero;
    [SerializeField] bool isRandomPositionBeingGenerated = false;
    [SerializeField] bool isNextTileReadyToBeDropped = false;

    bool isRandomPositionTooNearWordMakers = false;
    bool isRandomPositionTooNearOtherLetters = false;

    private void Start()
    {
        // Rather than starting will all 26 True Letters, probably should pull this from the player every time an Arena is built
        // trueLetters = GetPlayersTrueLetters();
        wv = FindObjectOfType<WordValidater>();
        sk = FindObjectOfType<SpeedKeeper>();
        ab = FindObjectOfType<ArenaBuilder>();
        wordMakers = GameObject.FindGameObjectsWithTag("Wordmaker");
        timeForNextDrop = Time.time + timeBetweenDrops;
        SeparateVowelsFromConsonants();
        GenerateProbabilityStarts();
    }

    private void SeparateVowelsFromConsonants()
    {
        foreach(var tl in trueLetters)
        {
            char letter = tl.GetLetter();
            if ((letter == 'A') || (letter == 'E') || (letter == 'I') 
                || (letter == 'O') || (letter == 'U') || (letter == 'Y'))
            {
                vowels.Add(tl);
            }
            else
            {
                consonants.Add(tl);
            }
        }
    }

    private void GenerateProbabilityStarts()
    {
        foreach (TrueLetter tl in consonants)
        {
            currentProbabilityCount_Consonants += tl.GetWeight();
            tl.ProbabilityTop = currentProbabilityCount_Consonants;
        }
        foreach (TrueLetter tl in vowels)
        {
            currentProbabilityCount_Vowels += tl.GetWeight();
            tl.ProbabilityTop = currentProbabilityCount_Vowels;
        }
    }

    private void Update()
    {
        if (!wv.GetPreppedStatus()) { return; }
        if (!isRandomPositionBeingGenerated && (nextTileDropPosition - previousTileDropPosition).magnitude < 0.1f)
        {
            StartCoroutine(UpdateRandomPositionOutsideOfMinRangeAndInsideArena_Coroutine());
            isRandomPositionBeingGenerated = true;
        }
        if (isNextTileReadyToBeDropped && Time.time >= timeForNextDrop)
        {
            DropLetterTile();
            timeForNextDrop = Time.time + timeBetweenDrops;
            isNextTileReadyToBeDropped = false;
            previousTileDropPosition = nextTileDropPosition;
        }

    }

    private void DropLetterTile()
    {
        GameObject newTile = Instantiate(letterTilePrefab, nextTileDropPosition, Quaternion.identity) as GameObject;
        TrueLetter randomLetter;
        if (dropsSinceLastVowel >= consonantsBetweenVowels)
        {
            randomLetter = ReturnWeightedRandomTrueLetter(false);
            dropsSinceLastVowel = 0;
        }
        else
        {
            randomLetter = ReturnWeightedRandomTrueLetter(true);
            dropsSinceLastVowel++;
        }
        
        LetterTile letterTile = newTile.GetComponent<LetterTile>();
        letterTile.Letter = randomLetter.GetLetter();
        letterTile.Power = randomLetter.GetPower();
        letterTile.Ability = randomLetter.GetAbility();
        letterTile.StartingLifetime = 10f + UnityEngine.Random.Range(-2f, 2f);
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
            if (attempts > 50)
            {
                break;
            }
        }
        while (!VerifyPositionIsNotNearWordMakers(randomPos) || !VerifyPositionIsNotNearLetters(randomPos));
        randomPos = GridHelper.SnapToGrid(randomPos, 1);
        return randomPos;
    }

    IEnumerator UpdateRandomPositionOutsideOfMinRangeAndInsideArena_Coroutine()
    {

        if (!ab)
        {
            ab = FindObjectOfType<ArenaBuilder>();
        }
        int attempts = 0;
        do
        {
            isRandomPositionTooNearOtherLetters = true;
            isRandomPositionTooNearWordMakers = true;
            randomPosition = ab.CreateRandomPointWithinArena();

            attempts++;
            if (attempts > 50)
            {
                break;
            }
            isRandomPositionTooNearOtherLetters = !VerifyPositionIsNotNearLetters(randomPosition);
            isRandomPositionTooNearWordMakers = !VerifyPositionIsNotNearWordMakers(randomPosition);
            
            yield return new WaitForEndOfFrame();
        }
        while (VerifyPositionIsNotNearWordMakers(randomPosition) || VerifyPositionIsNotNearLetters(randomPosition) || VerifyPositionIsReachable(randomPosition));
        randomPosition = GridHelper.SnapToGrid(randomPosition, 1);
        nextTileDropPosition = randomPosition;
        isRandomPositionBeingGenerated = false;
        isNextTileReadyToBeDropped = true;
    }

    private bool VerifyPositionIsNotNearWordMakers(Vector2 randomPos)
    {
        if (!sk)
        {
            sk = FindObjectOfType<SpeedKeeper>();
        }
        float currentMinDistance = Mathf.RoundToInt(sk.CurrentSpeed);
        currentMinDistance = Mathf.Clamp(currentMinDistance, universalMinDistanceToWordMaker, 10f);
        bool isOutsideMinRange = false;
        foreach (var wordmaker in wordMakers)
        {
            if ((wordmaker.transform.position - (Vector3)randomPos).magnitude < currentMinDistance)
            {
                isOutsideMinRange = true;
                break;
            }
        }
        return isOutsideMinRange;
    }
    private bool VerifyPositionIsNotNearLetters(Vector2 randomPos)
    {
        var coll = Physics2D.OverlapCircle(randomPos, minDistanceBetweenLetters, layerMask_Letter);
        if (coll)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool VerifyPositionIsReachable(Vector2 randomPos)
    {
        var coll = Physics2D.OverlapCircle(randomPos, 0.05f, layerMask_Impassable);
        if (coll)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private TrueLetter ReturnWeightedRandomTrueLetter(bool shouldBeConsonant)
    {
        if (shouldBeConsonant)
        {
            int rand = UnityEngine.Random.Range(0, currentProbabilityCount_Consonants);
            foreach (var tl in consonants)
            {
                if (rand <= tl.ProbabilityTop)
                {
                    //Debug.Log($"saw {rand}, choosing {tl.Letter} with top of {tl.ProbabilityTop}");
                    return tl;
                }
            }
        }
        else
        {
            int rand = UnityEngine.Random.Range(0, currentProbabilityCount_Vowels);
            foreach (var tl in vowels)
            {
                if (rand <= tl.ProbabilityTop)
                {
                    //Debug.Log($"saw {rand}, choosing {tl.Letter} with top of {tl.ProbabilityTop}");
                    return tl;
                }
            }
        }
        return null;
    }

    private void AddLetterToSpawnedLetterList(LetterTile newLetterTile)
    {
        OnLetterListModified?.Invoke(newLetterTile, true);
        letterTilesOnBoard.Add(newLetterTile);
        doesBoardHaveLettersAvailable = true;
    }

    public void RemoveLetterFromSpawnedLetterList(LetterTile letterTileToRemove)
    {
        OnLetterListModified?.Invoke(letterTileToRemove, false);
        letterTilesOnBoard.Remove(letterTileToRemove);
        if (letterTilesOnBoard.Count == 0)
        {
            doesBoardHaveLettersAvailable = false;
        }
    }

    public void DestroyAllLetters()
    {
        for (int i = 0; i < letterTilesOnBoard.Count; i++)
        {
            Destroy(letterTilesOnBoard[i].gameObject);
        }
        letterTilesOnBoard.Clear();
    }

    public List<LetterTile> FindAllReachableLetterTiles(Vector2 originPosition, float speed)
    {
        List<LetterTile> letterTilesInRange = new List<LetterTile>();

        foreach (var letterTile in letterTilesOnBoard)
        {
            float dist = (letterTile.transform.position - (Vector3)originPosition).magnitude;
            float timeRequiredToReach = dist * 1.5f / speed;
            if (timeRequiredToReach <= letterTile.LifetimeRemaining)
            {
                letterTilesInRange.Add(letterTile);
            }
        }
        return letterTilesInRange;
    }

    private void OnDestroy()
    {
        DestroyAllLetters();
    }
}
