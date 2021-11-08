using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI_Panel : MonoBehaviour
{
    abstract public void ShowHideElements(bool shouldBeShown);
}
