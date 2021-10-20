using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorDialogManager : NPCDialogManager
{
    WordWeaponizer playerWWZ;
    UIDriver uid;
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
        uid = FindObjectOfType<UIDriver>();
    }

    public void SetupTutorDM(WordWeaponizer playerWWZ)
    {
        this.playerWWZ = playerWWZ;
        playerWWZ.OnFireWord += HandleWordFired;

        uid.ShowHideTopPanel(false);
        bagman = FindObjectOfType<BagManager>();
        bagman.ModifyBagsEnabled(0);

    }

    private void HandleWordFired()
    {
        wordsFired++;
        if (wordsFired == wordFiredToStartConvo1)
        {
            StartCombatConversation(combatConvo1);
            bagman.ModifyBagsEnabled(3);
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
