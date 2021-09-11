using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ArenaStarter : MonoBehaviour, IGridModifier
{
    [SerializeField] GameObject arenaBuilderPrefab;
    GameObject ab;
    GameController gc;
    GameObject player;
    GraphUpdateScene gus;

    //param
    [SerializeField] float arenaTriggerRange;


    private void Start()
    {
        gc = FindObjectOfType<GameController>();
        player = gc.GetPlayer();
        gus = GetComponent<GraphUpdateScene>();
        UnknitGridGraph();
    }

    private void Update()
    {
        if (gc.isInArena) { return; } // Don't let the player be in more than one arena
        if ((player.transform.position - transform.position).magnitude <= arenaTriggerRange)
        {
            ab = Instantiate(arenaBuilderPrefab, transform.position, transform.rotation) as GameObject;
            ArenaBuilder arenaBuilder = ab.GetComponent<ArenaBuilder>();
            arenaBuilder.SetArenaStarter(this);
            arenaBuilder.SetupArena(transform.position);
        }
    }

    public void ReknitGridGraph()
    {
        gus.setWalkability = true;
        gus.Apply();
    }

    public void UnknitGridGraph()
    {
        gus.setWalkability = false;
        gus.Apply();
    }

    public void RemoveArenaStarter()
    {
        ReknitGridGraph();
        Destroy(gameObject);
    }




}
