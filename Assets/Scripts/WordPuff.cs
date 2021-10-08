using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WordPuff : MonoBehaviour
{
    //init
    TextMeshPro tmp;
    RectTransform rt;


    //param
    float StartingLifetime = 3;

    //state
    float lifetimeRemaining;
    float factor;

    // Start is called before the first frame update
    void Awake()
    {
        tmp = GetComponent<TextMeshPro>();
        tmp.text = "";
        lifetimeRemaining = StartingLifetime;
        factor = 1;
        rt = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        lifetimeRemaining -= Time.deltaTime;
        factor = lifetimeRemaining / StartingLifetime;
        tmp.color = new Color(1, 1, 1, factor);
        if (factor <= 0)
        {
            Destroy(gameObject);
        }
        rt.position = new Vector3(rt.position.x, rt.position.y + Time.deltaTime, 0);
    }

    public void SetText(string text)
    {
        tmp.text = text;
    }

    public void SetColorByPower(float power)
    {
        if (power >= 10)
        {
            tmp.color = Color.red;
            return;
        }
        if (power >= 5)
        {
            tmp.color = Color.yellow;
            return;
        }
        else
        {
            tmp.color = Color.white;
        }

    }
}
