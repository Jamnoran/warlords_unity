using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingByFKeys : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            setTarget(0);
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            setTarget(1);
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            setTarget(2);
        }
        else if (Input.GetKeyDown(KeyCode.F4))
        {
            setTarget(3);
        }


    }

    private void setTarget(int position)
    {
        Hero myHero = getGameLogic().getMyHero();
        if (getGameLogic().getHeroes().Count >= position)
        {
            myHero.targetFriendly = getGameLogic().getHeroes()[position].id;
            myHero.targetEnemy = 0;
        }
    }


    GameLogic getGameLogic()
    {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }
}
