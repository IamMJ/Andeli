using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuDriver : MonoBehaviour
{
    SceneLoader sl;
    GameController gc;
    public void StartGameSelected_SkirmishMode()
    {
        if (!sl)
        {
            sl = FindObjectOfType<SceneLoader>();
        }
        sl.GoToMainGameScene();
    }
    public void StartGameSelected_TutorialMode()
    {
        gc = FindObjectOfType<GameController>();
        gc.isInTutorialMode = true;
        if (!sl)
        {
            sl = FindObjectOfType<SceneLoader>();
        }
        sl.GoToMainGameScene();
    }


}
