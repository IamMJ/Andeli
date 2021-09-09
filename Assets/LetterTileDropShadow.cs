using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterTileDropShadow : MonoBehaviour
{

    Vector3 scaleFactor = new Vector3(1, 1, 0);

    float lifetime;
    float shrinkRate = 0.1f;

    //state
    float life = 0;
    float factor;

    // Update is called once per frame
    void Update()
    {
        transform.localScale -= scaleFactor * shrinkRate * Time.deltaTime;
    }

    public void RemoveShadow()
    {
        Destroy(gameObject);
    }


}
