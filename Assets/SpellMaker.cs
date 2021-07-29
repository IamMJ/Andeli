using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellMaker : MonoBehaviour
{
    [SerializeField] GameObject spellPrefab = null;
    LetterCollector lc;

    // Start is called before the first frame update
    void Start()
    {
        lc = GetComponent<LetterCollector>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
