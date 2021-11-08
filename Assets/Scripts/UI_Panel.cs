using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI_Panel : MonoBehaviour
{
    [SerializeField] protected GameObject[] elements = null;
     
    public virtual void ShowHideElements(bool shouldBeShown) 
    {
        foreach (var element in elements)
        {
            element.SetActive(shouldBeShown);
        }
    }
}
