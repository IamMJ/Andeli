using System;
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

    Librarian lib;

    GameObject driftingThing;
    GameObject player;
    CinemachineVirtualCamera cvc;
    //SceneLoader sl;
    WordValidater wv;
    VictoryMeter vm;
    GameStateHolder gash;
    UI_Controller uic;

    CombatPanel uid;
    ConversationPanelDriver cpd;
    BriefPanel bpd;
    DebriefPanelDriver dpd;
    RewardPanelDriver rpd;
    AdvertPanelDriver apd;
    StartMenuPanel mmd;
    LetterPowerMenuDriver lpmd;
    DebugPanel pmd;


    ArenaBuilder currentArenaBuilder;
    PixelPerfectCamera ppc;
    MusicController mc;

    public Action OnGameStart;


    //param
    Vector2 normalStartLocation = new Vector2(93, -72);
    Vector2 tutorialStartLocation = new Vector2(102, 66);
    Vector2 skirmishStartLocation = new Vector2(93, -72);
    int cameraSize_ZoomedIn = 10;
    int cameraSize_ZoomedOut = 30;
    int zoomRate = 10;
    Vector3 cameraOffset_Arena = new Vector3(0, -1.5f, 0);
    Vector3 cameraOffset_Overworld = Vector3.zero;

    int pauseRequests = 0;
    public bool isPaused = false;
    public bool isInArena { get; set; } = false;
    public bool isInGame { get; set; } = false;

    [SerializeField] float currentZoom;
    public bool debug_IgniteAll = false;
    public bool debug_ShowAILetterValues = false;
    public bool debug_ShowDebugMenuButton = false;


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
        lib = FindObjectOfType<Librarian>();
        uic = lib.ui_Controller;
        uic.SetContext(UI_Controller.Context.StartMenu);


        cvc = Camera.main.GetComponentInChildren<CinemachineVirtualCamera>();
        ppc = Camera.main.GetComponent<PixelPerfectCamera>();
        //uid = FindObjectOfType<UIDriver>();
        //cpd = FindObjectOfType<ConversationPanelDriver>();
        //dpd = FindObjectOfType<DebriefPanelDriver>();
        //bpd = FindObjectOfType<BriefPanelDriver>();
        //rpd = FindObjectOfType<RewardPanelDriver>();
        //apd = FindObjectOfType<AdvertPanelDriver>();



        mc = FindObjectOfType<MusicController>();
        //uid.HideAllOverworldUIElements();
        //uid.ShowHideMainMenu(true);
        //uid.ShowHideDebugMenu(debug_ShowDebugMenu);

        gash = FindObjectOfType<GameStateHolder>();
        vm = FindObjectOfType<VictoryMeter>();
        wv = GetComponent<WordValidater>();

        currentZoom = cameraSize_ZoomedOut;
        ppc.enabled = true;

        //BeginIdleMode();
    }

    //private void BeginIdleMode()
    //{
    //    ResumeGameSpeed(true);
    //    if (!driftingThing)
    //    {
    //        driftingThing = Instantiate(driftingThingPrefab, Vector2.zero, Quaternion.identity);
    //    }

    //    SetCameraToIdleMode();
    //    // Create a bird or fox to run around the map?
 
    //}

    //private void SetCameraToIdleMode()
    //{
    //    cvc.Follow = driftingThing.transform;
    //    driftingThing.SetActive(true);
    //    StopAllCoroutines();
    //    StartCoroutine(ZoomCamera(false));
    //    SetCameraToOverworldOffset();
    //    // move cvc somewhere other than location where player quit?
    //}

    //IEnumerator ZoomCamera(bool shouldZoomIn)
    //{
    //    if (shouldZoomIn)
    //    {
            
    //        Time.timeScale = 1;
    //        ppc.enabled = false;
    //        while (currentZoom > cameraSize_ZoomedIn)
    //        {
    //            currentZoom -= Time.deltaTime * zoomRate;
                
    //            float factor = Mathf.InverseLerp(cameraSize_ZoomedIn, cameraSize_ZoomedOut, currentZoom);
    //            mc.FadeMainThemeWithZoom(factor);
    //            if (currentZoom - cameraSize_ZoomedIn <= 1)
    //            {
    //                ppc.enabled = true;
    //                ppc.assetsPPU = 16;
    //            }
    //            currentZoom = Mathf.Clamp(currentZoom, cameraSize_ZoomedIn, cameraSize_ZoomedOut);
    //            cvc.m_Lens.OrthographicSize = currentZoom;
    //            yield return new WaitForEndOfFrame();
    //        }
    //    }
    //    else
    //    {
    //        Time.timeScale = 1;
    //        ppc.enabled = false;
    //        while (currentZoom < cameraSize_ZoomedOut)
    //        {
    //            currentZoom += Time.deltaTime * zoomRate;
    //            float factor = Mathf.InverseLerp(cameraSize_ZoomedIn, cameraSize_ZoomedOut, currentZoom);
    //            mc.FadeMainThemeWithZoom(factor);
    //            if (cameraSize_ZoomedOut - currentZoom <= 1)
    //            {
    //                ppc.enabled = true;
    //                ppc.assetsPPU = 4;
    //            }
    //            currentZoom = Mathf.Clamp(currentZoom, cameraSize_ZoomedIn, cameraSize_ZoomedOut);
    //            cvc.m_Lens.OrthographicSize = currentZoom;
    //            yield return new WaitForEndOfFrame();
    //        }

    //    }

    //}

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
        uic.SetContext(UI_Controller.Context.Overworld);
        SpawnPlayer();
        SetCameraToFollowPlayer();
        SetCameraToOverworldOffset();

        uic.ShowHideDebugMenuButton(debug_ShowDebugMenuButton);

        //gash.RestoreAllObelisks();


        //Populate enemies on the ArenaMenu, then switch to it.



        //ResumeGameSpeed(true);
        //SpawnPlayer();
        //uid.ShowHideMainMenu(false);
        //uid.ShowOverworldUIElements();
        //SetCameraToFollowPlayer();
        //SetCameraToOverworldOffset();
        ////AdjustGameForStartMode();
        //uid.ShowHideDebugMenu(debug_ShowDebugMenu);
        OnGameStart.Invoke();
    }

    private void SpawnPlayer()
    {
        if (!player)
        {
            player = Instantiate(playerPrefab, normalStartLocation, Quaternion.identity) as GameObject;
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
        //StopAllCoroutines();
        //StartCoroutine(ZoomCamera(true));
    }

    //private void AdjustGameForStartMode()
    //{

    //    switch (startMode)
    //    {
    //        case StartMode.Story:
    //            player.transform.position = tutorialStartLocation;
    //            return;

    //        case StartMode.Skirmish:
    //            player.transform.position = skirmishStartLocation;
    //            return;
    //    }
    //}


    #endregion

    #region End Game Methods
    public void EndCurrentGame()
    {
        isInArena = false;
        isInGame = false;
        if (currentArenaBuilder)
        {
            currentArenaBuilder.CloseDownArena();
            ResumeGameSpeed(true);
        }
        Destroy(player);
        //uid.HideAllOverworldUIElements();
        //uid.ShowHideMainMenu(true);
        //cpd.ShowHideEntirePanel(false);
        //dpd.ShowHideEntirePanel(false);
        //bpd.ShowHideEntirePanel(false);
        //BeginIdleMode();
        // uid.disable or hide everything
        //
    }

    #endregion

    #region Public Methods

    public bool ToggleDebugMenuMode()
    {
        Debug.Log($"Debug Menu is {debug_ShowDebugMenuButton}");
        debug_ShowDebugMenuButton = !debug_ShowDebugMenuButton;
        return debug_ShowDebugMenuButton;
    }
    public void SetCameraToArenaOffset()
    {
        cvc.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = cameraOffset_Arena;
    }

    public void SetCameraToOverworldOffset()
    {
        cvc.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = cameraOffset_Overworld;
    }
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
