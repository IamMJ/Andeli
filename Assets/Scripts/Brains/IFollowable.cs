using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IFollowable
{

    void DropBreadcrumb();

    Vector2 GetOldestBreadcrumb();
}
