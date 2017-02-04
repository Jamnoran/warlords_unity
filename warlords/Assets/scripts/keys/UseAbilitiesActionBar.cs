using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UseAbilitiesActionBar : MonoBehaviour
{
    public GameObject spell1;
    public GameObject spell2;
    public GameObject spell3;
    public GameObject spell4;
    public GameObject spell5;
    public GameObject spell6;
    public GameObject spell7;
    public GameObject spell8;
    public GameObject spell9;
    public GameObject spell10;
    public GameObject spell11;
    // Use this for initialization
    void Start()
    {
        
        spell2 = GameObject.Find("spell2");
        spell3 = GameObject.Find("spell3");
        spell4 = GameObject.Find("spell4");
        spell5 = GameObject.Find("spell5");
        spell6 = GameObject.Find("spell6");
        spell7 = GameObject.Find("spell7");
        spell8 = GameObject.Find("spell8");
        spell9 = GameObject.Find("spell9");
        spell10 = GameObject.Find("spell10");
        spell11 = GameObject.Find("spell11");
    }

   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            FindSpellAux(spell1, "spell1");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            FindSpellAux(spell2, "spell2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            FindSpellAux(spell3, "spell3");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            FindSpellAux(spell4, "spell4");
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            FindSpellAux(spell5, "spell5");
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            FindSpellAux(spell6, "spell6");
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            FindSpellAux(spell7, "spell7");
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            FindSpellAux(spell8, "spell8");
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            FindSpellAux(spell9, "spell9");
        }
    }

    private void FindSpellAux(GameObject spell, string spellString)
    {
        spell = GameObject.Find(spellString);
        if (spell.transform.childCount > 0)
        {
            List<int> enemies = new List<int>();
            enemies.Add(getGameLogic().getMyHero().targetEnemy);
            List<int> friends = new List<int>();
            friends.Add(getGameLogic().getMyHero().targetFriendly);
            getGameLogic().sendSpell((int)getGameLogic().getAbilityIdByAbilityName(spell.transform.GetChild(0).name), enemies, friends);
        }
        else
        {
            Debug.Log("No spell assigned to " + spellString);
        }
    }

    GameLogic getGameLogic()
    {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }
}
