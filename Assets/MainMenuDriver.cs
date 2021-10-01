using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuDriver : MonoBehaviour
{
    SceneLoader sl;
    GameController gc;
    public void StartGameSelected_SkirmishMode()
    {
        gc = FindObjectOfType<GameController>();
        gc.startMode = GameController.StartMode.Skirmish;
        if (!sl)
        {
            sl = FindObjectOfType<SceneLoader>();
        }
        sl.GoToMainGameScene();
    }
    public void StartGameSelected_TutorialMode()
    {
        gc = FindObjectOfType<GameController>();
        gc.startMode = GameController.StartMode.Tutorial;
        if (!sl)
        {
            sl = FindObjectOfType<SceneLoader>();
        }
        sl.GoToMainGameScene();
    }

    public void StartGameSelected_StoryMode()
    {
        gc = FindObjectOfType<GameController>();
        gc.startMode = GameController.StartMode.Story;
        if (!sl)
        {
            sl = FindObjectOfType<SceneLoader>();
        }
        sl.GoToMainGameScene();
    }


}
