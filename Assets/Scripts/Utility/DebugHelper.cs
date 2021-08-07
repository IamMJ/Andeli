using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class DebugHelper : MonoBehaviour
{
    [SerializeField] Image debugLogPanel = null;
    [SerializeField] TextMeshProUGUI textline_0 = null;
    [SerializeField] TextMeshProUGUI textline_1 = null;
    [SerializeField] TextMeshProUGUI textline_2 = null;

    //param
    float timeToDisplayDebugLog = 5f;

    //state
    float timeToHideDebugLog = Mathf.Infinity;

    // Update is called once per frame

    private void Start()
    {
        timeToHideDebugLog = Time.time;
    }
    void Update()
    {
        HandleDebugLogVisibility();
    }

    private void HandleDebugLogVisibility()
    {
        if (Time.time >= timeToHideDebugLog)
        {
            debugLogPanel.gameObject.SetActive(false);
            textline_0.gameObject.SetActive(false);
            textline_1.gameObject.SetActive(false);
            textline_2.gameObject.SetActive(false);
        }
    }

    public void DisplayDebugLog(string newText)
    {
        debugLogPanel.gameObject.SetActive(true);
        textline_0.gameObject.SetActive(true);
        textline_1.gameObject.SetActive(true);
        textline_2.gameObject.SetActive(true);
        timeToHideDebugLog = Time.time + timeToDisplayDebugLog;
        PushUpOldTexts();
        textline_0.text = newText;
    }

    private void PushUpOldTexts()
    {
        textline_2.text = textline_1.text;
        textline_1.text = textline_0.text;
    }
}
