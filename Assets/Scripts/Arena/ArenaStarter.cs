using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
[RequireComponent (typeof(ArenaSettingHolder))]
public class ArenaStarter : MonoBehaviour
{
    [SerializeField] GameObject arenaBuilderPrefab = null;
    [SerializeField] Sprite inactiveSprite = null;
    [SerializeField] AudioClip defeatedSound = null;
    Sprite activeSprite;
    [SerializeField] public GameObject ArenaEnemyPrefab = null;

    Librarian lib;
    BriefPanel bp;
    ArenaSettingHolder ash;
    GameObject ab;
    GameController gc;
    GameObject player;
    SpriteRenderer sr;
    Animator anim;
    PlayerMemory pm;
    NPCDialogManager ndm;
    AudioSource auso;

    //param
    [SerializeField] string[] keywordsRequiredForCombat = null;
    [SerializeField] string keywordGivenUponVictory = null;
    [SerializeField] string keywordGivenUponDefeat = null;
    [SerializeField] bool canBeDestroyed = true;
    [SerializeField] GameObject arenaCentroid = null;
    Vector2 centroidOffset = new Vector2(1, -1);
    float arenaTriggerRange = 1.5f;
    float timeBetweenPlayerResponses = 3f;

    //state
    Vector2 locationToReturnPlayerTo;
    bool isActivated = true;
    float timeToBecomeResponsiveToPlayer = 0;


    private void Start()
    {
        lib = Librarian.GetLibrarian();

        bp = lib.ui_Controller.briefPanel.GetComponent<BriefPanel>();
        gc = FindObjectOfType<GameController>();
        gc.OnGameStart += HandleOnGameStart;

        ash = GetComponent<ArenaSettingHolder>();
        GridModifier.UnknitAllGridGraphs(transform);
        sr = GetComponent<SpriteRenderer>();
        activeSprite = sr.sprite;
        anim = GetComponent<Animator>();
        pm = GetComponent<PlayerMemory>();
        ndm = GetComponent<NPCDialogManager>();
        auso = GetComponent<AudioSource>();
    }

    private void HandleOnGameStart()
    {
        player = gc.GetPlayer();
        pm = player.GetComponent<PlayerMemory>();
        arenaCentroid.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void Update()
    {
        if (!isActivated) { return; } // don't do anything if inactive (ie, defeated)
        if (!gc.isInGame) { return; } // don't do anything if not in the game
        if (gc.isInArena) { return; } // Don't let the player be in more than one arena
        if (Time.time < timeToBecomeResponsiveToPlayer) { return; }
        if ((player.transform.position - transform.position).magnitude <= arenaTriggerRange)
        {
            bool hasRequiredKeywords = true;
            foreach (var keyword in keywordsRequiredForCombat)
            {
                hasRequiredKeywords = pm.CheckForPlayerKnowledgeOfARequiredKeyword(keyword);
                if (hasRequiredKeywords == false)
                {
                    break;
                }
            }

            if (hasRequiredKeywords)
            {
                lib.ui_Controller.SetContext(UI_Controller.Context.Brief);
                bp.PopulateBriefPanel(this, ash.arenaSetting);
                timeToBecomeResponsiveToPlayer = Time.time + timeBetweenPlayerResponses;
            }
            else
            {
                Debug.Log($"player is missing at least one keyword required to fight here");
            }

        }
    }

    public void RetreatFromArena()
    {
        lib.ui_Controller.SetContext(UI_Controller.Context.Overworld);
    }

    public void StartArena()
    {
        lib.ui_Controller.SetContext(UI_Controller.Context.Combat);

        locationToReturnPlayerTo = player.transform.position;
        player.transform.position = arenaCentroid.transform.position + (Vector3)centroidOffset;
        player.GetComponent<Movement>().HaltPlayerMovement();
        player.GetComponent<PlayerInput>().HaltPlayerMovement();

        ab = Instantiate(arenaBuilderPrefab, arenaCentroid.transform.position, transform.rotation) as GameObject;
        ArenaBuilder arenaBuilder = ab.GetComponent<ArenaBuilder>();
        arenaBuilder.SetArenaStarter(this, ash);
        arenaBuilder.SetupArena(arenaCentroid);


    }

    public void DeactivateArenaStarter(bool didPlayerWin)
    {
        //GridModifier.ReknitAllGridGraphs(transform);
        if (canBeDestroyed && didPlayerWin)
        {
            isActivated = false;
            if (anim)
            {
                anim.enabled = false;
            }

            if (ndm)
            {
                ndm.enabled = false;
            }
            auso.PlayOneShot(defeatedSound);
            sr.sprite = inactiveSprite;
        }
        else
        {
            timeToBecomeResponsiveToPlayer = Time.time + timeBetweenPlayerResponses;
        }

        if (didPlayerWin)
        {
            if (keywordGivenUponVictory != "")
            {
                pm.AddKeyword(keywordGivenUponVictory);   
            }
        }
        else
        {
            if (keywordGivenUponDefeat != "")
            {
                pm.AddKeyword(keywordGivenUponDefeat);
            }
        }

        player.transform.position = locationToReturnPlayerTo;
        player.GetComponent<Movement>().HaltPlayerMovement();
        player.GetComponent<PlayerInput>().HaltPlayerMovement();

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
        if (ndm)
        {
            ndm.enabled = true;
        }
        //GridModifier.UnknitAllGridGraphs(transform);
    }

    private void OnDestroy()
    {
        gc.OnGameStart -= HandleOnGameStart;
    }




}
