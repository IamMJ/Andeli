using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorDialogManager : NPCDialogManager
{
    WordWeaponizer playerWWZ;
    CombatPanel uid;
    BagManager bagman;

    //param
    [SerializeField] Conversation combatConvo1 = null;
    [SerializeField] int wordFiredToStartConvo1 = 2;
    [SerializeField] Conversation combatConvo2 = null;
    [SerializeField] int wordFiredToStartConvo2 = 4;
    [SerializeField] Conversation combatConvo3 = null;
    [SerializeField] int wordFiredToStartConvo3 = 6;

    //state
    int wordsFired = 0;

    protected override void Start()
    {
        base.Start();
        uid = FindObjectOfType<CombatPanel>();
    }

    protected override void Update()
    {
        if (gc.isPaused) { return; }
        if (Time.time > timeForNextBark && gc.isInGame)
        {
            UpdateBark();
        }
        if (noticeMe && noticeMe.isActivated)
        {
            ListenForConversationEntryAttempt();
        }    
    }
    public void SetupTutorDM(WordWeaponizer playerWWZ)
    {
        this.playerWWZ = playerWWZ;
        if (player.GetComponent<PlayerMemory>().CheckForPlayerKnowledgeOfARequiredKeyword("ITUT1"))
        {
            playerWWZ.OnFireWord += HandleWordFired;
            uid.ShowHideIgnitionChancePanel(false);
            uid.ShowHideTopPanel(false);
            bagman = FindObjectOfType<BagManager>();
            bagman.ModifyBagsEnabled(0);
        }
    }

    private void HandleWordFired()
    {
        wordsFired++;
        if (wordsFired == wordFiredToStartConvo1)
        {
            StartCombatConversation(combatConvo1);
            bagman.ModifyBagsEnabled(2);
        }
        if (wordsFired == wordFiredToStartConvo2)
        {
            StartCombatConversation(combatConvo2);
            uid.ShowHideTopPanel(true);
        }
        if (wordsFired == wordFiredToStartConvo3)
        {
            StartCombatConversation(combatConvo3);
            uid.ShowHideIgnitionChancePanel(true);

        }
    }

    private void StartCombatConversation(Conversation nextConvo)
    {
        if (cpd.ClaimConversationPanelDriverIfUnused(this))
        {
            cpd.InitalizeConversationPanel(nextConvo, this);
            noticeMe.ToggleNoticeMe(false);
        }
    }
}
