using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Librarian : MonoBehaviour
{
    public GameController gameController;
    public WordValidater wordValidater;
    public UI_Controller ui_Controller;
    public CameraController cameraController;
    public MusicController musicController;
    public BagManager bagManager;
    public LetterTileDropper letterTileDropper;
    [SerializeField] LetterTile sourceLT = null;


    public void Start()
    {
        gameController = FindObjectOfType<GameController>();
        wordValidater = FindObjectOfType<WordValidater>();
        ui_Controller = FindObjectOfType<UI_Controller>();
        cameraController = FindObjectOfType<CameraController>();
        musicController = FindObjectOfType<MusicController>();
        bagManager = ui_Controller.combatPanel.GetComponent<BagManager>();
        letterTileDropper = wordValidater.GetComponent<LetterTileDropper>();

    }

    public static Librarian GetLibrarian()
    {
        return GameObject.FindGameObjectWithTag("Librarian").GetComponent<Librarian>();
    }

    public LetterTile GetSourceLetterTile()
    {
        return sourceLT;
    }
}
