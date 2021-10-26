using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMemory : MonoBehaviour
{
    [SerializeField] List<string> knownKeywords = new List<string>(); //This is used to 
    //check if a particular bark or convo should be used or suppressed

    public Action<string> OnKeywordAdded;

    public float BaseEnergyRegenRate;
    public float BaseMoveSpeed;


    public bool CheckForPlayerKnowledgeOfARequiredKeyword(string testRequiredKeyword)
    {
        bool result = false;
        if (testRequiredKeyword == "")
        {
            result = true;
            return result;
        }
        foreach (var keyword in knownKeywords)
        {
            if (keyword == testRequiredKeyword)
            {
                result = true;
                return result;
            }
        }
        return result;
    }

    public bool CheckForPlayerKnowledgeOfABannedKeyword(string testBannedKeyword)
    {
        bool result = false;
        if (testBannedKeyword == "")
        {
            result = false;
            return result;
        }
        foreach (var keyword in knownKeywords)
        {
            if (keyword == testBannedKeyword)
            {
                result = true;
                return result;
            }
        }
        return result;
    }

    public void AddKeyword(string newKeyword)
    {
        knownKeywords.Add(newKeyword);
        OnKeywordAdded?.Invoke(newKeyword);

    }
}
