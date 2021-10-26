using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStep : ScriptableObject
{

    public TutorialTag.TagName targetTutorialTag = TutorialTag.TagName.Nothing;
    public enum completeStepCondition { HitOkay, MoveToLocation, EnterCombat}

    completeStepCondition completionCriteria = completeStepCondition.HitOkay;
    public string instruction = null;
    public Vector2 worldSpacePosition;
    public float arrowRotation;


}
