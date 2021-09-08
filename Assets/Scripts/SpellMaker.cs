using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellMaker : MonoBehaviour
{
    [SerializeField] GameObject puffPrefab = null;
    [SerializeField] GameObject normalSpellPrefab = null;
    [SerializeField] GameObject freezeSpellPrefab = null;
    List<GameObject> spellsInFlight = new List<GameObject>();

    WordBuilder wbd;
    DebugHelper dh;
    WordValidater wv;
    PowerMeter pm;
    VictoryMeter vm;
    PlayerMemory playmem;
    string testWord;

    public enum SpellType { Normal, Freeze };

    //param
    float spellInitialSpeed = 6.0f;


    void Start()
    {
        playmem = GetComponent<PlayerMemory>();
        wbd = GetComponent<WordBuilder>();

        dh = FindObjectOfType<DebugHelper>();
        wv = FindObjectOfType<WordValidater>();
        vm = FindObjectOfType<VictoryMeter>();
        
    }
    public bool FireCurrentWordIfValid()
    {
        testWord = wbd.GetCurrentWord();
        if (wv.CheckWordValidity(testWord))
        {
            GameObject puff = Instantiate(puffPrefab, transform.position, Quaternion.identity) as GameObject;
            WordPuff wordPuff = puff.GetComponent<WordPuff>();
            wordPuff.SetText(testWord);
            wordPuff.SetColorByPower(wbd.CurrentPower);
            playmem.IncrementWordCount();
            vm.ModifyBalance(wbd.CurrentPower);
            return true;
        }
        else
        {
            playmem.ResetConsecutiveWordCount();
            Debug.Log("stun the player.");

            // stun the player

            return false;
        }
    }

    public void CreateSpell(Transform source, Transform target, SpellType spellType)
    {
        float amount = UnityEngine.Random.Range(-180f, 179f);
        Quaternion randRot = Quaternion.Euler(0, 0, amount);
        GameObject spell;

        switch (spellType)
        {
            case SpellType.Normal:
                spell = Instantiate(normalSpellPrefab, source.position, randRot);
                spell.GetComponent<Rigidbody2D>().velocity = spell.transform.up * spellInitialSpeed;
                spell.GetComponent<SpellSeeker>().SetTarget(target);
                spellsInFlight.Add(spell);
                return;

            case SpellType.Freeze:
                spell = Instantiate(freezeSpellPrefab, source.position, randRot);
                spell.GetComponent<Rigidbody2D>().velocity = spell.transform.up * spellInitialSpeed;
                spell.GetComponent<SpellSeeker>().SetTarget(target);
                spellsInFlight.Add(spell);
                return;
        }
    }

    public void RemoveAllSpellsInFlight()
    {
        for (int i = 0; i < spellsInFlight.Count; i++)
        {
            Destroy(spellsInFlight[i]);            
        }
        spellsInFlight.Clear();
    }


}
