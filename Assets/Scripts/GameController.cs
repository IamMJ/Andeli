﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.U2D;

public class GameController : MonoBehaviour
{
    //init
    [SerializeField] GameObject playerPrefab = null;
    [SerializeField] GameObject driftingThingPrefab = null;

    GameObject driftingThing;
    GameObject player;
    CinemachineVirtualCamera cvc;
    //SceneLoader sl;
    WordValidater wv;
    VictoryMeter vm;

    UIDriver uid;
    MainMenuDriver mmd;
    LetterPowerMenuDriver lpmd;
    OptionMenuDriver pmd;
    Tutor tutor;
    ArenaBuilder currentArenaBuilder;
    PixelPerfectCamera ppc;

    public Action OnGameStart;

    public enum StartMode { Story, Skirmish, Tutorial};

    //param
    Vector2 storyStartLocation = new Vector2(0, 0);
    Vector2 tutorialStartLocation = new Vector2(-4, 17);
    Vector2 skirmishStartLocation = new Vector2(93, -72);
    int cameraSize_ZoomedIn = 10;
    int cameraSize_ZoomedOut = 40;
    int zoomRate = 10;


    //state
    public StartMode startMode = StartMode.Story;

    int pauseRequests = 0;
    public bool isPaused { get; set; } = false;
    public bool isInArena { get; set; } = false;
    public bool isInGame { get; set; } = false;
    public bool isInTutorialMode { get; set; } = false;
    [SerializeField] float currentZoom;
    public bool debug_IgniteAll = false;


    //void Awake()
    //{
    //    int count = FindObjectsOfType<GameController>().Length;
    //    if (count > 1)
    //    {
    //        Destroy(gameObject);
    //    }
    //    else
    //    {
    //        DontDestroyOnLoad(gameObject);
    //    }
    //}

    void Start()
    {
        //sl = FindObjectOfType<SceneLoader>();
        //sl.OnSceneChange += CheckGameStateBasedOnSceneChange;
        cvc = Camera.main.GetComponentInChildren<CinemachineVirtualCamera>();
        ppc = Camera.main.GetComponent<PixelPerfectCamera>();
        uid = FindObjectOfType<UIDriver>();
        uid.HideAllOverworldUIElements();
        uid.ShowHideMainMenu(true);

        vm = FindObjectOfType<VictoryMeter>();
        wv = GetComponent<WordValidater>();

        currentZoom = cameraSize_ZoomedOut;
        ppc.enabled = false;

        BeginIdleMode();
    }

    private void BeginIdleMode()
    {
        ResumeGameSpeed(true);
        if (!driftingThing)
        {
            driftingThing = Instantiate(driftingThingPrefab, Vector2.zero, Quaternion.identity);
        }

        SetCameraToIdleMode();
        // Create a bird or fox to run around the map?
 
    }

    private void SetCameraToIdleMode()
    {
        cvc.Follow = driftingThing.transform;
        StartCoroutine(ZoomCamera(false));
        // move cvc somewhere other than location where player quit?
    }

    IEnumerator ZoomCamera(bool shouldZoomIn)
    {
        if (shouldZoomIn)
        {
            ppc.enabled = false;
            while (currentZoom > cameraSize_ZoomedIn)
            {
                currentZoom -= Time.deltaTime * zoomRate;
                cvc.m_Lens.OrthographicSize = currentZoom;
                if (currentZoom - cameraSize_ZoomedIn <= 1)
                {
                    ppc.enabled = true;
                    ppc.assetsPPU = 16;
                }
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            ppc.enabled = false;
            while (currentZoom < cameraSize_ZoomedOut)
            {

                currentZoom += Time.deltaTime * zoomRate;
                cvc.m_Lens.OrthographicSize = currentZoom;
                if (cameraSize_ZoomedOut - currentZoom <= 1)
                {
                    ppc.enabled = true;
                    ppc.assetsPPU = 4;
                }
                yield return new WaitForEndOfFrame();
            }

        }

    }

    //private void CheckGameStateBasedOnSceneChange(int newScene)
    //{
    //    if (newScene == sl.MainGameScene)
    //    {
    //        StartNewGame();
    //    }
    //    if (newScene == sl.EndingScene)
    //    {
    //        EndCurrentGame();
    //    }
    //}

    #region Start Game Methods
    public void StartNewGame()
    {
        isInGame = true;
        uid.ShowOverworldUIElements();
        //ResumeGameSpeed(true);
        SpawnPlayer();
        uid.ShowHideMainMenu(false);
        uid.ShowOverworldUIElements();
        SetCameraToFollowPlayer();
        AdjustGameForStartMode();
        OnGameStart.Invoke();
    }

    private void SpawnPlayer()
    {
        if (!player)
        {
            player = Instantiate(playerPrefab, storyStartLocation, Quaternion.identity) as GameObject;
        }
    }
    private void SetCameraToFollowPlayer()
    {
        if (driftingThing)
        {
            driftingThing.SetActive(false);
        }
        if (!cvc)
        {
            cvc = Camera.main.GetComponentInChildren<CinemachineVirtualCamera>();
        }
        cvc.Follow = player.transform;
        StartCoroutine(ZoomCamera(true));
    }

    private void AdjustGameForStartMode()
    {
        isInTutorialMode = false;
        switch (startMode)
        {
            case StartMode.Story:
                player.transform.position = storyStartLocation;
                return;

            case StartMode.Tutorial:
                FindObjectOfType<Tutor>().gc = this;
                player.transform.position = tutorialStartLocation;
                isInTutorialMode = true;
                uid.ShowHideTutorialPanel(true);
                return;

            case StartMode.Skirmish:
                player.transform.position = skirmishStartLocation;
                return;
        }
    }

    #endregion

    #region End Game Methods
    public void EndCurrentGame()
    {
        isPaused = false;
        isInTutorialMode = false;
        isInArena = false;
        isInGame = false;
        if (currentArenaBuilder)
        {
            currentArenaBuilder.CloseDownArena();
        }
        Destroy(player);
        uid.HideAllOverworldUIElements();
        uid.ShowHideMainMenu(true);
        BeginIdleMode();
        // uid.disable or hide everything
        //
    }

    #endregion

    #region Public Methods

    public void RegisterCurrentArenaBuilder(ArenaBuilder ab)
    {
        currentArenaBuilder = ab;
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    public VictoryMeter GetVictoryMeter()
    {
        return vm;
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        pauseRequests++;
    }

    //public void SlowGameSpeed()
    //{
    //    Time.timeScale = UIParameters.SlowGameCoefficient;
    //}

    public void ResumeGameSpeed(bool hasOverride)
    {
        if (hasOverride)
        {
            pauseRequests = 0;
            isPaused = false;
            Time.timeScale = 1;
        }
        else
        {
            pauseRequests--;
            if (pauseRequests <= 0)
            {
                isPaused = false;
                Time.timeScale = 1;
                pauseRequests = 0;
            }
        }


    }


    #endregion
    void OnDestroy()
    {
        //sl.OnSceneChange -= CheckGameStateBasedOnSceneChange;
    }
   
}
