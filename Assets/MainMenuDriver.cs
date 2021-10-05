using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuDriver : MonoBehaviour
{

    GameController gc;
    
    public void StartGameSelected_SkirmishMode()
    {
        gc = FindObjectOfType<GameController>();
        gc.startMode = GameController.StartMode.Skirmish;
        gc.debug_IgniteAll = false;
        gc.StartNewGame();
    }
    public void StartGameSelected_TutorialMode()
    {
        gc = FindObjectOfType<GameController>();
        gc.startMode = GameController.StartMode.Tutorial;
        gc.debug_IgniteAll = false;
        gc.StartNewGame();
    }

    public void StartGameSelected_StoryMode()
    {
        gc = FindObjectOfType<GameController>();
        gc.startMode = GameController.StartMode.Story;
        gc.debug_IgniteAll = false;
        gc.StartNewGame();
    }

    public void StartGameSelected_DebugSkirmishMode()
    {
        gc = FindObjectOfType<GameController>();
        gc.startMode = GameController.StartMode.Skirmish;
        gc.debug_IgniteAll = true;
        gc.StartNewGame();
    }



}
