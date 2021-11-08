using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Librarian : MonoBehaviour
{
    public GameController gameController;
    public WordValidater wordValidater;
    public UI_Controller ui_Controller;

    public void Start()
    {
        gameController = FindObjectOfType<GameController>();
        wordValidater = FindObjectOfType<WordValidater>();
        ui_Controller = FindObjectOfType<UI_Controller>();
    }
}
