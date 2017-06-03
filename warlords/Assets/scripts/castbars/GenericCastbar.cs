using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            FindSpellAux(spell1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            FindSpellAux(spell2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            FindSpellAux(spell3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            FindSpellAux(spell4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            FindSpellAux(spell5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            FindSpellAux(spell6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            FindSpellAux(spell7);
        }
        //temp key so we can test this 
        if (Input.GetKeyDown("h"))
        {
            isCasting = true;
        }

        if (isCasting && castBarFiller.fillAmount < 1.0f)
        {
            castBarFiller.fillAmount += 1.0f / castTime * Time.deltaTime;
            timeLeft = timeLeft - Time.deltaTime;
            tmpTxt.text = (Math.Round(timeLeft,2)).ToString();
        }
        else
        {
            resetCastBar();
        }
	}

    private void resetCastBar()
    {
        tmpTxt.text = "";
        isCasting = false;
        castBarFiller.fillAmount = 0;
        timeLeft = castTime;
    }

    private void FindSpellAux(GameObject spell)
    {

        if (spell.transform.childCount > 0)
        {
            List<int> enemies = new List<int>();
            enemies.Add(getGameLogic().getMyHero().targetEnemy);
            List<int> friends = new List<int>();
            friends.Add(getGameLogic().getMyHero().targetFriendly);
            getGameLogic().sendSpell((int)getGameLogic().getAbilityIdByAbilityName(spell.transform.GetChild(0).GetComponent<Image>().sprite.name), enemies, friends);
        }
        else
        {
            Debug.Log("No spell assigned to ");
        }
    }

    GameLogic getGameLogic()
    {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }
}
