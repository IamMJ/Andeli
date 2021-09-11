using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordWeaponizer : MonoBehaviour
{
    [SerializeField] GameObject puffPrefab = null;
    [SerializeField] GameObject normalSpellPrefab = null;
    [SerializeField] GameObject freezeSpellPrefab = null;
    List<GameObject> spellsInFlight = new List<GameObject>();

    GameController gc;
    WordBuilder wbd;
    DebugHelper dh;
    WordValidater wv;
    PowerMeter pm;
    VictoryMeter vm;
    PlayerMemory playmem;
    ArenaBuilder ab;
    ArenaLetterEffectsHandler aleh;
    string testWord;

    public enum SpellType { Normal, Freeze };

    //param
    float spellInitialSpeed = 6.0f;
    public GameObject currentEnemy;


    void Start()
    {
        playmem = GetComponent<PlayerMemory>();
        wbd = GetComponent<WordBuilder>();

        gc = FindObjectOfType<GameController>();
        dh = FindObjectOfType<DebugHelper>();
        wv = FindObjectOfType<WordValidater>();
        vm = FindObjectOfType<VictoryMeter>();
        ab = FindObjectOfType<ArenaBuilder>();
    }
    public bool AttemptToFireWord()
    {
        testWord = wbd.GetCurrentWord();
        if (wv.CheckWordValidity(testWord))
        {
            playmem.IncrementWordCount();
            GameObject puff = Instantiate(puffPrefab, transform.position, Quaternion.identity) as GameObject;
            WordPuff wordPuff = puff.GetComponent<WordPuff>();
            wordPuff.SetText(testWord);
            wordPuff.SetColorByPower(wbd.CurrentPower);
            if (vm.ModifyBalanceAndCheckForArenaEnd(wbd.CurrentPower))
            {
                //Technically a valid word, but the arena is over now and shouldn't count this word
                return false;
            }
            else
            {
                FireKnownValidWord();
                return true;
            }

        }
        else
        {
            playmem.ResetConsecutiveWordCount();
            Debug.Log("stun the player.");

            // stun the player

            return false;
        }
    }

    public void FireKnownValidWord()
    {
        TargetBestEnemy();
        CreateSpell(transform, currentEnemy.transform, WordWeaponizer.SpellType.Normal);

        foreach (var letter in wbd.GetLettersCollected())
        {
            if (letter.GetLatentAbilityStatus() == false)
            {
                continue;
            }
            if (!aleh)
            {
                ab = FindObjectOfType<ArenaBuilder>();
                aleh = ab.GetComponent<ArenaLetterEffectsHandler>();
            }
            aleh.ApplyLetterEffectOnFiring(letter, gameObject, currentEnemy);
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

    private void TargetBestEnemy()
    {
        if (GetComponent<PlayerInput>())
        {
            if (!ab)
            {
                ab = FindObjectOfType<ArenaBuilder>();
            }
            currentEnemy = ab.GetEnemiesInArena()[0];
        }
        else
        {
            currentEnemy = gc.GetPlayer();
        }
    }

}
