using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "TutorialStep")]

public class TutorialStep : ScriptableObject
{
    public enum PositionAnchor { relativeToTMP, worldSpace};

    public string instruction = null;
    public Vector2 position;
    public PositionAnchor positionAnchor;
    public float arrowRotation;
}
