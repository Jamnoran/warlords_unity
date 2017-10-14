using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UseAbilitiesActionBar : MonoBehaviour
{
    private GameObject spell1;
    private GameObject spell2;
    private GameObject spell3;
    private GameObject spell4;
    private GameObject spell5;
    private GameObject spell6;
    private GameObject spell7;
    // Use this for initialization
    void Start()
    {
        spell1 = GameObject.FindGameObjectWithTag("spell1");
        spell2 = GameObject.FindGameObjectWithTag("spell2");
        spell3 = GameObject.FindGameObjectWithTag("spell3");
        spell4 = GameObject.FindGameObjectWithTag("spell4");
        spell5 = GameObject.FindGameObjectWithTag("spell5");
        spell6 = GameObject.FindGameObjectWithTag("spell6");
        spell7 = GameObject.FindGameObjectWithTag("spell7");

    }

   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
        
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
      
    }

    private void FindSpellAux(GameObject spell)
    {
   
        if (spell.transform.childCount > 0)
        {
            // This return false if we dont have a target
            //if (!getTargetingLogic().sendSpell(getGameLogic().getAbilityByAbilityName(spell.transform.GetChild(0).GetComponent<Image>().sprite.name)))
            //{
                // Show no target message to user
            //}
        }
        else
        {
            Debug.Log("No spell assigned to ");
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
