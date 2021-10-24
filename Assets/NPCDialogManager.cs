using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogManager : MonoBehaviour
{
    //init
    [SerializeField] Bark[] allPeaceBarks = null;
    [SerializeField] Bark[] allReplyBarks = null;
    [SerializeField] Bark[] allCombatBarks = null;
    [SerializeField] List<Conversation> allConversations = new List<Conversation>();
    [SerializeField]  List<Bark> availablePeaceBarks = new List<Bark>();
    [SerializeField]  List<Bark> availableReplyBarks = new List<Bark>();
    [SerializeField]  List<Bark> availableCombatBarks = new List<Bark>();
    [SerializeField]  List<Conversation> availableConversations = new List<Conversation>();


    [SerializeField] protected GameObject barkPrefab = null;
    [SerializeField] protected GameObject noticeMePrefab = null;

    protected NoticeMeDriver noticeMe;
    protected GameController gc;
    protected NPC_Brain brain;
    protected GameObject player;
    protected ConversationPanelDriver cpd;
    protected PlayerDialogMemory pdm;

    //param
    [SerializeField] float timeBetweenBarks_average = 4;
    float conversationRange = 2f;
    float timeBetweenConvoReset = 4f;

    //state
    bool isPlayerInRange = false;
    int currentBarkIndex = -1;
    protected float timeForNextConvo = 0;
    protected float timeForNextBark;
    BarkShell currentBark;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        brain = GetComponent<NPC_Brain>();
        cpd = FindObjectOfType<ConversationPanelDriver>();
        gc = FindObjectOfType<GameController>();
        gc.OnGameStart += RespondToGameStart;
        timeForNextBark = Time.time + 2;
        timeForNextConvo = Time.time + 2;

    }

    private void RespondToGameStart()
    {
        player = gc.GetPlayer();
        pdm = player.GetComponent<PlayerDialogMemory>();

        availablePeaceBarks = RebuildAvailableBarks(ref allPeaceBarks);
        availableReplyBarks = RebuildAvailableBarks(ref allReplyBarks);
        availableCombatBarks = RebuildAvailableBarks(ref allCombatBarks);
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
        else
        {
            DeactivateNoticeMe();
        }

        availablePeaceBarks = RebuildAvailableBarks(ref allPeaceBarks, newKeyword);
        availableReplyBarks = RebuildAvailableBarks(ref allReplyBarks, newKeyword);
        availableCombatBarks = RebuildAvailableBarks(ref allCombatBarks, newKeyword);
        currentBarkIndex = 0;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (gc.isPaused) { return; }

        if (Time.time > timeForNextBark && gc.isInGame && isPlayerInRange)
        {
            UpdateBark();
        }

        if (noticeMe && noticeMe.isActivated)
        {
            ListenForConversationEntryAttempt();
        }       
        
    }

    protected void ListenForConversationEntryAttempt()
    {
        if (brain.requestedToHalt && !gc.isInArena && !gc.isPaused && !cpd.isDisplayed && Time.time >= timeForNextConvo)
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
                    availableConversations = RebuildAvailableConversationsBasedOnPlayerKnownKeywords();
                    if (availableConversations.Count > 0)
                    {
                        ActivateNoticeMe();
                        timeForNextConvo = Time.time + timeBetweenConvoReset;
                    }
                }

            }
        }
    }

    #region Public Methods
    public void ProvideReplyBarkToPlayer()
    {
        if (availableReplyBarks.Count == 0) { return; }
        int rand = UnityEngine.Random.Range(0, availableReplyBarks.Count);
        Bark bark = availableReplyBarks[rand];
        if (!currentBark)
        {
            currentBark = Instantiate(barkPrefab).GetComponent<BarkShell>();
        }
        currentBark.ActivateBark(bark, transform);

        timeForNextBark = Time.time + (timeBetweenBarks_average*UnityEngine.Random.Range(0.8f, 1.2f)) + bark.DisplayTime;
    }

    public void PassNewKeywordToPlayerDialogMemory(string newKeyword)
    {
        pdm.AddKeyword(newKeyword);
    }

    #endregion

    #region Private Helpers

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 15)
        {
            isPlayerInRange = true;
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 15)
        {
            isPlayerInRange = false;
        }
    }

    protected void UpdateBark()
    {
        Bark bark;
        if (gc.isInArena)
        {
            if (availableCombatBarks.Count == 0) { return; }
            currentBarkIndex++;
            if (currentBarkIndex > availableCombatBarks.Count - 1)
            {
                currentBarkIndex = 0;
            }

            bark = availableCombatBarks[currentBarkIndex];
        }
        else
        {
            if (availablePeaceBarks.Count == 0) { return; }
            currentBarkIndex++;
            if (currentBarkIndex > availablePeaceBarks.Count - 1)
            {
                currentBarkIndex = 0;
            }
            bark = availablePeaceBarks[currentBarkIndex];
        }
        
        if (currentBark == null)
        {
            currentBark = Instantiate(barkPrefab).GetComponent<BarkShell>();
        }
        else
        {
            currentBark.ActivateBark(bark, transform);
            timeForNextBark = Time.time + (timeBetweenBarks_average * UnityEngine.Random.Range(0.8f, 1.2f)) + bark.DisplayTime;
        }
    }
        
    

    private List<Bark> RebuildAvailableBarks(ref Bark[] masterBarkList)
    {
        List<Bark> availBarks = new List<Bark>();
        foreach (var bark in masterBarkList)
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
    private List<Bark> RebuildAvailableBarks(ref Bark[] masterBarkList, string specificKeyword)
    {
        List<Bark> availBarks = new List<Bark>(0);
        foreach (var bark in masterBarkList)
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

    private void DeactivateNoticeMe()
    {
        if (noticeMe)
        {
            noticeMe.ToggleNoticeMe(false);
        }
    }

    #endregion

    private void OnDestroy()
    {
        gc.OnGameStart -= RespondToGameStart;
    }


    
}
