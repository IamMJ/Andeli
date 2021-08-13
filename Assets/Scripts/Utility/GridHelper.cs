using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridHelper : object
{
    //int horizUnits = 20;
    //int vertUnits = 40;
    static float gridUnit = 1f;
    static float highestYValueForWordSection = 0.21f;

    public static Vector2 SnapToGrid(Vector2 inputPos, int subStepAmt)
    {
        inputPos.x = Mathf.Round(inputPos.x * subStepAmt) / (float)subStepAmt;
        inputPos.y = Mathf.Round(inputPos.y * subStepAmt) / (float)subStepAmt;

        return inputPos;

    }
    
    public static bool CheckIfSnappedToGrid(Vector2 inputPos)
    {
        if (Mathf.Abs(inputPos.x) % 1 > 0 || Mathf.Abs(inputPos.y) % 1 < 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    public static bool CheckIsTouchingWordSection(Vector2 touchPos)
    {
        if (touchPos.y < highestYValueForWordSection * Screen.height)
        {
            //Debug.Log($"touched at {touchPos}. word section top is {highestYValueForWordSection*Screen.height}. true");
            return true;
        }
        else
        {
            //Debug.Log($"touched at {touchPos}. word section top is {highestYValueForWordSection * Screen.height}. false");
            return false;
        }
    }

}
