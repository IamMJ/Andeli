using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridHelper : object
{
    //int horizUnits = 20;
    //int vertUnits = 40;
    static int minX_worldmap = -50;
    static int maxX_worldmap = 50;
    static int minY_worldmap = -50;
    static int maxY_worldmap = 50;
        
    static float gridUnit = 1f;
    static float lowestYValueForMoveInput_BottomPanel = 0.3f;
    static float highestYValueForMoveInput_TopPanel = .86f;

    static float lowestYValueForMoveInput_PermButtons = .2f;
    static float lowestXValueForMoveInput_PermButton_Left = .21f;
    static float highestXValueForMoveInput_PermButton_Right = .79f;

    public static Vector2 SnapToGrid(Vector2 inputPos, int subStepAmt)
    {
        inputPos.x = Mathf.Round(inputPos.x * subStepAmt) / (float)subStepAmt;
        inputPos.y = Mathf.Round(inputPos.y * subStepAmt) / (float)subStepAmt;

        return inputPos;

    }
    
    public static bool CheckIfSnappedToGrid(Vector2 inputPos)
    {
        if (Mathf.Abs(inputPos.x) % 1 > 0 || Mathf.Abs(inputPos.y) % 1 > 0)
        {

            return false;
        }
        else
        {
            return true;
        }
    }


    public static bool CheckIsTouchingWordSection(Vector2 touchPos, bool isInArena, bool isInTutorialMode)
    {
        if (isInArena || isInTutorialMode)
        {
            if (touchPos.y < lowestYValueForMoveInput_BottomPanel * Screen.height || touchPos.y > highestYValueForMoveInput_TopPanel * Screen.height)
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
        else
        {
            if (touchPos.y < lowestYValueForMoveInput_PermButtons * Screen.height)
            {
                if (touchPos.x < lowestXValueForMoveInput_PermButton_Left * Screen.width ||
                    touchPos.x > highestXValueForMoveInput_PermButton_Right * Screen.width)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

    }


    public static Vector2 GetRandomPositionOnWorldMap()
    {
        Vector2 output = new Vector2();
        output.x = Random.Range(minX_worldmap, maxX_worldmap);
        output.y = Random.Range(minY_worldmap, maxY_worldmap);
        return output;
    }
}
