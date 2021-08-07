using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class WordValidater : MonoBehaviour
{
    [SerializeField] TextAsset wordListRaw = null;
    HashSet<string> wordListProcessed;
    string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    List<string> masterWordList;
    Dictionary<char, WordBand> wordListLetterIndex = new Dictionary<char, WordBand>(26);

    DebugHelper dh;
    PlayerMemory pm;

    void Start()
    {
        pm = FindObjectOfType<PlayerMemory>();
        dh = FindObjectOfType<DebugHelper>();
        var arr = wordListRaw.text.Split();
        wordListProcessed = new HashSet<string>(arr);
        masterWordList = new List<string>(arr);
        masterWordList.RemoveAll(x => x == "");

        PrepLetterIndex();

        //Debug.Log($"raw array contains {arr.Length} words");
        //Debug.Log($"word list hash contains {wordListProcessed.Count} words");
        //Debug.Log($"word list list contains {wordListAZ.Count} words");
        //dh.DisplayDebugLog($"Loaded {wordListProcessed.Count} words");
        //FindPossibleWordCountWithStubWord("ANGER");
    }

    private void PrepLetterIndex()
    {
        char[] alphabetAsChars = alphabet.ToCharArray();
        for (int i = 0; i < 26; i++)
        {
            StringComparison sc = new StringComparison(alphabetAsChars[i].ToString());
            int firstInstance = masterWordList.FindIndex(sc.CompareString);
            int lastInstance = masterWordList.FindLastIndex(sc.CompareString);
            int range = lastInstance - firstInstance;

            WordBand wordband = new WordBand(firstInstance, range, lastInstance);
            //Debug.Log($"adding {alphabetAsChars[i]} with a WordBand: {firstInstance},{range}");
            wordListLetterIndex.Add(alphabetAsChars[i], wordband);
        }
    }

    public WordBand GetWordBandForStartingChar(char startingChar)
    {
        return wordListLetterIndex[startingChar];
    }

    public List<string> GetMasterWordList()
    {
        return masterWordList;
    }

    public bool CheckWordValidity(string testWord, GameObject wordSubmitter)
    {
        if (!pm)
        {
            pm = FindObjectOfType<PlayerMemory>();
        }

        if (wordListProcessed.Contains(testWord))
        {
            //Debug.Log($"{testWord} is valid");
            //dh.DisplayDebugLog($"{testWord} is valid");
            if (wordSubmitter == pm.gameObject)
            {
                pm.IncrementWordCount();
            }

            return true;
        }
        else
        {
            pm.ResetConsecutiveWordCount();
            //Debug.Log($"{testWord} is invalid");
            //dh.DisplayDebugLog($"{testWord} is invalid");
            return false;
        }

    }

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
    public WordBand FindWordBandWithStubWord(string stubWord)
    {
        StringComparison sc = new StringComparison(stubWord);
        //Debug.Log($"Searching for {stubWord}, starting at {bandToSearch.StartIndex}, count: {bandToSearch.Range}");

        int actualStartIndex;
        int actualRange;
        int actualEndIndex;

        char[] stubWordAsChar = stubWord.ToCharArray();
        Debug.Log($"stubWordAsChar[0]: {stubWordAsChar[0]}");
        actualStartIndex = GetWordBandForStartingChar(stubWordAsChar[0]).StartIndex;
        actualRange = GetWordBandForStartingChar(stubWordAsChar[0]).Range;
        actualEndIndex = GetWordBandForStartingChar(stubWordAsChar[0]).EndIndex;
        //Debug.Log($"given search band is too big. Helped out. start: {actualStartIndex}, range: {actualRange}");


        //Debug.Log($"stub: {stubWord} start: {actualStartIndex}, range: {actualRange}");
        int firstInstance = masterWordList.FindIndex(actualStartIndex, actualRange, sc.CompareString);
        int lastInstance = masterWordList.FindLastIndex(actualEndIndex, actualRange, sc.CompareString);
        int possibleWords = lastInstance - firstInstance;

        WordBand wordBand = new WordBand(firstInstance, possibleWords, lastInstance);
        
        if (possibleWords > 0)
        {
            //Debug.Log($"{stubWord} has {possibleWords} possible words");
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
}


public class StringComparison
{
    char[] unchangingWordAsChars;
    public StringComparison(String newUnchangingWord)
    {
        unchangingWordAsChars = newUnchangingWord.ToCharArray();
    }

    public bool CompareString(String wordToTest)
    {
        bool isMatch = false;
        char[] wordToTestAsChars = wordToTest.ToCharArray();
        for (int i = 0; i < unchangingWordAsChars.Length; i++)
        {
            if (i > wordToTestAsChars.Length - 1)
            {
                isMatch = false;
                continue;
            }
            if (wordToTestAsChars[i] != unchangingWordAsChars[i])
            {
                isMatch = false;
                break;
            }
            else
            {
                isMatch = true;
                continue;
            }
        }
        return isMatch;
    }

}

