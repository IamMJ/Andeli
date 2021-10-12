using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
[RequireComponent (typeof(ArenaSettingHolder))]
public class ArenaStarter : MonoBehaviour
{
    [SerializeField] GameObject arenaBuilderPrefab = null;
    [SerializeField] GameObject arenaBriefMenuPrefab = null;
    [SerializeField] Sprite inactiveSprite = null;
    Sprite activeSprite;
    [SerializeField] public GameObject ArenaEnemyPrefab = null;

    ArenaSettingHolder ash;
    GameObject ab;
    GameController gc;
    GameObject player;
    GameObject arenaBrief;
    SpriteRenderer sr;
    Animator anim;

    //param
    [SerializeField] bool canBeDestroyed = true;
    [SerializeField] float arenaTriggerRange;
    float timeBetweenPlayerResponses = 10f;

    //state
    bool isActivated = true;
    float timeToBecomeResponsiveToPlayer = 0;


    private void Start()
    {

        gc = FindObjectOfType<GameController>();
        gc.OnGameStart += HandleOnGameStart;
        player = gc.GetPlayer();
        ash = GetComponent<ArenaSettingHolder>();
        GridModifier.UnknitAllGridGraphs(transform);
        sr = GetComponent<SpriteRenderer>();
        activeSprite = sr.sprite;
        anim = GetComponent<Animator>();
    }

    private void HandleOnGameStart()
    {
        player = gc.GetPlayer();
    }

    private void Update()
    {
        if (!isActivated) { return; } // don't do anything if inactive (ie, defeated)
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

    public void DeactivateArenaStarter()
    {
        //GridModifier.ReknitAllGridGraphs(transform);
        if (canBeDestroyed)
        {
            isActivated = false;
            if (anim)
            {
                anim.enabled = false;
            }

            sr.sprite = inactiveSprite;
        }
        else
        {
            timeToBecomeResponsiveToPlayer = Time.time + timeBetweenPlayerResponses;
        }

        //gameObject.SetActive(false);
    }

    public void ActivateArenaStarter()
    {
        //gameObject.SetActive(true);
        isActivated = true;
        sr.sprite = activeSprite;
        if (anim)
        {
            anim.enabled = true;
        }

        //GridModifier.UnknitAllGridGraphs(transform);
    }

    private void OnDestroy()
    {
        gc.OnGameStart -= HandleOnGameStart;
    }




}
