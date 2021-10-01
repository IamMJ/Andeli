using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTag : MonoBehaviour
{
    public Vector2 offsetForArrow = Vector2.zero;
    public float rotationForArrow = 0;

    public enum TagName {Nothing, TutorialOkay, OptionMenu, ThirdEnergyBar,
        PowersMenu, FireButton, EraseButton, SecondLetterTile, WorldSpacePos, PlayerInWS, FloatBelowVictoryMeter };

    public TagName tagName = TagName.Nothing;
}
