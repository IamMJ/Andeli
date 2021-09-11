using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class LetterTileDropShadow : MonoBehaviour
{
    GraphUpdateScene gus;
    Vector3 scaleFactor = new Vector3(1, 1, 0);

    float lifetime;
    float shrinkRate = 0.1f;

    //state
    float life = 0;
    float factor;

    private void Start()
    {
        gus = GetComponent<GraphUpdateScene>();
        UnknitGridGraph();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale -= scaleFactor * shrinkRate * Time.deltaTime;
    }
    private void UnknitGridGraph()
    {
        gus.setWalkability = false;
        gus.Apply();
    }

    public void RemoveShadow()
    {
        Destroy(gameObject);
    }


}
