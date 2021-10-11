using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Conversation")]

public class Conversation : ScriptableObject
{
    [SerializeField] public string RequiredKeywordToStart = "";
    [SerializeField] public string KeywordAtCompletion = "";
    [SerializeField] ConversationStep[] convoSteps = null;


    public ConversationStep GetConversationStepAtIndex(int index)
    {
        if (index < convoSteps.Length)
        {
            return convoSteps[index];
        }
        else
        {
            return null;
        }

    }

    public int GetConversationStepLength()
    {
        return convoSteps.Length;
    }


}
