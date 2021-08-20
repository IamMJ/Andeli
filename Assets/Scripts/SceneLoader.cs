﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SceneLoader : MonoBehaviour
{
    public Action<int> OnSceneChange;

    //param
    public int WelcomeScene = 0;
    public int MainGameScene = 1;
    public int EndingScene = 2;

    //state
    AsyncOperation loadingOperation;

    void Awake()
    {
        int slCount = FindObjectsOfType<SceneLoader>().Length;
        if (slCount > 1)
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
        loadingOperation = SceneManager.LoadSceneAsync(MainGameScene,LoadSceneMode.Single); //Load this at start because it takes a while.
        loadingOperation.allowSceneActivation = false;
    }
    public void GoToMainGameScene()
    {
        loadingOperation.allowSceneActivation = true;
        StartCoroutine(WaitForLoadToFinish());
    }

    IEnumerator WaitForLoadToFinish()
    {
        while (loadingOperation.progress < 1)
        {
            yield return new WaitForEndOfFrame();
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(MainGameScene));
        OnSceneChange.Invoke(MainGameScene);
        SceneManager.UnloadSceneAsync(WelcomeScene, UnloadSceneOptions.None);
    }

    public void LoadEndingScene()
    {
        SceneManager.LoadScene(EndingScene);
        OnSceneChange.Invoke(EndingScene);
    }
}
