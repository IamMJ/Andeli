using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WordValidater : MonoBehaviour
{
    [SerializeField] TextAsset wordListRaw = null;
    string wordList;
    DebugHelper dh;
    void Start()
    {
        dh = FindObjectOfType<DebugHelper>();
        wordList = wordListRaw.ToString();
        dh.DisplayDebugLog($"Loaded {wordList.Length} words");
    }

    public bool CheckWordValidity(string testWord)
    {
        if (wordList.Contains(testWord))
        {
            Debug.Log($"{testWord} is valid");
            dh.DisplayDebugLog($"{testWord} is valid");
            return true;
        }
        else
        {
            Debug.Log($"{testWord} is invalid");
            dh.DisplayDebugLog($"{testWord} is invalid");
            return false;
        }
    }
}
