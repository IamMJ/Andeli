using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Bark")]

public class Bark : ScriptableObject
{
    [SerializeField] public string BarkText = "";
    [SerializeField] public float DisplayTime = 3f;
    [SerializeField] public Color DisplayColor = Color.black;
}
