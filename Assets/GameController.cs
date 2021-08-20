using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameController : MonoBehaviour
{
    //init
    [SerializeField] GameObject playerPrefab;
    GameObject player;
    CinemachineVirtualCamera cvc;
    SceneLoader sl;

    //state
    bool isPaused = false;
    bool isInArena = false;
    bool isInGame = false;

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
        SpawnPlayer();
        SetCameraToFollowPlayer();
    }
    private void SpawnPlayer()
    {
        if (!player)
        {
            player = Instantiate(playerPrefab, Vector2.zero, Quaternion.identity) as GameObject;
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
    private void EndCurrentGame()
    {
        isInGame = false;

    }

    #endregion

    void OnDestroy()
    {
        sl.OnSceneChange -= CheckGameStateBasedOnSceneChange;
    }
   
}
