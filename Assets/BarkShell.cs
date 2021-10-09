using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BarkShell : MonoBehaviour
{
    TextMeshPro tmp;
    Transform followTarget;
    SpriteRenderer sr;

    //param
    Vector3 offsetVector = new Vector3(0, 2,0);
    float fadeoutTime = 1;
    Color defaultTextColor = Color.black;
    Color defaultBGColor = new Color(1,1,1,0.5f);

    //state
    float beginFadeTime;
    float factor = 1;


    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        tmp = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = followTarget.position + offsetVector;
        //if (Time.time > beginFadeTime)
        //{
        //    factor -= (Time.deltaTime / fadeoutTime);
        //    defaultTextColor.a = factor;
        //    tmp.color = defaultTextColor;
        //}
        if (Time.time > beginFadeTime + fadeoutTime)
        {
            DeactivateBark();
        }
    }

    public void ActivateBark(Bark bark, Transform transformToFollow)
    {
        tmp.color = bark.DisplayColor;
        tmp.text = bark.BarkText;
        sr.color = defaultBGColor;
        followTarget = transformToFollow;
        beginFadeTime = Time.time + bark.DisplayTime;
    }

    private void DeactivateBark()
    {
        sr.color = Color.clear;
        tmp.color = Color.clear;
        tmp.text = "";
    }
}
