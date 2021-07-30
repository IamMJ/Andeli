using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    //init
    DebugHelper dh;
    WordBoxDriver wbd;
    LetterCollector lc;
    [SerializeField] Collider2D targetHit;

    private enum Intent { Move, EraseWord, FireWord, Unknown };

    //param
    float longPressTime = 0.7f;
    float pressRadius = 1.5f;

    //state
    Vector2 truePosition = Vector2.zero;
    Vector2 desMove = Vector2.zero;
    Vector2 touchStartPos = Vector2.zero;
    Vector2 touchEndPos = Vector2.zero;
    float moveRate = 3f;
    Touch currentTouch;
    Intent intent = Intent.Unknown;
    bool isMobile = false;
    public float timeSpentLongPressing { get; private set; }  

    void Start()
    {
        dh = FindObjectOfType<DebugHelper>();
        wbd = FindObjectOfType<WordBoxDriver>();

        isMobile = Application.isMobilePlatform;
        dh.DisplayDebugLog($"isMobile: {isMobile}");
    }

    // Update is called once per frame
    void Update()
    {
        if (isMobile)
        {
            HandleTouchInput();
        }
        else
        {
            HandleKeyboardInput();
            HandleMouseInput();
        }
        CardinalizeDesiredMovement();
    }

    #region Touch Input
    private void HandleTouchInput()
    { 
        if (Input.touchCount == 0)
        {
            intent = Intent.Unknown;
            return;
        }
        if (Input.touchCount > 0)
        {
            currentTouch = Input.GetTouch(0);
            DetermineTouchIntent();

            switch (intent)
            {
                case Intent.Move:
                    HandleMovementTouch();
                    break;

                case Intent.FireWord:
                    HandleFiringTouch();
                    break;

                case Intent.EraseWord:
                    HandleEraseWordTouch();
                    break;
            }
        }

    }

    private void DetermineTouchIntent()
    {
        //if touching a target, then intent is FireWeapon
        targetHit = null;
        targetHit = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(currentTouch.position), pressRadius, 1 << 10);
        if (targetHit)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.DrawLine(pos, Vector3.up + pos, Color.blue, 2f);
            Debug.DrawLine(pos, Vector3.left + pos, Color.blue, 2f);
            intent = Intent.FireWord;
            dh.DisplayDebugLog("targeting something");
            return;
        }

        //if touching in main area, but not on a target, then intent is Move
        if (GridHelper.CheckIfTouchingWordSection(currentTouch.position) == false)
        {
            intent = Intent.Move;
            dh.DisplayDebugLog("intend to move");
            return;
        }

        else
        {
            intent = Intent.EraseWord;
            dh.DisplayDebugLog("intend to erase word");
            return;
        }
    }
    
    private void HandleMovementTouch()
    {
        switch (currentTouch.phase)
        {
            case TouchPhase.Began:
                touchStartPos = currentTouch.position;
                dh.DisplayDebugLog($"touched here: {touchStartPos}");
                break;

            case TouchPhase.Ended:
                touchEndPos = currentTouch.position;
                //dh.DisplayDebugLog($"ended here: {touchEndPos}");
                desMove = (touchStartPos - touchEndPos).normalized;
                //dh.DisplayDebugLog($"direction: {desMove}");
                break;

        }
    }

    private void HandleEraseWordTouch()
    {
        if (!wbd.HasLetters) { return; }  //can't erase what isn't there. Possibility to fake-erase to gain advantage.
        if (currentTouch.phase == TouchPhase.Began)
        {
            timeSpentLongPressing = 0;
            return;
        }
        if (currentTouch.phase == TouchPhase.Stationary || currentTouch.phase == TouchPhase.Stationary)
        {
            wbd.FillWordEraseSlider(timeSpentLongPressing / longPressTime);
            timeSpentLongPressing += Time.unscaledDeltaTime;
            Time.timeScale = 0;
        }
        if (timeSpentLongPressing >= longPressTime)
        {
            //erase the word
            CompleteLongPress_WordBoxActions();
            return;
        }
        if (currentTouch.phase == TouchPhase.Ended)
        {
            IncompleteLongPress_WordBoxActions();
        }

    }

    private void HandleFiringTouch()
    {
        if (!wbd.HasLetters) { return; }
        if (currentTouch.phase == TouchPhase.Began)
        {
            timeSpentLongPressing = 0;
            return;
        }
        if (currentTouch.phase == TouchPhase.Stationary || currentTouch.phase == TouchPhase.Moved)
        {
            timeSpentLongPressing += Time.unscaledDeltaTime;
            wbd.FillWordFiringSlider(timeSpentLongPressing / longPressTime);
            Time.timeScale = 0;
        }
        if (timeSpentLongPressing >= longPressTime)
        {
            //Fire the word!
            CompleteLongPress_WordBoxActions();
        }
        if (currentTouch.phase == TouchPhase.Ended)
        {
            IncompleteLongPress_WordBoxActions();
        }
    }

    #endregion

    #region Computer Input
    private void HandleKeyboardInput()
    {
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) + Mathf.Abs(Input.GetAxisRaw("Vertical")) <= Mathf.Epsilon)
        {
            return;
        }
        desMove.x = Input.GetAxisRaw("Horizontal");
        desMove.y = Input.GetAxisRaw("Vertical");

    }
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DetermineMouseIntent();            
        }
        switch (intent)
        {
            case Intent.Move:
                //do nothing; movement not handled by mouse, just keyboard
                return;

            case Intent.EraseWord:
                HandleEraseWordMouse();
                return;

            case Intent.FireWord:
                HandleFireWordMouse();
                return;

            case Intent.Unknown:
                //do nothing
                return;
        }
        if (Input.GetMouseButtonUp(0))
        {
            intent = Intent.Unknown;
        }
    }

    private void DetermineMouseIntent()
    {
        //if touching an enemy, then intent is FireWeapon
        targetHit = null;
        targetHit = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition), pressRadius, 1 << 10);
        if (targetHit)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.DrawLine(pos, Vector3.up + pos, Color.blue, 2f);
            Debug.DrawLine(pos, Vector3.left + pos, Color.blue, 2f);
            intent = Intent.FireWord;
            dh.DisplayDebugLog("targeting something");
            return;
        }

        //if touching in main area, but not on a target, then intent is Move
        if (GridHelper.CheckIfTouchingWordSection(Input.mousePosition) == false)
        {
            intent = Intent.Move;
            dh.DisplayDebugLog("intend to move");
            return;
        }

        else
        {
            intent = Intent.EraseWord;
            dh.DisplayDebugLog("intend to erase word");
            return;
        }
    }
    private void HandleEraseWordMouse()
    {
        if (!wbd.HasLetters) { return; }
        if (Input.GetMouseButton(0))
        {
            timeSpentLongPressing += Time.unscaledDeltaTime;
            Time.timeScale = 0;
            wbd.FillWordEraseSlider(timeSpentLongPressing / longPressTime);
        }

        if (timeSpentLongPressing >= longPressTime)
        {
            Debug.Log("Erase off the word");
            //Placeholder for anything related to erasing the word here.
            CompleteLongPress_WordBoxActions();
            return;
        }

        if (Input.GetMouseButtonUp(0))  // Early release catcher
        {
            IncompleteLongPress_WordBoxActions();
        }
    }
    private void HandleFireWordMouse()
    {
        if (!wbd.HasLetters) { return; }
        if (Input.GetMouseButton(0))
        {
            timeSpentLongPressing += Time.unscaledDeltaTime;
            wbd.FillWordFiringSlider(timeSpentLongPressing / longPressTime);
            Time.timeScale = 0;
        }

        if (timeSpentLongPressing >= longPressTime)
        {
            Debug.Log("Fire off the word");
            //Placeholder for whatever action is required when a word if fired off.
            CompleteLongPress_WordBoxActions();
        }
        if (Input.GetMouseButtonUp(0))  // Early release catcher
        {
            IncompleteLongPress_WordBoxActions();
        }
    }

    private void CompleteLongPress_WordBoxActions()
    {
        wbd.ClearOutWordBox();
        IncompleteLongPress_WordBoxActions();
    }

    private void IncompleteLongPress_WordBoxActions()
    {
        wbd.ClearWordEraseSlider();
        wbd.ClearWordFiringSlider();
        timeSpentLongPressing = 0;
        intent = Intent.Unknown;
        Time.timeScale = 1f;
    }



    #endregion

    private void CardinalizeDesiredMovement()
    {

        if (Mathf.Abs(desMove.x) > Mathf.Abs(desMove.y))
        {
            desMove.y = 0;
            if (desMove.x < 0)
            {
                desMove.x = -1;
            }
            else
            {
                desMove.x = 1;
            }
        }
        else
        {
            desMove.x = 0;
            if (desMove.y < 0)
            {
                desMove.y = -1;
            }
            else
            {
                desMove.y = 1;
            }
        }

    }

    #region Handle Movement
    private void FixedUpdate()
    {
        UpdateTruePosition();
        SnapDepictedPositionToTruePositionViaGrid();
    }

    private void UpdateTruePosition()
    {
        truePosition += desMove * moveRate * Time.deltaTime;
    }

    private void SnapDepictedPositionToTruePositionViaGrid()
    {
        Vector2 oldPosition = transform.position;
        transform.position = GridHelper.SnapToGrid(truePosition, 4);
        Vector2 dir = transform.position - (Vector3)oldPosition;
        //if (dir.x > 0)
        //{
        //    dh.DisplayDebugLog("moved right");
        //}
        //if (dir.x < 0)
        //{
        //    dh.DisplayDebugLog("moved left");
        //}
        //if (dir.y < 0)
        //{
        //    dh.DisplayDebugLog("moved down");
        //}
        //if (dir.y > 0)
        //{
        //    dh.DisplayDebugLog("moved up");
        //}

    }
    #endregion
}
