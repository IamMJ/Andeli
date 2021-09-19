﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ArenaStarter : MonoBehaviour
{
    [SerializeField] GameObject arenaBuilderPrefab;
    GameObject ab;
    GameController gc;
    GameObject player;

    //param
    [SerializeField] float arenaTriggerRange;


    private void Start()
    {
        gc = FindObjectOfType<GameController>();
        player = gc.GetPlayer();

        GridModifier.UnknitAllGridGraphs(transform);
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

    public void RemoveArenaStarter()
    {
        GridModifier.ReknitAllGridGraphs(transform);
        Destroy(gameObject);
    }




}
