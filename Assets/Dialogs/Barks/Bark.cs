using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Bark")]

public class Bark : ScriptableObject
{
    [SerializeField] public string KeywordToShowFor = ""; // this bark will only show if the player has this Keyword
    [SerializeField] public string KeywordToHideFrom = ""; // this bark will always hide if the player has this Keyword
    [SerializeField] public string BarkText = "";
    [SerializeField] public float DisplayTime = 3f;
    [SerializeField] public Color DisplayColor = Color.black;
}
