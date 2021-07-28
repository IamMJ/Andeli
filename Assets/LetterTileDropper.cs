using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LetterTileDropper : MonoBehaviour
{
    [SerializeField] GameObject letterTilePrefab = null;
    string letterOptionsString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    char[] letterOptionsChar;

    //param
    float timeBetweenDrops = 2f;
    float distanceFromOrigin = 10f;

    //state
    float timeForNextDrop;

    private void Start()
    {
        timeForNextDrop = Time.time + timeBetweenDrops;
        letterOptionsChar = letterOptionsString.ToCharArray();
    }

    private void Update()
    {
        if (Time.time >= timeForNextDrop)
        {
            DropLetterTile();
            timeForNextDrop = Time.time + timeBetweenDrops;
            //Debug.Log($"current time: {Time.time}. Next drop at: {timeForNextDrop}");
        }
    }

    private void DropLetterTile()
    {
        float randomX = UnityEngine.Random.Range(-distanceFromOrigin, distanceFromOrigin);
        float randomY = UnityEngine.Random.Range(-distanceFromOrigin, distanceFromOrigin);
        Vector2 randomPos = new Vector2(randomX, randomY);
        randomPos = GridHelper.SnapToGrid(randomPos, 1);

        GameObject newTile = Instantiate(letterTilePrefab, randomPos, Quaternion.identity) as GameObject;

        int rand = UnityEngine.Random.Range(0, letterOptionsChar.Length);
        newTile.GetComponent<LetterTile>().Letter = letterOptionsChar[rand];
        newTile.GetComponentInChildren<TextMeshPro>().text = letterOptionsChar[rand].ToString();
    }
}
