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

    //state
    public bool isPaused { get; set; } = false;
    public bool isInArena { get; set; } = false;
    public bool isInGame { get; set; } = false;

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
        UnpauseGame();
        SpawnPlayer();
        SpawnWordUtilities();
        SetCameraToFollowPlayer();
        
    }

    private void SpawnPlayer()
    {
        if (!player)
        {
            player = Instantiate(playerPrefab, Vector2.zero, Quaternion.identity) as GameObject;
        }
    }
    private void SpawnWordUtilities()
    {
        if (!wv)
        {
            wv = Instantiate(wordValidaterPrefab, Vector2.zero, Quaternion.identity) as GameObject;
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
    }

    public void UnpauseGame()
    {
        isPaused = false;
        Time.timeScale = 1;
    }


    #endregion
    void OnDestroy()
    {
        sl.OnSceneChange -= CheckGameStateBasedOnSceneChange;
    }
   
}
