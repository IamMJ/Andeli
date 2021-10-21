using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class GridHelper : object
{
    //int horizUnits = 20;
    //int vertUnits = 40;
    static int minX_worldmap = -50;
    static int maxX_worldmap = 100;
    static int minY_worldmap = -50;
    static int maxY_worldmap = 90;
        
    static float gridUnit = 1f;
    static float lowestYValueForMoveInput_BottomPanel = 0.3f;
    static float highestYValueForMoveInput_TopPanel = .86f;

    static float lowestYValueForMoveInput_PermButtons = .2f;
    static float lowestXValueForMoveInput_PermButton_Left = .21f;
    static float highestXValueForMoveInput_PermButton_Right = .79f;

    static int layerMask_Impassable = 1 << 13;
    static float pointSize = 0.25f;

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


    public static bool CheckIsTouchingWordSection(Vector2 touchPos, bool isInArena)
    {
        if (isInArena)
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


    public static Vector2 CreateValidRandomPosition()
    {
        Vector2 output = new Vector2();
        output.x = UnityEngine.Random.Range(minX_worldmap, maxX_worldmap);
        output.y = UnityEngine.Random.Range(minY_worldmap, maxY_worldmap);
        return output;
    }

    public static Vector2 CreateValidRandomPosition(Vector2 targetPosition, float range_unidimensional, bool ignoresImpassable)
    {
        Vector2 output = Vector2.zero;

        if (ignoresImpassable)
        {
            output = CreateRandomPositionWithinWorldBoundaries(targetPosition, range_unidimensional);
            return output;
        }
        else
        {
            bool isOccupiedSquare = false;
            int attempts = 0;
            do
            {
                output = CreateRandomPositionWithinWorldBoundaries(targetPosition, range_unidimensional);
                isOccupiedSquare = CheckPositionForImpassable(output);
                attempts++;
                if (attempts > 50)
                {
                    break;
                }
            }
            while (isOccupiedSquare);
            return output;
        }

    }

    private static bool CheckPositionForImpassable(Vector2 testPos)
    {
        var coll = Physics2D.OverlapCircle(testPos, 0.05f, layerMask_Impassable);
        if (coll)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private static Vector2 CreateRandomPositionWithinWorldBoundaries(Vector2 targetPosition, float range_unidimensional)
    {
        Vector2 output = new Vector2();
        output.x = UnityEngine.Random.Range(targetPosition.x - range_unidimensional, targetPosition.x + range_unidimensional);
        output.x = Mathf.Clamp(output.x, minX_worldmap, maxX_worldmap);
        output.y = UnityEngine.Random.Range(targetPosition.y - range_unidimensional, targetPosition.y + range_unidimensional);
        output.y = Mathf.Clamp(output.y, minY_worldmap, maxY_worldmap);
        output = SnapToGrid(output, 1);
        return output;
    }
}
