using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingLogic : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public bool sendSpell(Ability abi)
    {
        Hero hero = getGameLogic().getMyHero();
        List<int> enemies = new List<int>();
        List<int> friends = new List<int>();
        FieldOfViewAbility fieldOfViewAbility = hero.trans.GetComponent<FieldOfViewAbility>();

        // Types of targeting systems (SELF, SINGLE, SINGLE_FRIENDLY, CONE, AOE, DOT, HOT, AOE_POINT, PROJECTILE)
        if (abi.targetType == "SELF")
        {
            friends.Add(getGameLogic().getMyHero().id);
        }
        else if (abi.targetType == "SINGLE" || abi.targetType == "DOT")
        {
            enemies.Add(getGameLogic().getMyHero().targetEnemy);
        }
        else if (abi.targetType == "SINGLE_FRIENDLY" || abi.targetType == "HOT")
        {
            friends.Add(getGameLogic().getMyHero().targetFriendly);
        }
        else if (abi.targetType == "AOE")
        {
            // Check if user is in range of auto attack otherwise set its location as targetPostion
            List<int> enemiesInRange = fieldOfViewAbility.FindVisibleTargets(360f, abi.range, false);
            if (enemiesInRange != null && enemiesInRange.Count > 0)
            {
                enemies = enemiesInRange;
            }
            else
            {
                return false;
            }
        }
        else
        {
            enemies.Add(getGameLogic().getMyHero().targetEnemy);
            friends.Add(getGameLogic().getMyHero().targetFriendly);
        }
        getGameLogic().sendSpell(abi.id, enemies, friends);
        return true;
    }


    GameLogic getGameLogic()
    {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }
}
