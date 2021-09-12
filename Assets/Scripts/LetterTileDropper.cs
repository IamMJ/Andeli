using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class LetterTileDropper : MonoBehaviour
{

    [SerializeField] TrueLetter[] trueLetters = null;
    [SerializeField] GameObject letterTilePrefab = null;
    [SerializeField] GameObject droppedLetterPrefab = null;

    public List<TrueLetter> consonants = new List<TrueLetter>();
    public List<TrueLetter> vowels = new List<TrueLetter>();
    WordValidater wv;
    ArenaBuilder ab;
    int layerMask_Impassable = 1 << 13;
    int layerMask_Letter = 1 << 9;
    public List<LetterTile> letterTilesOnBoard = new List<LetterTile>();
    List<Vector2> dropLocations = new List<Vector2>();

    public Action<LetterTile, bool> OnLetterListModified;  //True means letter was added, false means letter was removed.

    //param
    int numberOfLettersToDropPerWave = 3;
    float minDistanceBetweenLetters = 1.5f;
    int consonantsBetweenVowels = 1;
    float averageLifetimeOfLettersInWave = 20f;
    float averageTimeBetweenWaves = 5f;
    Vector2 fallVector = new Vector2(0, 10f);

    //state
    int dropsSinceLastVowel = 0;
    int currentProbabilityCount_Consonants = 0;
    int currentProbabilityCount_Vowels = 0;
    float timeForNextWave;

    public bool doesBoardHaveLettersAvailable { get; private set; } = true;

    Vector2 randomPosition = Vector2.zero;

    #region Startup
    private void Start()
    {
        // Rather than starting will all 26 True Letters, probably should pull this from the player every time an Arena is built
        // trueLetters = GetPlayersTrueLetters();

        wv = FindObjectOfType<WordValidater>();
        ab = FindObjectOfType<ArenaBuilder>();

        SeparateVowelsFromConsonants();
        GenerateProbabilityStarts();
        CreateLetterDropLocations();
        DropLettersAtDropLocations();
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

    #endregion

    private void Update()
    {
        if (!wv.GetPreppedStatus()) { return; }        
       
        if (Time.time >= timeForNextWave)
        {
            CreateLetterDropLocations();
            DropLettersAtDropLocations();
        }
    }

    private void CreateLetterDropLocations()
    {
        for (int i = 0; i < numberOfLettersToDropPerWave; i++)
        {
            dropLocations.Add(GetRandomPositionInsideArenaButAwayFromOtherDropLocations(i));
        }
    }

    private void DropLettersAtDropLocations()
    {
        timeForNextWave = Time.time + averageTimeBetweenWaves + UnityEngine.Random.Range(-1f, +1f);
        foreach (var dropLocation in dropLocations)
        {
            DropLetterTile(dropLocation);
        }
        dropLocations.Clear();      
        
    }

    #region Tiledrop Helpers
    private Vector2 GetRandomPositionInsideArenaButAwayFromOtherDropLocations(int currentDropLocationIndex)
    {
        int attempts = 0;
        bool isTooNearOtherDropLocation = false;
        bool isAnImpassablePosition = false;
        bool isTooNearAnExistingLetter = false;
        if (!ab)
        {
            ab = FindObjectOfType<ArenaBuilder>();
        }
        do
        {
            randomPosition = ab.CreateRandomPointWithinArena();
            attempts++;
            if (attempts > 50)
            {
                Debug.Log("too many attempts - break");
                break;
            }

            isTooNearOtherDropLocation = CheckRandomPositionAgainstOtherDropLocations(currentDropLocationIndex, randomPosition);
            isAnImpassablePosition = CheckRandomPositionAgainstImpassableTerrain(randomPosition);
            isTooNearAnExistingLetter = CheckRandomPositionAgainstExistingLettersOnBoard(randomPosition);
        }
        while (isTooNearOtherDropLocation || isAnImpassablePosition || isTooNearAnExistingLetter);
        return randomPosition;
    }

    #region Helpers
    /// <summary>
    /// This returns 'true' if the testPosition is inside of the MinDistanceBetweenLetters from a single drop
    /// location before it in the array. Returns 'false' if outside the MinDistanceBetweenLetters
    /// </summary>
    /// <param name="currentDropLocationIndex"></param>
    /// <param name="testPos"></param>
    /// <returns></returns>
    private bool CheckRandomPositionAgainstOtherDropLocations(int currentDropLocationIndex, Vector2 testPos)
    {
        bool isOutsideOfMinDistance = false;

        for (int i = 0; i < currentDropLocationIndex; i++)
        {
            if ((testPos - dropLocations[i]).magnitude <= minDistanceBetweenLetters)
            {
                isOutsideOfMinDistance = true;
                break;
            }
            else
            {
                continue;
            }
        }
        return isOutsideOfMinDistance;
    }
    private bool CheckRandomPositionAgainstImpassableTerrain(Vector2 randomPos)
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

    private bool CheckRandomPositionAgainstExistingLettersOnBoard(Vector2 testPos)
    {
        var coll = Physics2D.OverlapCircle(testPos, 0.05f, layerMask_Letter);
        if (coll)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void DropLetterTile(Vector2 dropPosition)
    {
        GameObject dropShadow = Instantiate(droppedLetterPrefab, dropPosition, Quaternion.identity);

        GameObject newTile = Instantiate(letterTilePrefab, dropPosition + fallVector, Quaternion.identity) as GameObject;
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
        letterTile.StartingLifetime = averageLifetimeOfLettersInWave + UnityEngine.Random.Range(-2f, 2f);
        letterTile.SetFallDistance(fallVector.magnitude);
        letterTile.SetLetterTileDropper(this);
        letterTile.AssignShadow(dropShadow.GetComponent<LetterTileDropShadow>());
        newTile.GetComponentInChildren<TextMeshPro>().text = randomLetter.GetLetter().ToString();
        //AddLetterToSpawnedLetterList(letterTile);  //Have the letter call this once it has landed for AI's sake

    }

    #endregion

    #endregion

    //private Vector2 GetRandomPositionOutsideOfMinRangeAndInsideArena()
    //{
    //    if (!ab)
    //    {
    //        ab = FindObjectOfType<ArenaBuilder>();
    //    }
    //    Vector2 randomPos;
    //    int attempts = 0;
    //    do
    //    {
    //        randomPos = ab.CreateRandomPointWithinArena();
    //        attempts++;
    //        if (attempts > 50)
    //        {
    //            break;
    //        }
    //    }
    //    while (!VerifyPositionIsNotNearWordMakers(randomPos) || !VerifyPositionIsNotNearLetters(randomPos));
    //    randomPos = GridHelper.SnapToGrid(randomPos, 1);
    //    return randomPos;
    //}

    //IEnumerator UpdateRandomPositionOutsideOfMinRangeAndInsideArena_Coroutine()
    //{

    //    if (!ab)
    //    {
    //        ab = FindObjectOfType<ArenaBuilder>();
    //    }
    //    int attempts = 0;
    //    do
    //    {
    //        isRandomPositionTooNearOtherLetters = true;
    //        isRandomPositionTooNearWordMakers = true;
    //        randomPosition = ab.CreateRandomPointWithinArena();

    //        attempts++;
    //        if (attempts > 50)
    //        {
    //            break;
    //        }
    //        isRandomPositionTooNearOtherLetters = !VerifyPositionIsNotNearLetters(randomPosition);
    //        isRandomPositionTooNearWordMakers = !VerifyPositionIsNotNearWordMakers(randomPosition);

    //        yield return new WaitForEndOfFrame();
    //    }
    //    while (VerifyPositionIsNotNearWordMakers(randomPosition) || VerifyPositionIsNotNearLetters(randomPosition) || VerifyPositionIsReachable(randomPosition));
    //    randomPosition = GridHelper.SnapToGrid(randomPosition, 1);
    //    nextTileDropPosition = randomPosition;
    //    isRandomPositionBeingGenerated = false;
    //    isNextTileReadyToBeDropped = true;
    //}

    //private bool VerifyPositionIsNotNearWordMakers(Vector2 randomPos)
    //{
    //    if (!sk)
    //    {
    //        sk = FindObjectOfType<SpeedKeeper>();
    //    }
    //    float currentMinDistance = Mathf.RoundToInt(sk.CurrentSpeed);
    //    currentMinDistance = Mathf.Clamp(currentMinDistance, universalMinDistanceToWordMaker, 10f);
    //    bool isOutsideMinRange = false;
    //    foreach (var wordmaker in wordMakers)
    //    {
    //        if ((wordmaker.transform.position - (Vector3)randomPos).magnitude < currentMinDistance)
    //        {
    //            isOutsideMinRange = true;
    //            break;
    //        }
    //    }
    //    return isOutsideMinRange;
    //}
    //private bool VerifyPositionIsNotNearLetters(Vector2 randomPos)
    //{
    //    var coll = Physics2D.OverlapCircle(randomPos, minDistanceBetweenLetters, layerMask_Letter);
    //    if (coll)
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}

    #region Public Methods
    public void RemoveLetterFromSpawnedLetterList(LetterTile letterTileToRemove)
    {
        OnLetterListModified?.Invoke(letterTileToRemove, false);
        letterTilesOnBoard.Remove(letterTileToRemove);
        if (letterTilesOnBoard.Count == 0)
        {
            doesBoardHaveLettersAvailable = false;
        }
    }

    public void AddLetterToSpawnedLetterList(LetterTile newLetterTile)
    {
        OnLetterListModified?.Invoke(newLetterTile, true);
        letterTilesOnBoard.Add(newLetterTile);
        doesBoardHaveLettersAvailable = true;
    }

    public void DestroyAllLetters()
    {
        Debug.Log("LTD asked to destroy all letters");
        int count = letterTilesOnBoard.Count;
        for (int i = 0; i < count; i++)
        {
            letterTilesOnBoard[i]?.DestroyThisLetterTile();
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

    #endregion
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





    private void OnDestroy()
    {
        DestroyAllLetters();
    }
}
