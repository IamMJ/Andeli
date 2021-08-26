using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaStarter : MonoBehaviour
{
    [SerializeField] GameObject arenaBuilderPrefab;
    GameObject ab;
    GameController gc;

    private void Start()
    {
        gc = FindObjectOfType<GameController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (gc.isInArena) { return; } // Don't let the player be in more than one arena
        if (collision.gameObject.layer == 8) // Player must have collided with it
        {
            ab = Instantiate(arenaBuilderPrefab, transform.position, transform.rotation) as GameObject;
            ab.GetComponent<ArenaBuilder>().SetupArena(transform.position);
        }
    }


}
