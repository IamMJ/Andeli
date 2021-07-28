using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterCollector : MonoBehaviour
{
    WordBoxDriver wbd;
    void Start()
    {
        wbd = FindObjectOfType<WordBoxDriver>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        LetterTile letterTile;
        if (collision.gameObject.TryGetComponent<LetterTile>(out letterTile))
        {
            wbd.AddLetter(letterTile.Letter);
            Destroy(collision.gameObject);
        }
    }
}
