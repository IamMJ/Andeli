using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogManager : MonoBehaviour
{
    //init
    [SerializeField] Bark[] possibleBarks = null;
    [SerializeField] Bark[] possibleReplyBarks = null;
    [SerializeField] Conversation[] possibleConversations = null;
    [SerializeField] GameObject barkPrefab = null;
    [SerializeField] GameObject noticeMePrefab = null;
    NoticeMeDriver noticeMe;
    GameController gc;

    //param
    float timeBetweenBarks = 4;
    float barkLifetime = 3;
    [SerializeField] public bool CanSpeak = true;

    //state
    float timeForNextBark;
    BarkShell currentBark;


    // Start is called before the first frame update
    void Start()
    {
        gc = FindObjectOfType<GameController>();
        if (!noticeMe)
        {
            noticeMe = Instantiate(noticeMePrefab, transform).GetComponent<NoticeMeDriver>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > timeForNextBark && gc.isInGame && !gc.isInArena)
        {
            UpdateBark();
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

}
