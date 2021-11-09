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
        ShowHideElements(true); // This is should always turn itself on
    }

    public void StartGameSelected()
    {
        gc = lib.gameController;
        gc.debug_IgniteAll = false;
        gc.StartNewGame();
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
