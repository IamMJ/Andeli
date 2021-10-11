using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogManager : MonoBehaviour
{
    //init
    [SerializeField] Bark[] allBarks = null;
    [SerializeField] Bark[] possibleReplyBarks = null;
    [SerializeField] List<Conversation> allConversations = new List<Conversation>();
    public List<Bark> availableBarks = new List<Bark>();
    public List<Conversation> availableConversations = new List<Conversation>();


    [SerializeField] GameObject barkPrefab = null;
    [SerializeField] GameObject noticeMePrefab = null;

    NoticeMeDriver noticeMe;
    GameController gc;
    NPC_Brain brain;
    GameObject player;
    ConversationPanelDriver cpd;
    PlayerDialogMemory pdm;

    //param
    float timeBetweenBarks = 4;
    float conversationRange = 1.5f;
    [SerializeField] public bool CanSpeak = true;

    //state
    int currentBarkIndex = 0; 

    float timeForNextBark;
    BarkShell currentBark;


    // Start is called before the first frame update
    void Start()
    {
        brain = GetComponent<NPC_Brain>();
        cpd = FindObjectOfType<ConversationPanelDriver>();
        gc = FindObjectOfType<GameController>();
        gc.OnGameStart += RespondToGameStart;

    }

    private void RespondToGameStart()
    {
        player = gc.GetPlayer();
        pdm = player.GetComponent<PlayerDialogMemory>();

        availableBarks = RebuildAvailableBarksBasedOnPlayerKnownKeywords();
        currentBarkIndex = 0;

        availableConversations = RebuildAvailableConversationsBasedOnPlayerKnownKeywords();
        if (availableConversations.Count > 0)
        {
            ActivateNoticeMe();
        }

        pdm.OnKeywordAdded += RespondToPlayerGainingKeyword;
    }



    private void RespondToPlayerGainingKeyword(string newKeyword)
    {
        availableConversations = RebuildAvailableConversationsBasedOnPlayerKnownKeywords(newKeyword);
        if (availableConversations.Count > 0)
        {
            ActivateNoticeMe();
        }

        availableBarks = RebuildAvailableBarksBasedOnPlayerKnownKeywords(newKeyword);
        currentBarkIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > timeForNextBark && gc.isInGame && !gc.isInArena)
        {
            UpdateBark();
        }

        if (noticeMe && noticeMe.isActivated)
        {
            ListenForConversationEntryAttempt();
        }       
        
    }

    private void ListenForConversationEntryAttempt()
    {
        if (brain.requestedToHalt && !gc.isInArena && !gc.isPaused && !cpd.isDisplayed)
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
                    //Debug.Log("start conv");
                    cpd.InitalizeConversationPanel(availableConversations[0], this);
                    availableConversations.RemoveAt(0);
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

    public void PassNewKeywordToPlayerDialogMemory(string newKeyword)
    {
        pdm.AddKeyword(newKeyword);
    }

    #endregion
    private void UpdateBark()
    {   
        if (currentBarkIndex > availableBarks.Count - 1)
        {
            currentBarkIndex = 0;
        }
        Bark bark = availableBarks[currentBarkIndex];
        currentBarkIndex++;
        
        if (!currentBark)
        {
            currentBark = Instantiate(barkPrefab).GetComponent<BarkShell>();
        }
        currentBark?.ActivateBark(bark, transform);

        timeForNextBark = Time.time + timeBetweenBarks + bark.DisplayTime;
    }

    private List<Bark> RebuildAvailableBarksBasedOnPlayerKnownKeywords()
    {
        List<Bark> availBarks = new List<Bark>();
        foreach (var bark in allBarks)
        {
            string testKeyword = bark.KeywordToShowFor;
            string bannedKeyword = bark.KeywordToHideFrom;
            if (pdm.CheckForPlayerKnowledgeOfARequiredKeyword(testKeyword) &&
                !pdm.CheckForPlayerKnowledgeOfABannedKeyword(bannedKeyword))
            {
                availBarks.Add(bark);
            }
        }
        return availBarks;
    }

    private List<Bark> RebuildAvailableBarksBasedOnPlayerKnownKeywords(string specificKeyword)
    {
        List<Bark> availBarks = new List<Bark>(0);
        foreach (var bark in allBarks)
        {
            string testKeyword = bark.KeywordToShowFor;
            string bannedKeyword = bark.KeywordToHideFrom;

            if (specificKeyword == testKeyword && specificKeyword != bannedKeyword)
            {
                availBarks.Add(bark);
            }
        }
        return availBarks;
    }

    private List<Conversation> RebuildAvailableConversationsBasedOnPlayerKnownKeywords(string specificKeyword)
    {
        List<Conversation> availConvos = new List<Conversation>(0);
        foreach (var convo in allConversations)
        {
            string testKeyword = convo.KeywordToShowFor;
            string bannedKeyword = convo.KeywordToHideFrom;
            
            if (specificKeyword == testKeyword && specificKeyword != bannedKeyword)
            {
                availConvos.Add(convo);
            }
        }
        return availConvos;
    }

    private List<Conversation> RebuildAvailableConversationsBasedOnPlayerKnownKeywords()
    {
        List<Conversation> availConvos = new List<Conversation>(0);
        foreach (var convo in allConversations)
        {
            string testKeyword = convo.KeywordToShowFor;
            string bannedKeyword = convo.KeywordToHideFrom;
            if (pdm.CheckForPlayerKnowledgeOfARequiredKeyword(testKeyword) &&
                !pdm.CheckForPlayerKnowledgeOfABannedKeyword(bannedKeyword))
            {
                availConvos.Add(convo);
            }
        }
        return availConvos;
    }


    private void ActivateNoticeMe()
    {
        if (!noticeMe)
        {
            noticeMe = Instantiate(noticeMePrefab, transform).GetComponent<NoticeMeDriver>();
            noticeMe.ToggleNoticeMe(true);
        }
        else
        {
            noticeMe.ToggleNoticeMe(true);
        }
    }

    private void OnDestroy()
    {
        gc.OnGameStart -= RespondToGameStart;
    }

    
}
