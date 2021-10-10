using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Conversation")]

public class Conversation : ScriptableObject
{
    [SerializeField] ConversationStep[] convoSteps = null;

    ConversationStep currentConvoStep;    

    public ConversationStep GetConversationStepAtIndex(int index)
    {
        return convoSteps[index];
    }

    public int GetConversationStepLength()
    {
        return convoSteps.Length;
    }


}
