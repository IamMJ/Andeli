﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ConversationPanelDriver : MonoBehaviour
{
    //init
    NPCDialogManager claimingDiaman;
    [SerializeField] Image NPCPortraitImage = null;
    [SerializeField] TextMeshProUGUI NPCTextTMP = null;
    [SerializeField] Image NPCMainImage = null;
    [SerializeField] Image PlayerPortraitImage = null;
    [SerializeField] GameObject playerResponseGO_0 = null;
    [SerializeField] TextMeshProUGUI playerResponseTMP_0 = null;
    [SerializeField] GameObject playerResponeGO_1 = null;
    [SerializeField] TextMeshProUGUI playerResponseTMP_1 = null;

    GameController gc;
    GameObject player;

    //state
    public bool isDisplayed = false;
    int currentConvoStepIndex = 0;
    Conversation convo;
    ConversationStep convoStep;
    ConversationStep.ReplyOption option0;
    ConversationStep.ReplyOption option1;

    // Start is called before the first frame update

    void Start()
    {
        gc = FindObjectOfType<GameController>();
        player = gc.GetPlayer();
        ShowHideEntirePanel(false);
    }


    #region Public Methods
    public bool ClaimConversationPanelDriverIfUnused(NPCDialogManager claimant)
    {
        if (claimingDiaman)
        {
            
            return false;
        }
        else
        {
            claimingDiaman = claimant;
            return true;
        }

    }

    public void InitalizeConversationPanel(Conversation newConvo, NPCDialogManager claimant)
    {
        convo = newConvo;
        if (claimant != claimingDiaman)
        {
            return;
        }
        if (!player)
        {
            player = gc.GetPlayer();
        }

        ShowHideEntirePanel(true);
        gc.PauseGame();
        currentConvoStepIndex = 0;
        convoStep = convo.GetConversationStepAtIndex(currentConvoStepIndex);
        UpdateUIWithCurrentConvoStep();
    }

    private void UpdateUIWithCurrentConvoStep()
    {
        if (!convoStep)
        {
            Debug.Log("no convo step");
            return;
        }
        UpdatePortraitImages(convoStep);
        UpdateConversationText(convoStep);
        ShowHideNPCImage(convoStep);
        option0 = convoStep.resultFromOption0;
        if (convoStep.PlayerText_1 != "")
        {
            option1 = convoStep.resultFromOption1;
        }
    }

    public void ShutdownConversationPanel()
    {
        gc.ResumeGameSpeed(true);
        ShowHideEntirePanel(false);
        claimingDiaman.PassNewKeywordToPlayerDialogMemory(convo.KeywordAtCompletion);
        claimingDiaman = null;

    }

    #endregion

    #region Player Response Handlers

    public void ClickOnResponseOption(int buttonIndex)
    {
        if (buttonIndex == 0)
        {
            HandleButtonPress(option0);
        }
        if (buttonIndex == 1)
        {
            HandleButtonPress(option1);
        }
    }

    private void HandleButtonPress(ConversationStep.ReplyOption option)
    {
        switch (option)
        {
            case ConversationStep.ReplyOption.AdvanceOneStep:
                currentConvoStepIndex++;
                convoStep = convo.GetConversationStepAtIndex(currentConvoStepIndex);
                UpdateUIWithCurrentConvoStep();
                return;

            case ConversationStep.ReplyOption.AdvanceTwoSteps:
                currentConvoStepIndex++;
                currentConvoStepIndex++;
                convoStep = convo.GetConversationStepAtIndex(currentConvoStepIndex);
                UpdateUIWithCurrentConvoStep();

                return;

            case ConversationStep.ReplyOption.MoveNPCandQuitConvo:
                claimingDiaman.GetComponent<NPC_Brain>().RequestNPCToMoveToSpecificDestination(convoStep.NPCDestinationIfMoving);
                ShutdownConversationPanel();
                return;

            case ConversationStep.ReplyOption.QuitConvo:

                ShutdownConversationPanel();
                return;
        }
    }


    #endregion

    private void ShowHideEntirePanel(bool shouldShowPanel)
    {
        if (shouldShowPanel)
        {
            NPCPortraitImage.enabled = true;
            NPCTextTMP.enabled = true;
            NPCMainImage.enabled = true;
            PlayerPortraitImage.enabled = true;
            playerResponseGO_0.SetActive(true);
            playerResponeGO_1.SetActive(true);
            GetComponent<Image>().enabled = true;

            isDisplayed = true;
        }
        if (shouldShowPanel == false)
        {
            NPCPortraitImage.enabled = false;
            NPCTextTMP.enabled = false;
            NPCMainImage.enabled = false;
            PlayerPortraitImage.enabled = false;
            playerResponseGO_0.SetActive(false);
            playerResponeGO_1.SetActive(false);
            GetComponent<Image>().enabled = false;

            isDisplayed = false;
        }
}

    private void UpdatePortraitImages(ConversationStep convoStep)
    {
        NPCPortraitImage.sprite = claimingDiaman.GetComponent<SpriteRenderer>().sprite;
        NPCPortraitImage.color = claimingDiaman.GetComponent<SpriteRenderer>().color;
        PlayerPortraitImage.sprite = player.GetComponent<SpriteRenderer>().sprite;
        PlayerPortraitImage.color = player.GetComponent<SpriteRenderer>().color;
    }

    private void UpdateConversationText(ConversationStep convoStep)
    {
        NPCTextTMP.text = convoStep.NPCText;
        playerResponseTMP_0.text = convoStep.PlayerText_0;
        if (convoStep.PlayerText_1 != "")
        {
            playerResponeGO_1.SetActive(true);
            playerResponseTMP_1.text = convoStep.PlayerText_1;
        }
        else
        {
            playerResponeGO_1.SetActive(false);
        }
    }


    private void ShowHideNPCImage(ConversationStep convoStep)
    {
        if (convoStep.NPCSprite)
        {
            NPCMainImage.color = Color.white;
            NPCMainImage.sprite = convoStep.NPCSprite;
        }
        else
        {
            NPCMainImage.color = Color.clear;
        }
    }
}