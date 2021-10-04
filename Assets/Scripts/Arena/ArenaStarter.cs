using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
[RequireComponent (typeof(ArenaSettingHolder))]
public class ArenaStarter : MonoBehaviour
{
    [SerializeField] GameObject arenaBuilderPrefab = null;
    [SerializeField] GameObject arenaBriefMenuPrefab = null;

    ArenaSettingHolder ash;
    GameObject ab;
    GameController gc;
    GameObject player;
    GameObject arenaBrief;

    //param
    [SerializeField] float arenaTriggerRange;
    float timeBetweenPlayerResponses = 10f;

    //state
    float timeToBecomeResponsiveToPlayer = 0;


    private void Start()
    {
        gc = FindObjectOfType<GameController>();
        gc.OnGameStart += HandleOnGameStart;
        player = gc.GetPlayer();
        ash = GetComponent<ArenaSettingHolder>();
        GridModifier.UnknitAllGridGraphs(transform);
    }

    private void HandleOnGameStart()
    {
        player = gc.GetPlayer();
    }

    private void Update()
    {
        if (!gc.isInGame) { return; } // don't do anything if not in the game
        if (gc.isInArena) { return; } // Don't let the player be in more than one arena
        if (Time.time < timeToBecomeResponsiveToPlayer) { return; }
        if ((player.transform.position - transform.position).magnitude <= arenaTriggerRange)
        {
            if (!arenaBrief)
            {
                arenaBrief = Instantiate(arenaBriefMenuPrefab);
                arenaBrief.GetComponent<ArenaBriefMenuDriver>().SetupArenaBriefMenu(
                    this, ash.arenaSetting.briefScreenIcon, ash.arenaSetting.briefScreenText);
            }
            arenaBrief.SetActive(true);
            gc.PauseGame();
            timeToBecomeResponsiveToPlayer = Time.time + timeBetweenPlayerResponses;
        }
    }

    public void RetreatFromArena()
    {
        arenaBrief.SetActive(false);
        gc.ResumeGameSpeed(false);
    }

    public void StartArena()
    {
        arenaBrief.SetActive(false);
        ab = Instantiate(arenaBuilderPrefab, transform.position, transform.rotation) as GameObject;
        ArenaBuilder arenaBuilder = ab.GetComponent<ArenaBuilder>();
        arenaBuilder.SetArenaStarter(this, ash);
        arenaBuilder.SetupArena(transform.position);
        gc.ResumeGameSpeed(false);
    }

    public void RemoveArenaStarter()
    {
        GridModifier.ReknitAllGridGraphs(transform);
        gc.OnGameStart -= HandleOnGameStart;
        Destroy(gameObject);
    }




}
