using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridHelper : object
{
    //int horizUnits = 20;
    //int vertUnits = 40;
    static float gridUnit = 1;

    public static Vector2 SnapToGrid(Vector2 inputPos)
    {
        Vector2 gridSnappedPos = new Vector2(Mathf.Round(inputPos.x / gridUnit), Mathf.Round(inputPos.y / gridUnit));
        return gridSnappedPos;
    }

}
