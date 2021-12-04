using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordbuilderData : MonoBehaviour
{
    [SerializeField] Sprite mugShot = null;
    [SerializeField] string wordbuilderName = "";


    public Sprite GetMugShot()
    {
        return mugShot;
    }

    public string GetName()
    {
        return wordbuilderName;
    }
}
