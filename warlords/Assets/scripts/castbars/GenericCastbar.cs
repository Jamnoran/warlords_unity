using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.scripts.vo;
using Assets.scripts.spells;

public class GenericCastbar : MonoBehaviour {

    private GameObject spell1;
    private GameObject spell2;
    private GameObject spell3;
    private GameObject spell4;
    private GameObject spell5;
    private GameObject spell6;
    private GameObject spell7;

    public Image castBarFiller;
    public GameObject textToUpdate;
    private Text tmpTxt;
    private bool isCasting;
    private float castTime;
    private float timeLeft;

    private GameObject activeSpellPrefab;
    private GameObject activeSpell;

    private void Start()
    {
        spell1 = GameObject.FindGameObjectWithTag("spell1");
        spell2 = GameObject.FindGameObjectWithTag("spell2");
        spell3 = GameObject.FindGameObjectWithTag("spell3");
        spell4 = GameObject.FindGameObjectWithTag("spell4");
        spell5 = GameObject.FindGameObjectWithTag("spell5");
        spell6 = GameObject.FindGameObjectWithTag("spell6");
        spell7 = GameObject.FindGameObjectWithTag("spell7");

        tmpTxt = textToUpdate.GetComponent<Text>();
        //instantiate casting to false, just in case something wierd sets it to true
        isCasting = false;
        //set fillammount to 0, seing how we allways start casting from scratch
        castBarFiller.fillAmount = 0;
      

        //mock casttime:
        castTime = 5.0f;
        timeLeft = castTime;
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var ab1 = GetAbility(spell1);
            SendSpell(spell1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            var ab2 = GetAbility(spell2);
            SendSpell(spell2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SendSpell(spell3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SendSpell(spell4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SendSpell(spell5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SendSpell(spell6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SendSpell(spell7);
        }

	}

    //Decide spelltype and instantiate proper spell
    private void DecideSpellType(string spellType, string spellName)
    {
        switch (spellType)
        {
            case "CONE":
                try
                {
                    //load abilityprefab
                    activeSpellPrefab = Instantiate(Resources.Load("abilities/" + spellName)) as GameObject;
                    break;
                }
                catch (Exception e)
                {

                    throw new Exception("Something went wrong, trying to cast a spell: " + e);
                }
              
            case "SINGLE":
                Debug.Log("Casting single");
                break;
            default:
                Debug.Log("Casting nothing");
                break;
        }
    }

    private Ability GetAbility(GameObject spell)
    {
        return getGameLogic().getAbility(getGameLogic().getAbilityIdByAbilityName(spell.transform.GetChild(0).GetComponent<Image>().sprite.name));
    }
    
    private void SendSpell(GameObject spell)
    {
        // This return false if we dont have a target
        if (!getTargetingLogic().sendSpell(getGameLogic().getAbilityByAbilityName(spell.transform.GetChild(0).GetComponent<Image>().sprite.name)))
        {
            // Show no target message to user
        }
    }


    TargetingLogic getTargetingLogic()
    {
        return ((TargetingLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(TargetingLogic)));
    }

    GameLogic getGameLogic()
    {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }
}
