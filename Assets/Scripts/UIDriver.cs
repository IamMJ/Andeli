using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class UIDriver : MonoBehaviour
{
    //init
    [SerializeField] GameObject pauseMenuPrefab = null;

    [SerializeField] Slider victoryBarSlider = null;
    [SerializeField] GameObject topBarPanel = null;
    [SerializeField] GameObject bottomBarPanel = null;

    [SerializeField] Slider wordEraseSliderBG = null;
    [SerializeField] Slider wordFiringSliderBG = null;
    [SerializeField] TextMeshProUGUI powerMeterTMP = null;

    [SerializeField] Sprite blankTileDefault = null;
    [SerializeField] Image[] wordboxImages = null;
    [SerializeField] TextMeshProUGUI[] wordboxTMPs = null;

    [SerializeField] Slider spellEnergySlider = null;
    [SerializeField] Image spellEnergyFillImage = null;

    WordBuilder playerWB;
    WordWeaponizer playerWWZ;
    GameController gc;
    SceneLoader sl;
    GameObject pauseMenu;

    //param
    float panelDeployRate = 100f; // pixels per second
    float topPanelShown_Y = -77f;
    float topPanelHidden_Y = 100f;

    //state
    bool isFireWeaponButtonPressed = false;
    bool isEraseWeaponButtonPressed = false;
    float timeButtonDepressed = 0;
    public float timeSpentLongPressing { get; private set; }

    Color noCast = Color.red;
    Color oneCast = new Color(1f, 0.64f, 0);
    Color twoCast = Color.yellow;
    Color threeCast = new Color(0.443f, 1f, 0);
    Color fourCast = Color.green;

    private void Start()
    {
        ClearWordBar();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerWB)
        {
            HandleFireWordButtonPressed();
            HandleEraseWordButtonPressed();
        }
    }

    public void SetPlayerObject(WordBuilder newPlayerWB, WordWeaponizer newPlayerWWZ)
    {
        playerWB = newPlayerWB;
        playerWWZ = newPlayerWWZ;
    }

    #region Initial Button Press Handlers
    public void OnPressDownFireWord()
    {
        if (playerWB.GetCurrentWordLength() == 0) { return; }


        if (isEraseWeaponButtonPressed == false)
        {
            isFireWeaponButtonPressed = true;
        }

    }

    public void OnReleaseFireWord()
    {
        IncompleteLongPress_WordBoxActions();
    }

    public void OnPressDownEraseWord()
    {
        if (playerWB.GetCurrentWordLength() == 0) { return; }

        if (isFireWeaponButtonPressed == false)
        {
            isEraseWeaponButtonPressed = true;
        }
    }

    public void OnReleaseEraseWord()
    {
        IncompleteLongPress_WordBoxActions();
    }
    #endregion

    private void HandleEraseWordButtonPressed()
    {
        if (isEraseWeaponButtonPressed)
        {
            timeButtonDepressed += Time.unscaledDeltaTime;
            Time.timeScale = 0;
            FillWordEraseSlider(timeButtonDepressed / UIParameters.LongPressTime);
            if (timeButtonDepressed >= UIParameters.LongPressTime)
            {
                //Placeholder for anything related to erasing the word
                CompleteLongPress_WordBoxActions();
            }
        }
    }

    private void HandleFireWordButtonPressed()
    {
        if (isFireWeaponButtonPressed)
        {
            timeButtonDepressed += Time.unscaledDeltaTime;
            Time.timeScale = 0;
            FillWordFiringSlider(timeButtonDepressed / UIParameters.LongPressTime);
            if (timeButtonDepressed >= UIParameters.LongPressTime)
            {
                if (playerWWZ.AttemptToFireWord())
                {
                    CompleteLongPress_WordBoxActions();
                }                
            }
        }
    }

    public void FillWordEraseSlider(float amount)
    {
        wordEraseSliderBG.value = amount;
    }

    public void ClearWordEraseSlider()
    {
        wordEraseSliderBG.value = 0f;
    }

    public void FillWordFiringSlider(float amount)
    {
        wordFiringSliderBG.value = amount;
    }

    public void ClearWordFiringSlider()
    {
        wordFiringSliderBG.value = 0;
    }


    private void CompleteLongPress_WordBoxActions()
    {
        playerWB.ClearCurrentWord();
        playerWB.ClearPowerLevel();
        IncompleteLongPress_WordBoxActions();
    }
    private void IncompleteLongPress_WordBoxActions()
    {
        ClearWordEraseSlider();
        ClearWordFiringSlider();
        timeButtonDepressed = 0;
        Time.timeScale = 1f;
        isFireWeaponButtonPressed = false;
        isEraseWeaponButtonPressed = false;

    }

    public void ModifyPowerMeterTMP(int valueToShow)
    {
        powerMeterTMP.text = valueToShow.ToString();
    }

    public void EnterOverworld()
    {
        ShowHideBottomPanel(false);
        ShowHideTopPanel(false);
        ShowHideVictoryMeter(false);
    }

    public void EnterArena()
    {
        ShowHideBottomPanel(true);
        ShowHideTopPanel(true);
        ShowHideVictoryMeter(true);
    }


    private void ShowHideTopPanel(bool shouldBeShown)
    {
        //StartCoroutine(ShowHideTopPanel_Coroutine(shouldBeShown));
        topBarPanel.SetActive(shouldBeShown);
    }

    private void ShowHideBottomPanel(bool shouldBeShown)
    {
        bottomBarPanel.SetActive(shouldBeShown);
    }

    private void ShowHideVictoryMeter(bool shouldBeShown)
    {
        victoryBarSlider.gameObject.SetActive(shouldBeShown);
    }

    public void ShowPauseMenu()
    {
        if (!gc)
        {
            gc = FindObjectOfType<GameController>();
        }
        gc.PauseGame();
        if (!pauseMenu)
        {
            pauseMenu = Instantiate(pauseMenuPrefab);
        }
        if (pauseMenu)
        {
            pauseMenu.SetActive(true);
        }

    }


    public void AddLetterToWordBar(Sprite letterTileSprite, char letter, int indexInWord)
    {
        wordboxImages[indexInWord].sprite = letterTileSprite;
        wordboxTMPs[indexInWord].text = letter.ToString();
    }

    public void ClearWordBar()
    {
        foreach(var image in wordboxImages)
        {
            image.sprite = blankTileDefault;
            if (image.gameObject.transform.childCount > 0)
            {
                Destroy(image.gameObject.transform.GetChild(0).gameObject);
            }
        }
        foreach(var TMP in wordboxTMPs)
        {
            TMP.text = "";
        }
    }

    public GameObject GetGameObjectAt(int index)
    {
        return wordboxImages[index].gameObject;
    }

    public void UpdateSpellEnergySlider( float currentEnergy)
    {
        spellEnergySlider.value = currentEnergy;
        float factor = currentEnergy / spellEnergySlider.maxValue;
        if (factor >= 1f)
        {
            spellEnergyFillImage.color = fourCast;
            return;
        }
        if (factor >= 0.75f)
        {
            spellEnergyFillImage.color = threeCast;
            return;
        }
        if (factor >= 0.5f)
        {
            spellEnergyFillImage.color = twoCast;
            return;
        }
        if (factor >= 0.25f)
        {
            spellEnergyFillImage.color = oneCast;
            return;
        }
        if (factor < 0.25f)
        {
            spellEnergyFillImage.color = noCast;
            return;
        }

    }

    public void SetSpellEnergySliderMaxValue(float maxEnergy)
    {
        spellEnergySlider.maxValue = maxEnergy;
    }

    //IEnumerator ShowHideTopPanel_Coroutine(bool shouldBeShown)
    //{
    //    float value = topBarPanel.anchoredPosition.y;
    //    Debug.Log($"value: {value}");
    //    if (shouldBeShown)
    //    {
    //        while (topBarPanel.anchoredPosition.y > topPanelShown_Y)
    //        {
    //            Debug.Log($"value: {value}, target is {topPanelShown_Y}");
    //            value = Mathf.MoveTowards(value, topPanelShown_Y, panelDeployRate * Time.unscaledDeltaTime);
    //            topBarPanel.position = new Vector2(0, value);
    //            yield return new WaitForEndOfFrame();
    //        }
    //    }
    //    else
    //    {
    //        while (topBarPanel.anchoredPosition.y < topPanelHidden_Y)
    //        {
    //            Debug.Log($"value: {value}, target is {topPanelHidden_Y}");
    //            value = Mathf.MoveTowards(value, topPanelHidden_Y, panelDeployRate * Time.unscaledDeltaTime);
    //            topBarPanel.position = new Vector2(0, value);
    //            yield return new WaitForEndOfFrame();
    //        }
    //    }
    //}

}
