using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingByFKeys : MonoBehaviour{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
	public void setTarget(int position)
    {
        Hero myHero = getGameLogic().getMyHero();
        if (getGameLogic().getHeroes().Count > position)
        {
            myHero.targetFriendly = getGameLogic().getHeroes()[position].id;
            myHero.targetEnemy = 0;
        }
        else
        {
            myHero.targetFriendly = 0;
            myHero.targetEnemy = 0;
        }
    }


    GameLogic getGameLogic()
    {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }
}
