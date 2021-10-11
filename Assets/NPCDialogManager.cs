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

    NoticeMeDriver noticeMe;
    GameController gc;
    NPC_Brain brain;
    GameObject player;
    ConversationPanelDriver cpd;

    //param
    float timeBetweenBarks = 4;
    float barkLifetime = 3;
    float conversationRange = 1.5f;
    [SerializeField] public bool CanSpeak = true;

    //state
    [SerializeField] List<string> knownKeywords = new List<string>(); //This is used to 
    //check if a particular bark or convo should be used or suppressed

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
        
        if (brain.requestedToHalt && noticeMe.gameObject.activeSelf && !gc.isInArena && !gc.isPaused && !cpd.isDisplayed)
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
                    Debug.Log("start conv");
                    cpd.InitalizeConversationPanel(possibleConversations[0], this);
                    possibleConversations.RemoveAt(0);
                    noticeMe.ToggleNoticeMe(false);
                }
                
            }
        }       
        
    }

    #region Public Methods
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

    public void AddKeyword(string newKeyword)
    {
        knownKeywords.Add(newKeyword);
    }

    #endregion
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

    private bool CheckForKnowledgeOfKeyword(string testKeyword)
    {
        bool result = false;
        foreach (var keyword in knownKeywords)
        {
            if (keyword == testKeyword)
            {
                result = true;
                return result;
            }
        }
        return result;
    }

    private Conversation CheckForOpenConversationBasedOnCurrentKeyword()
    {
        foreach (var convo in possibleConversations)
        {
            string testKeyword = convo.RequiredKeywordToStart;
            if (CheckForKnowledgeOfKeyword(testKeyword))
            {
                return convo;
            }
        }
        return null;
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
