using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BagManager : MonoBehaviour
{
    [SerializeField] Image[] bagImages = null;
    [SerializeField] TextMeshProUGUI[] bagTMPS = null;
    [SerializeField] SpriteRenderer emptyLetterSpriteRef = null;
    WordBuilder wb_player;
    GameController gc;

    //param
    Vector2 wayfar = new Vector2(1000, 1000);

    //state
    LetterTile[] letterTilesInBag;
    int selectedLetterTileIndex = -1;
    bool isButtonDepressed = false;
    float timeButtonDepressed;
    bool wasLongPress = false;


    void Start()
    {
        gc = FindObjectOfType<GameController>();
        gc.OnGameStart += HandleGameStart;
        letterTilesInBag = new LetterTile[bagImages.Length];
        ClearAllLetterIcons();
    }

    private void HandleGameStart()
    {
        wb_player = gc.GetPlayer().GetComponent<WordBuilder>();
    }

    private void ClearAllLetterIcons()
    {
        for (int i = 0; i < letterTilesInBag.Length; i++)
        {
            letterTilesInBag[i] = null;
            ClearLetterSlotInUI(i);
        }
    }

    private void Update()
    {
        if (isButtonDepressed)
        {
            timeButtonDepressed += Time.unscaledDeltaTime;
            if (timeButtonDepressed >= UIParameters.LongPressTime)
            {
                // Long press in bag means erase letter.
                wasLongPress = true;
                ReleaseButton();
            }
        }

    }

    #region Button Handlers
    public void PressDownButtonInitial(int letterIndex)
    {

        if (letterTilesInBag[letterIndex] != null)
        {
            selectedLetterTileIndex = letterIndex;
            isButtonDepressed = true;
        }

    }

    public void ReleaseButton()
    {
        if (!isButtonDepressed)
        {
            return;
        }
        if (wasLongPress)
        {
            DestroySelectedLetterFromSlot(selectedLetterTileIndex);
        }
        else
        {
            if (AttemptToPassLetterToSword(selectedLetterTileIndex))
            {
                //TODO play a whoosh for the letter moving.
            }
            else
            {
                Debug.Log("sword is full!");
                //TODO play a bump sound;
            }
        }
        selectedLetterTileIndex = -1;
        isButtonDepressed = false;
        wasLongPress = false;
        timeButtonDepressed = 0;

    }

    #endregion

    #region Public Methods

    public bool AttemptToReceiveLetter(LetterTile incomingLT)
    {
       int openSlot = -1;
       if (FindEmptyBagSlot(out openSlot))
        {
            letterTilesInBag[openSlot] = incomingLT;
            UpdateUI();
            return true;
        }
       else
        {
            Debug.Log("bag is full!");
            //TODO play a bump sound;
            // TODO shake a bag a bit
            return false;
        }
    }
    #endregion

    #region UI Helpers

    private void ClearLetterSlotInUI(int i)
    {
        bagImages[i].sprite = emptyLetterSpriteRef.sprite;
        bagImages[i].color = emptyLetterSpriteRef.color;
        bagTMPS[i].text = "";
    }

    /// <summary>
    /// This goes through all the letter tile slots. If null, it reverts the sprite, color, and text to the default.
    /// If ref'd, it uses that ref's sprite, color, and text.
    /// </summary>
    private void UpdateUI()
    {
        for (int i = 0; i < letterTilesInBag.Length; i++)
        {
            if (letterTilesInBag[i] != null)
            {
                bagTMPS[i].text = letterTilesInBag[i].Letter.ToString();
                SpriteRenderer sr = letterTilesInBag[i].GetComponent<SpriteRenderer>();
                bagImages[i].sprite = sr.sprite;
                bagImages[i].color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
            }
            else
            {
                ClearLetterSlotInUI(i);
            }


        }
    }

    #endregion

    #region Private Tile Helpers
    private void DestroySelectedLetterFromSlot(int index)
    {
        ParticleSystem.MainModule newMod = bagImages[index].GetComponent<ParticleSystem>().main;
        newMod.startColor = bagImages[index].color;
        bagImages[index].GetComponent<ParticleSystem>().Play();
        letterTilesInBag[index].DestroyLetterTile();
        letterTilesInBag[index] = null;
        UpdateUI();
        
    }

    private bool AttemptToPassLetterToSword(int index)
    {
        if (wb_player.AttemptToReceiveLetterFromBag(letterTilesInBag[index]))
        {
            letterTilesInBag[index] = null;
            UpdateUI();
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region Private Bag Helpers
    private bool FindEmptyBagSlot(out int openSlot)
    {
        
        for (int i = 0; i < letterTilesInBag.Length; i++)
        {
            if (letterTilesInBag[i] == null)
            {
                openSlot = i;
                return true;
            }
        }
        openSlot = -1;
        return false;
    }

    #endregion


    private void OnDestroy()
    {
        gc.OnGameStart -= HandleGameStart;
    }
}

