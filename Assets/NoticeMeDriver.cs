using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoticeMeDriver : MonoBehaviour
{
    //init

    //param
    Vector3 minY = new Vector3(0,0.7f,0);
    Vector3 maxY = new Vector3(0,1.0f,0);
    float rate = 0.7f;

    //state
    bool isDescending = false;


    void Start()
    {
        transform.localPosition = maxY;
        isDescending = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDescending)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, minY, rate * Time.deltaTime);
            if (transform.localPosition.y <= minY.y)
            {
                isDescending = false;
            }
        }
        else
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, maxY, rate * Time.deltaTime);
            if (transform.localPosition.y >= maxY.y)
            {
                isDescending = true;
            }
        }
    }

    public void ToggleNoticeMe(bool shouldBeShown)
    {
        gameObject.SetActive(shouldBeShown);
    }

}
