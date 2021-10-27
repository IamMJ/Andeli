using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class BarkShell : MonoBehaviour
{
    TextMeshPro tmp;
    Transform followTarget;
    RectTransform tmpRT;
    SpriteRenderer sr;

    //param
    [SerializeField] Vector3 offsetVector = new Vector3(0, 1,0);
    float fadeoutTime = 1;
    Color defaultTextColor = Color.black;
    Color defaultBGColor = new Color(1,1,1,0.5f);
    float defaultImageSize_X = 3f;

    //state
    Bark currentBark;
    float beginFadeTime;
    float factor = 1;


    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        tmp = GetComponent<TextMeshPro>();
        tmpRT = GetComponent<RectTransform>();
        DeactivateBark();
    }

    // Update is called once per frame
    void Update()
    {
        if (followTarget)
        {
            transform.position = followTarget.position + offsetVector;
        }

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
        if (bark == null) { return; }

        currentBark = bark;
        tmp.color = currentBark.DisplayColor;
        tmp.SetText(currentBark.BarkText);
        sr.color = defaultBGColor;
        RescaleImageToFitText();

        if (transformToFollow)
        {
            followTarget = transformToFollow;
        }
        beginFadeTime = Time.time + bark.DisplayTime;
    }

    private void RescaleImageToFitText()
    {

        int estimate = Mathf.RoundToInt(tmp.text.Length / 3f);
        sr.size = new Vector2(estimate, 1);
        tmpRT.sizeDelta = sr.size;
        //tmp.SetText(currentBark.BarkText);

        //int count = 0;
        //if (tmp.isTextOverflowing)
        //{
        //    Debug.Log("going up a size");
        //    sr.size = new Vector2(estimate + 1, 1);
        //    tmpRT.sizeDelta = sr.size;
        //    tmp.SetText(currentBark.BarkText);
        //}

        //do
        //{
        //    count++;
        //    if (count > 10)
        //    {
        //        Debug.Log("loop breaker");
        //        break;
        //    }

        //}
        //while (tmp.isTextOverflowing);

    }

    private void DeactivateBark()
    {
        sr.color = Color.clear;
        tmp.color = Color.clear;
        tmp.text = "";
    }
}
