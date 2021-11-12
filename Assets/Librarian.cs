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


    public void Start()
    {
        gameController = FindObjectOfType<GameController>();
        wordValidater = FindObjectOfType<WordValidater>();
        ui_Controller = FindObjectOfType<UI_Controller>();
        cameraController = FindObjectOfType<CameraController>();
        musicController = FindObjectOfType<MusicController>();
        bagManager = ui_Controller.combatPanel.GetComponent<BagManager>();

    }

    public static Librarian GetLibrarian()
    {
        return GameObject.FindGameObjectWithTag("Librarian").GetComponent<Librarian>();
    }
}
