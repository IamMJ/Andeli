using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class LetterTileDropper : MonoBehaviour
{

    [SerializeField] List<TrueLetter> trueLetters = null;
    [SerializeField] GameObject letterTilePrefab = null;
    [SerializeField] GameObject droppedLetterPrefab = null;

    public List<TrueLetter> consonants = new List<TrueLetter>();
    public List<TrueLetter> vowels = new List<TrueLetter>();

    
    WordValidater wv;
    ArenaBuilder ab;
    GameController gc;
    LetterMaskHolder lmh_Player; // the player's letter mask holder is needed to assign correct sprite/color/power/ability to each letter Tile
    EnemyProfile enemyProfile;
    int layerMask_Impassable = 1 << 13;
    int layerMask_Letter = 1 << 9;
    int layerMask_Player = 1 << 8;
    public List<LetterTile> letterTilesOnBoard = new List<LetterTile>();
    private List<LetterTile> allLettersDropped = new List<LetterTile>();
    [SerializeField] string lettersOnBoard = "";
    List<Vector2> dropLocations = new List<Vector2>();

    public Action<LetterTile, bool> OnLetterListModified;  //True means letter was added, false means letter was removed.

    // fixed param
    float minDistanceBetweenLetters = 1.7f;
    Vector2 fallVector = new Vector2(0, 10f);

    // changeable params
    float lifetimeOfLetter;
    float averageTimeBetweenWaves;
    int consonantsBetweenVowels;
    int numberOfLettersToDropPerWave ;
    string letterstoIgnore = "";
    float percentageOfLettersToDropAsMisty;
    int maxLettersOnBoard;

    //state
    bool shouldSeparateVowelsFromConsonants = false;
    int dropsSinceLastVowel = 0;
    int currentProbabilityCount_AllLetters = 0;
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
        gc = FindObjectOfType<GameController>();
        wv = FindObjectOfType<WordValidater>();
        ab = FindObjectOfType<ArenaBuilder>();
        enemyProfile = ab.GetEnemyInArena().GetComponent<SpellingStrategy>().GetEnemyProfile();
        lmh_Player = FindObjectOfType<GameController>().GetPlayer().GetComponent<LetterMaskHolder>();

        if (shouldSeparateVowelsFromConsonants)
        {
            SeparateVowelsFromConsonants();
        }

        GenerateProbabilityStarts();
        //CreateLetterDropLocations();
        //DropLettersAtDropLocations();
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
        if (shouldSeparateVowelsFromConsonants)
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
        else
        {
            foreach (TrueLetter tl in trueLetters)
            {
                currentProbabilityCount_AllLetters += tl.GetWeight();
                tl.ProbabilityTop = currentProbabilityCount_AllLetters;
            }
        }

    }

    #endregion

    private void Update()
    {
        if (gc.isPaused) { return; }
        if (!wv.GetPreppedStatus()) { return; }        
       
        if (Time.time >= timeForNextWave && lettersOnBoard.Length < maxLettersOnBoard)
        {

            CreateLetterDropLocations(ref dropLocations, numberOfLettersToDropPerWave);
            DropLettersAtDropLocations(ref dropLocations, false, 0);
            timeForNextWave = Time.time + averageTimeBetweenWaves; // + UnityEngine.Random.Range(-1f, +1f);
        }
    }

    private void CreateLetterDropLocations(ref List<Vector2> listToPopulate, int numberOfLettersToDrop)
    {
        for (int i = 0; i < numberOfLettersToDrop; i++)
        {
            listToPopulate.Add(GetRandomPositionInsideArenaButAwayFromOtherDropLocations(ref listToPopulate, i));
        }
    }

    private void DropLettersAtDropLocations(ref List<Vector2> targetDropLocations, bool isMystic, float mysticPower)
    {
        int limit = maxLettersOnBoard - lettersOnBoard.Length;

        foreach (var dropLocation in targetDropLocations)
        {
            limit--;
            if (limit <= 0) { break; }
            DropLetterTile(dropLocation, isMystic, mysticPower);
        }
        dropLocations.Clear();              
    }

    #region Tiledrop Helpers
    private Vector2 GetRandomPositionInsideArenaButAwayFromOtherDropLocations(ref List<Vector2> targetDropLocations, int currentDropLocationIndex)
    {
        int attempts = 0;
        bool isTooNearOtherDropLocation = false;
        bool isAnImpassablePosition = false;
        bool isTooNearAnExistingLetter = false;
        bool isTooNearThePlayer = false;
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

            isTooNearOtherDropLocation = CheckRandomPositionAgainstOtherDropLocations(ref targetDropLocations, currentDropLocationIndex, randomPosition);
            isAnImpassablePosition = CheckRandomPositionAgainstImpassableTerrain(randomPosition);
            isTooNearAnExistingLetter = CheckRandomPositionAgainstExistingLettersOnBoard(randomPosition);
            isTooNearThePlayer = CheckRandomPositionAgainstPlayerForMysticDrop(randomPosition);
        }
        while (isTooNearOtherDropLocation || isAnImpassablePosition || isTooNearAnExistingLetter || isTooNearThePlayer);
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
    private bool CheckRandomPositionAgainstOtherDropLocations(ref List<Vector2> targetDropLocations, int currentDropLocationIndex, Vector2 testPos)
    {
        bool isOutsideOfMinDistance = false;

        for (int i = 0; i < currentDropLocationIndex; i++)
        {
            if ((testPos - targetDropLocations[i]).magnitude <= minDistanceBetweenLetters)
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
        var coll = Physics2D.OverlapCircle(randomPos, 0.48f, layerMask_Impassable);
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

    private bool CheckRandomPositionAgainstPlayerForMysticDrop(Vector2 testPos)
    {
        var coll = Physics2D.OverlapCircle(testPos, 0.05f, layerMask_Player);
        if (coll)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void DropLetterTile(Vector2 dropPosition, bool isMystic, float mysticPower)
    {
        GameObject dropShadow = null;
        GameObject newTile = null;
        if (!isMystic)
        {
            dropShadow = Instantiate(droppedLetterPrefab, dropPosition, Quaternion.identity);
            newTile = Instantiate(letterTilePrefab, dropPosition + fallVector, Quaternion.identity) as GameObject;
        }
        else
        {
            newTile = Instantiate(letterTilePrefab, dropPosition, Quaternion.identity) as GameObject;
        }
        
        TrueLetter randomLetter;
        if (shouldSeparateVowelsFromConsonants)
        {
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
        }
        else
        {
            randomLetter = ReturnWeightedRandomTrueLetter();
        }

        if (isMystic)
        {
            LetterTile letterTile = newTile.GetComponent<LetterTile>();
            letterTile.IsMystic = true;
            letterTile.Letter = randomLetter.GetLetter();
            letterTile.Power_Player = 0;
            letterTile.Ability_Player = TrueLetter.Ability.Normal;
            letterTile.StartingLifetime = mysticPower;
            letterTile.IsFalling = false;
            letterTile.SetLetterTileDropper(this);
            newTile.GetComponentInChildren<TextMeshPro>().text = randomLetter.GetLetter().ToString();
        }
        else
        {
            LetterTile letterTile = newTile.GetComponent<LetterTile>();
            letterTile.Letter = randomLetter.GetLetter();

            LetterMask playerLM = lmh_Player.GetLetterMaskForTrueLetter(randomLetter);
            letterTile.Power_Player = playerLM.GetPower();
            letterTile.Ability_Player = playerLM.GetAbility();

            if (false) // lmh_Enemy)
            {
                //pull enemy powers from the Enemy Profile
            }
            else
            {
                letterTile.Power_Enemy = randomLetter.GetPower();
                letterTile.Ability_Enemy = randomLetter.GetAbility();
            }


            letterTile.StartingLifetime = lifetimeOfLetter + lifetimeOfLetter * UnityEngine.Random.Range(-0.3f, 0.3f);
            letterTile.SetFallDistance(fallVector.magnitude);
            letterTile.SetLetterTileDropper(this);
            letterTile.AssignShadow(dropShadow.GetComponent<LetterTileDropShadow>());
            newTile.GetComponentInChildren<TextMeshPro>().text = randomLetter.GetLetter().ToString();
            allLettersDropped.Add(letterTile);
            lettersOnBoard += letterTile.Letter;
        }

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

    public void SpawnMysticLetters(int count, float mysticPower)
    {
        List<Vector2> mysticDropPoints = new List<Vector2>(3);
        CreateLetterDropLocations(ref mysticDropPoints, 3);
        DropLettersAtDropLocations(ref mysticDropPoints, true, mysticPower);
        
    }
    public void RemoveLetterFromSpawnedLetterList(LetterTile letterTileToRemove)
    {
        OnLetterListModified?.Invoke(letterTileToRemove, false);
        letterTilesOnBoard.Remove(letterTileToRemove);
        if (letterTilesOnBoard.Count == 0)
        {
            doesBoardHaveLettersAvailable = false;
        }
        int letterToRemove = lettersOnBoard.IndexOf(letterTileToRemove.Letter);
        if (letterToRemove >= 0)
        {
            //Debug.Log($"should remove a {letterTileToRemove.Letter} at index {letterToRemove}");
            lettersOnBoard = lettersOnBoard.Remove(letterToRemove, 1);
        }

    }

    public void AddLetterToSpawnedLetterList(LetterTile newLetterTile)
    {

        letterTilesOnBoard.Add(newLetterTile);
        OnLetterListModified?.Invoke(newLetterTile, true);
        doesBoardHaveLettersAvailable = true;
    }

    public void RemoveLetterFromAllLettersSpawnedList(LetterTile letterTile)
    {
        allLettersDropped.Remove(letterTile);
    }

    public void DestroyAllLetters()
    {
        //foreach (var letter in allLettersDropped)
        //{
        //    letter.DestroyLetterTile();
        //}

        //for (int i = 0; i < allLettersDropped.Count; i++)
        //{
        //    allLettersDropped[i].DestroyLetterTile();
        //}

        for (int i = allLettersDropped.Count - 1; i >= 0; i--)
        {
            allLettersDropped[i].DestroyLetterTile();
        }
        letterTilesOnBoard.Clear();
        allLettersDropped.Clear();
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

    #region Public Arena Parameter Setting
    //float averageLifetimeOfLettersInWave = 20f;
    //float averageTimeBetweenWaves = 5f;
    //int consonantsBetweenVowels = 1;
    //int numberOfLettersToDropPerWave = 3;
    public void SetupArenaParameters_Lifetime(float letterLifetime)
    {
        lifetimeOfLetter = letterLifetime;
    }

    public void SetupArenaParameters_LettersInWave(int lettersInWave)
    {
        numberOfLettersToDropPerWave = lettersInWave;
    }
    public void SetupArenaParameters_LettersToIgnore(string lettersToIgnore)
    {
        this.letterstoIgnore = lettersToIgnore;
        RemoveBannedLetters();
    }
    public void SetupArenaParameters_LettersAsMisty(float percentageAsMisty)
    {
        percentageOfLettersToDropAsMisty = percentageAsMisty;
    }
    public void SetupArenaParameters_MaxLettersOnBoard(int maxLetters)
    {
        maxLettersOnBoard = maxLetters;
    }
    public void SetupArenaParameters_TimeBetweenWaves(float timeBetweenWaves)
    {
        averageTimeBetweenWaves = timeBetweenWaves;
    }


    #endregion

    #region Internal Methods
    /// <summary>
    /// This returns a random TrueLetter, but either a vowel or consonant depending on the argument.
    /// No argument provided will simply pull a random letter from all 26 letters as a group.
    /// </summary>
    /// <param name="shouldBeConsonant"></param>
    /// <returns></returns>
    private TrueLetter ReturnWeightedRandomTrueLetter(bool shouldBeConsonant)
    {
        TrueLetter chosenLetter = null;
        int attempts = 0;
        do
        {
            attempts++;
            if (attempts > 3)
            {
                break;
            }

            if (shouldBeConsonant)
            {
                int rand = UnityEngine.Random.Range(0, currentProbabilityCount_Consonants);
                foreach (var tl in consonants)
                {
                    if (rand <= tl.ProbabilityTop)
                    {
                        //Debug.Log($"saw {rand}, choosing {tl.GetLetter()} with top of {tl.ProbabilityTop}");
                        chosenLetter = tl;
                        break;
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
                        //Debug.Log($"saw {rand}, choosing {tl.GetLetter()} with top of {tl.ProbabilityTop}");
                        chosenLetter = tl;
                        break;
                    }
                }
            }

        }
        while (lettersOnBoard.Contains(chosenLetter.GetLetter().ToString()));

        return chosenLetter;
    }
    /// <summary>
    /// This method returns a random letter from all 26 letters. If an argument is provided, it will instead
    /// pull from either the vowels only or the consonants only.
    /// </summary>
    /// <param name="shouldBeConsonant"></param>
    /// <returns></returns>
    private TrueLetter ReturnWeightedRandomTrueLetter()
    {
        TrueLetter chosenLetter = null;
        int attempts = 0;
        do
        {
            attempts++;
            if (attempts > 3)
            {
                break;
            }

            int rand = UnityEngine.Random.Range(0, currentProbabilityCount_AllLetters);
            foreach (var tl in trueLetters)
            {
                if (rand <= tl.ProbabilityTop)
                {
                    //Debug.Log($"saw {rand}, choosing {tl.GetLetter()} with top of {tl.ProbabilityTop}");
                    chosenLetter = tl;
                    break;
                }
            }            
        }
        while (lettersOnBoard.Contains(chosenLetter.GetLetter().ToString()));

        return chosenLetter;
    }

    private void RemoveBannedLetters()
    {
        for (int i = trueLetters.Count - 1; i >= 0; i--)
        {
            string bannedLetter = trueLetters[i].GetLetter().ToString();
            if (letterstoIgnore.Contains(bannedLetter))
            {
                //Debug.Log($"removing {bannedLetter}");
                trueLetters.RemoveAt(i);
            }
        }
    }

    private void OnDestroy()
    {
        DestroyAllLetters();
    }

    #endregion
}
