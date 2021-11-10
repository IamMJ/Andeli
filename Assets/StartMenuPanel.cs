using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartMenuPanel : UI_Panel
{
    [SerializeField] TextMeshProUGUI lastLetter = null;

    Librarian lib;
    GameController gc;

    private void Start()
    {
        lib = FindObjectOfType<Librarian>();
        gc = lib.gameController;
        ShowHideElements(true); // The start menu panel should always turn itself on
    }

    public void StartGameSelected()
    {
        gc.StartNewGame();
    }

    public void StartTutorial()
    {
        //Start tutorial
        Debug.Log("Start Tutorial");
    }

    public void GoToPlayerSelectPanel()
    {
        //Switch context to player select screen
        Debug.Log("Select different player");
    }

    public void ToggleDebugMenuOption()
    {
        gc = FindObjectOfType<GameController>();
        if (gc.ToggleDebugMenuMode())
        {
            lastLetter.fontStyle = FontStyles.Italic;
        }
        else
        {
            lastLetter.fontStyle = FontStyles.Normal;
        }
    }
}
