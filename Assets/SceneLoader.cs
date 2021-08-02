using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadSecondScene()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadThirdScene()
    {
        SceneManager.LoadScene(2);
    }
}
