using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class LetterTileDropShadow : MonoBehaviour
{
    Vector3 scaleFactor = new Vector3(1, 1, 0);
    Bounds bounds;

    float shrinkRate = 0.1f;


    private void Start()
    {
        GridModifier.UnknitAllGridGraphs(transform);
    }

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
