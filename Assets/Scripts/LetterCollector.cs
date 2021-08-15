using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterCollector : MonoBehaviour
{
    //init
    WordBuilder wbd;
    PlayerInput pi;
    PowerMeter pm;

    void Start()
    {
        wbd = FindObjectOfType<WordBuilder>();
        pi = GetComponent<PlayerInput>();
        pm = FindObjectOfType<PowerMeter>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (pi.timeSpentLongPressing > 0) { return; } //Don't pick up letters if pressing the screen. Prevents accidental misspells.
        LetterTile letterTile;
        if (collision.gameObject.TryGetComponent<LetterTile>(out letterTile))
        {
            wbd.AddLetter(letterTile);
            Destroy(collision.gameObject);
        }
    }
}
