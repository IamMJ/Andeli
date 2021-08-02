using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WordValidater : MonoBehaviour
{
    [SerializeField] TextAsset wordListRaw = null;
    HashSet<string> wordListProcessed;
    DebugHelper dh;
    void Start()
    {
        dh = FindObjectOfType<DebugHelper>();
        var arr = wordListRaw.text.Split();
        wordListProcessed = new HashSet<string>(arr);
        //dh.DisplayDebugLog($"Loaded {wordListProcessed.Count} words");
    }

    public bool CheckWordValidity(string testWord)
    {
        if (wordListProcessed.Contains(testWord))
        {
            //Debug.Log($"{testWord} is valid");
            dh.DisplayDebugLog($"{testWord} is valid");
            return true;
        }
        else
        {
            //Debug.Log($"{testWord} is invalid");
            dh.DisplayDebugLog($"{testWord} is invalid");
            return false;
        }
    }
}
