using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class WordValidater : MonoBehaviour
{
    [SerializeField] TextAsset wordListRaw = null;
    HashSet<string> wordListProcessed;

    List<string> masterWordList;

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

        //Debug.Log($"raw array contains {arr.Length} words");
        //Debug.Log($"word list hash contains {wordListProcessed.Count} words");
        //Debug.Log($"word list list contains {wordListAZ.Count} words");
        //dh.DisplayDebugLog($"Loaded {wordListProcessed.Count} words");
        //FindPossibleWordCountWithStubWord("ANGER");
    }

    public bool CheckWordValidity(string testWord)
    {
        if (!pm)
        {
            pm = FindObjectOfType<PlayerMemory>();
        }

        if (wordListProcessed.Contains(testWord))
        {
            //Debug.Log($"{testWord} is valid");
            dh.DisplayDebugLog($"{testWord} is valid");
            pm.IncrementWordCount();
            return true;
        }
        else
        {
            pm.ResetConsecutiveWordCount();
            //Debug.Log($"{testWord} is invalid");
            dh.DisplayDebugLog($"{testWord} is invalid");
            return false;
        }

    }

    public int FindPossibleWordCountWithStubWord(string stubWord)
    {
        StringComparison sc = new StringComparison(stubWord);

        int firstInstance = masterWordList.FindIndex(sc.CompareString);
        int lastInstance = masterWordList.FindLastIndex(sc.CompareString);
        int possibleWords = lastInstance - firstInstance;
        
        if (possibleWords > 0)
        {
            Debug.Log($"{stubWord} has {possibleWords} possible words");
            //Debug.Log($"first word: {masterWordList[firstInstance]}. last word: {masterWordList[lastInstance]}");
        }
        else
        {
            Debug.Log($"{stubWord} has no possible words!");
        }

        return possibleWords;
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

