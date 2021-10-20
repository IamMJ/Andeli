using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class WordValidater : MonoBehaviour
{
    [SerializeField] TextAsset wordListRaw = null;
    HashSet<string> masterWordHashSet;
    string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    List<string> masterWordList;
    Dictionary<char, WordBand> TOC_1Deep = new Dictionary<char, WordBand>();
    Dictionary<char, Dictionary<char, WordBand>> TOC_2Deep = new Dictionary<char, Dictionary<char, WordBand>>(26);
    public struct WordBand
    {
        public WordBand(int startIndex, int range, int endIndex)
        {
            StartIndex = startIndex;
            Range = range;
            EndIndex = endIndex;
        }

        public int StartIndex;
        public int Range;
        public int EndIndex;
    }

    DebugHelper dh;
    WordMakerMemory pm;


    //state
    [SerializeField] bool isPrepped = false;

    void Awake()
    {
        int count = FindObjectsOfType<GameController>().Length;
        if (count > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }


    void Start()
    {
        pm = FindObjectOfType<WordMakerMemory>();
        dh = FindObjectOfType<DebugHelper>();
        var arr = wordListRaw.text.Split();
        masterWordHashSet = new HashSet<string>(arr);
        masterWordList = new List<string>(arr);
        masterWordList.RemoveAll(x => x == "");

        StartCoroutine(PrepTableOfContents_Coroutine());

        //Debug.Log($"raw array contains {arr.Length} words");
        //Debug.Log($"word list hash contains {wordListProcessed.Count} words");
        //Debug.Log($"word list list contains {wordListAZ.Count} words");
        //dh.DisplayDebugLog($"Loaded {wordListProcessed.Count} words");
    }

    #region Public Methods
    public bool CheckWordValidity(string testWord)
    {
        if (masterWordHashSet.Contains(testWord))
        {
            return true;
        }
        else
        {
            return false;
        }


    }


    public WordBand FindWordBandWithStubWord(string stubWord)
    {
        StringComparison sc = new StringComparison(stubWord);
        //Debug.Log($"Searching for {stubWord}, starting at {bandToSearch.StartIndex}, count: {bandToSearch.Range}");

        int actualStartIndex = 0;
        int actualRange = 123;
        int actualEndIndex = 122;

        char[] stubWordAsChar = stubWord.ToCharArray();


        if (stubWordAsChar.Length < 2)
        {
            WordBand wb = GetWordBandForStartingChar(stubWordAsChar[0]);
            actualStartIndex = wb.StartIndex;
            actualRange = wb.Range;
            actualEndIndex = wb.EndIndex;
        }
        if (stubWordAsChar.Length >= 2)
        {
            WordBand wb = GetWordBandForStartingChar(stubWordAsChar[0], stubWordAsChar[1]);
            actualStartIndex = wb.StartIndex;
            actualRange = wb.Range;
            actualEndIndex = wb.EndIndex;
        }

        if (actualStartIndex < 0) //Then there are no instances of this 2-letter stub in TOC_2Deep
        {
            WordBand wb = new WordBand(0, 0, 0);
            return wb;
        }

        int firstInstance = masterWordList.FindIndex(actualStartIndex, actualRange, sc.CompareString);
        int lastInstance = masterWordList.FindLastIndex(actualEndIndex, actualRange, sc.CompareString);
        int possibleWords = lastInstance - firstInstance;

        WordBand wordBand = new WordBand(firstInstance, possibleWords, lastInstance);

        if (possibleWords > 0)
        {
            // Debug.Log($"{stubWord} has {possibleWords} possible words");
            //Debug.Log($"first word: {masterWordList[firstInstance]}. last word: {masterWordList[lastInstance]}");
        }
        else
        {
            //Debug.Log($"{stubWord} has no possible words!");
        }

        return wordBand;
    }

    public int GetMasterWordListCount()
    {
        return masterWordList.Count;
    }

    public bool GetPreppedStatus()
    {
        return isPrepped;
    }

    #endregion

    #region Private Methods
    IEnumerator PrepTableOfContents_Coroutine()
    {
        float startTime = Time.time;
        char[] alphabetAsChars = alphabet.ToCharArray();
        int startIndex = 0;

        //Create the 1-deep TOC
        for (int k = 0; k < 26; k++)
        {
            StringComparison sc = new StringComparison(alphabetAsChars[k].ToString());
            int firstInstance = masterWordList.FindIndex(startIndex, sc.CompareString);
            int lastInstance = masterWordList.FindLastIndex(sc.CompareString);
            int range = lastInstance - firstInstance;
            startIndex = lastInstance + 1;

            WordBand wordband = new WordBand(firstInstance, range, lastInstance);
            TOC_1Deep.Add(alphabetAsChars[k], wordband);
            if (k % 2 > 0)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        //Create the volume of 2-deep subTOCs
        for (int j = 0; j < 26; j++)
        {
            Dictionary<char, WordBand> subTOC = new Dictionary<char, WordBand>(26);
            WordBand momentWB = GetWordBandForStartingChar(alphabetAsChars[j]);
            //Debug.Log($"moment WB: {momentWB.StartIndex}, {momentWB.Range}");

            for (int i = 0; i < 26; i++)
            {
                string testStub = alphabetAsChars[j].ToString() + alphabetAsChars[i].ToString();
                //Debug.Log($"test word: {testStub}");
                StringComparison sc = new StringComparison(testStub);
                int firstInstance = masterWordList.FindIndex(momentWB.StartIndex, momentWB.Range, sc.CompareString);
                int lastInstance = masterWordList.FindLastIndex(momentWB.EndIndex, momentWB.Range, sc.CompareString);
                int range = lastInstance - firstInstance;

                WordBand wordband = new WordBand(firstInstance, range, lastInstance);
                //Debug.Log($"adding subTOC {alphabetAsChars[j]}, {alphabetAsChars[i]} with a WordBand: {firstInstance},{range}");
                subTOC.Add(alphabetAsChars[i], wordband);
                if (i % 2 > 0)
                {
                    yield return new WaitForEndOfFrame();
                }
            }

            TOC_2Deep.Add(alphabetAsChars[j], subTOC);
            yield return new WaitForEndOfFrame();
        }
        isPrepped = true;
        Debug.Log($"finished prep: {Time.time - startTime} seconds");
    }

    private WordBand GetWordBandForStartingChar(char startingChar)
    {
        //Debug.Log($"Invoked 1 Deep Search for {startingChar}, which yields: {TOC_1Deep[startingChar].StartIndex}, {TOC_1Deep[startingChar].Range}");
        return TOC_1Deep[startingChar];
    }

    private WordBand GetWordBandForStartingChar(char startingChar, char secondChar)
    {
        //Debug.Log($"Invoked 2 Deep Search for {startingChar},{secondChar}");
        Dictionary<char, WordBand> subTOCtoPull = TOC_2Deep[startingChar];
        return subTOCtoPull[secondChar];
    }

    #endregion

}


public class StringComparison : IComparer<string>
{
    char[] unchangingWordAsChars;
    public StringComparison(String newUnchangingWord)
    {
        unchangingWordAsChars = newUnchangingWord.ToCharArray();
    }

    public int Compare(string stubWord, string wordFromDictionary)
    {
        char[] stubWordAsChars = stubWord.ToCharArray();
        char[] dictionaryWordAsChars = wordFromDictionary.ToCharArray();

        if (stubWordAsChars[1] < dictionaryWordAsChars[1])
        {
            Debug.Log($"{stubWordAsChars[1]} is before {dictionaryWordAsChars[1]}, returned -1");
            return -1;
        }
        if (stubWordAsChars[1] > dictionaryWordAsChars[1])
        {
            Debug.Log($"{stubWordAsChars[1]} is after {dictionaryWordAsChars[1]}, returned 1");
            return 1;
        }
        else
        {
            Debug.Log($"{stubWordAsChars[1]} is same as {dictionaryWordAsChars[1]}, returned 0");
            return 0;
        }

        //for (int i = 0; i < 20; i++)
        //{
        //    if (i >= stubWord.Length-1 || i >= wordFromDictionary.Length-1)
        //    {
        //        break;
        //    }
        //    if (stubWordAsChars[i] == dictionaryWordAsChars[i])
        //    {
        //        value = 0;
        //        continue;
        //    }
        //    if (stubWordAsChars[i] < dictionaryWordAsChars[i])
        //    {
        //        value = -1;
        //        break;
        //    }
        //    if (stubWordAsChars[i] > dictionaryWordAsChars[i])
        //    {
        //        value = 1;
        //        break;
        //    }
        //}
        //Debug.Log($"compared {wordFromDictionary} to {stubWord}. Returned {value}. Neg means x is earlier than y.");
        //return value;
    }

    public bool CompareString(String wordToTest)
    {
        bool isMatch = false;
        char[] wordToTestAsChars = wordToTest.ToCharArray();
        for (int i = 0; i < unchangingWordAsChars.Length; i++)
        {
            if (i > wordToTestAsChars.Length-1)
            {
                isMatch = false; // The word to test is shorter than the unchanging word, so not a match.
                continue;
            }
            if (wordToTestAsChars[i] != unchangingWordAsChars[i])
            {
                isMatch = false;
                break; //First letters don't match, so don't even look at second letter.
            }
            else //Implied: if each word still has remaining letters, and the currently evaluated letters are equal, matched, keep going.
            {
                isMatch = true;
                continue;
            }
        }
        return isMatch;
    }

    


}

