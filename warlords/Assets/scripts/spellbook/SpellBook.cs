using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.scripts.vo;

public class SpellBook : MonoBehaviour {
    private List<Ability> abilities = null;

    void Start()
    {
     
        abilities = getGameLogic().getAbilities();
        Debug.Log("========================================");
        Debug.Log(abilities[0]);
        Debug.Log("========================================");


    }

    GameLogic getGameLogic()
    {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }

    

 
}
