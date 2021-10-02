using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameController : MonoBehaviour
{
    //init
    [SerializeField] GameObject playerPrefab = null;
    [SerializeField] GameObject wordValidaterPrefab = null;

    GameObject player;
    CinemachineVirtualCamera cvc;
    SceneLoader sl;
    GameObject wv;
    VictoryMeter vm;
    UIDriver uid;

    public enum StartMode { Story, Skirmish, Tutorial};

    //param
    Vector2 storyStartLocation = new Vector2(0, 0);
    Vector2 tutorialStartLocation = new Vector2(-4, 17);
    Vector2 skirmishStartLocation = new Vector2(93, -72);

    //state
    public StartMode startMode = StartMode.Story;

    int pauseRequests = 0;
    public bool isPaused { get; set; } = false;
    public bool isInArena { get; set; } = false;
    public bool isInGame { get; set; } = false;
    public bool isInTutorialMode { get; set; } = false;

    void Awake()
    {
        int count = FindObjectsOfType<GameController>().Length;
        if (count > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        sl = FindObjectOfType<SceneLoader>();
        sl.OnSceneChange += CheckGameStateBasedOnSceneChange;
        cvc = Camera.main.GetComponentInChildren<CinemachineVirtualCamera>();
    }  
    private void CheckGameStateBasedOnSceneChange(int newScene)
    {
        if (newScene == sl.MainGameScene)
        {
            StartNewGame();
        }
        if (newScene == sl.EndingScene)
        {
            EndCurrentGame();
        }
    }


    #region Start Game Methods
    public void StartNewGame()
    {
        isInGame = true;
        uid = FindObjectOfType<UIDriver>();
        uid.EnterOverworld();
        vm = FindObjectOfType<VictoryMeter>();
        ResumeGameSpeed();

        SpawnPlayer();
        SpawnWordUtilities();
        SetCameraToFollowPlayer();

        AdjustGameForStartMode();

        
    }

    private void SpawnPlayer()
    {
        if (!player)
        {
            player = Instantiate(playerPrefab, storyStartLocation, Quaternion.identity) as GameObject;
        }
    }
    private void SpawnWordUtilities()
    {
        if (!wv)
        {
            wv = FindObjectOfType<WordValidater>().gameObject;
            if (!wv)
            {
                wv = Instantiate(wordValidaterPrefab, Vector2.zero, Quaternion.identity) as GameObject;
            }

        }
    }

    private void SetCameraToFollowPlayer()
    {
        if (!cvc)
        {
            cvc = Camera.main.GetComponentInChildren<CinemachineVirtualCamera>();
        }
        cvc.Follow = player.transform;
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
        isInGame = false;
        Destroy(player);

    }

    #endregion

    #region Public Methods

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

    public void SlowGameSpeed()
    {
        Time.timeScale = UIParameters.SlowGameCoefficient;
    }

    public void ResumeGameSpeed()
    {
        pauseRequests--;
        if(pauseRequests <= 0)
        {
            isPaused = false;
            Time.timeScale = 1;
            pauseRequests = 0;
        }

    }


    #endregion
    void OnDestroy()
    {
        sl.OnSceneChange -= CheckGameStateBasedOnSceneChange;
    }
   
}
