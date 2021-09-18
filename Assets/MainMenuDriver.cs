using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuDriver : MonoBehaviour
{
    SceneLoader sl;
    public void StartGameSelected()
    {
        if (!sl)
        {
            sl = FindObjectOfType<SceneLoader>();
        }
        sl.GoToMainGameScene();
    }
}
