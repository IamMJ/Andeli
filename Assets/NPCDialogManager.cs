using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogManager : MonoBehaviour
{
    //init
    [SerializeField] Bark[] possibleBarks = null;
    [SerializeField] Bark[] possibleReplyBarks = null;
    [SerializeField] List<Conversation> possibleConversations = new List<Conversation>();
    [SerializeField] GameObject barkPrefab = null;
    [SerializeField] GameObject noticeMePrefab = null;

    public NoticeMeDriver noticeMe;
    GameController gc;
    NPC_Brain brain;
    GameObject player;
    public ConversationPanelDriver cpd;

    //param
    float timeBetweenBarks = 4;
    float barkLifetime = 3;
    float conversationRange = 1.5f;
    [SerializeField] public bool CanSpeak = true;

    //state
    float timeForNextBark;
    BarkShell currentBark;


    // Start is called before the first frame update
    void Start()
    {
        brain = GetComponent<NPC_Brain>();
        cpd = FindObjectOfType<ConversationPanelDriver>();
        gc = FindObjectOfType<GameController>();
        player = gc.GetPlayer();
        if (possibleConversations.Count > 0)
        {
            ActivateNoticeMe();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > timeForNextBark && gc.isInGame && !gc.isInArena)
        {
            UpdateBark();
        }
        
        if (noticeMe && brain.requestedToHalt && !gc.isInArena && !gc.isPaused && !cpd.isDisplayed)
        {
            if (!player)
            {
                player = gc.GetPlayer();
            }
            float dist = (player.transform.position - transform.position).magnitude;
            if (dist < conversationRange)
            {
                if (cpd.ClaimConversationPanelDriverIfUnused(this))
                {
                    cpd.InitalizeConversationPanel(possibleConversations[0], this);
                    possibleConversations.RemoveAt(0);
                    noticeMe.ToggleNoticeMe(false);
                }
                
            }
        }       
        
    }

    private void UpdateBark()
    {
        int rand = UnityEngine.Random.Range(0, possibleBarks.Length);
        Bark bark = possibleBarks[rand];
        if (!currentBark)
        {
            currentBark = Instantiate(barkPrefab).GetComponent<BarkShell>();
        }
        currentBark?.ActivateBark(bark, transform);

        timeForNextBark = Time.time + timeBetweenBarks + bark.DisplayTime;
    }

    public void ProvideReplyBarkToPlayer()
    {
        int rand = UnityEngine.Random.Range(0, possibleReplyBarks.Length);
        Bark bark = possibleReplyBarks[rand];
        if (!currentBark)
        {
            currentBark = Instantiate(barkPrefab).GetComponent<BarkShell>();
        }
        currentBark?.ActivateBark(bark, transform);

        timeForNextBark = Time.time + timeBetweenBarks + bark.DisplayTime;
    }


    private void ActivateNoticeMe()
    {
        if (!noticeMe)
        {
            noticeMe = Instantiate(noticeMePrefab, transform).GetComponent<NoticeMeDriver>();
        }
        else
        {
            noticeMe.ToggleNoticeMe(true);
        }
    }
}
