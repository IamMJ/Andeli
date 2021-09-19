using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public static class GridModifier : object
{
    static public void ReknitAllGridGraphs(Transform transform)
    {
        var guo = new GraphUpdateObject(UpdateBounds(transform));
        guo.modifyWalkability = true;
        guo.setWalkability = true;
        AstarPath.active.UpdateGraphs(guo);
    }

    static public void UnknitAllGridGraphs(Transform transform)
    {
        var guo = new GraphUpdateObject(UpdateBounds(transform));
        guo.modifyWalkability = true;
        guo.setWalkability = false;
        AstarPath.active.UpdateGraphs(guo);
    }

    static public void UnknitSpecificGridGraph(Transform transform, int graphIndexToUnknit)
    {
        var guo = new GraphUpdateObject(UpdateBounds(transform));
        guo.modifyWalkability = true;
        guo.setWalkability = false;
        guo.nnConstraint.graphMask = 1 << graphIndexToUnknit;
        AstarPath.active.UpdateGraphs(guo);
    }

    static public void ReknitSpecificGridGraph(Transform transform, int graphIndexToReknit)
    {
        var guo = new GraphUpdateObject(UpdateBounds(transform));
        guo.modifyWalkability = true;
        guo.setWalkability = true;
        guo.nnConstraint.graphMask = 1 << graphIndexToReknit;
        AstarPath.active.UpdateGraphs(guo);
    }
    static private Bounds UpdateBounds(Transform transform)
    {
        Bounds bounds = new Bounds(); 
        bounds.center = transform.position;
        bounds.extents = new Vector3(.5f, .5f, 1);
        return bounds;
        
    }
}
