using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFollowable
{

    void DropBreadcrumb();

    Vector2 GetOldestBreadcrumb();
}
