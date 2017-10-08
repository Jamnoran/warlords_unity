using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TargetingLogic : MonoBehaviour {

    private Ability abilityToWaitForMoreInput;

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

        // TODO: Need to check range of abilities else give error message

        // Types of targeting systems (SELF, SINGLE, SINGLE_FRIENDLY, CONE, AOE, DOT, HOT, AOE_POINT, PROJECTILE)
        if (abi.targetType == "SELF")
        {
            friends.Add(getGameLogic().getMyHero().id);
        }
        else if (abi.targetType == "SINGLE" || abi.targetType == "DOT")
        {
            enemies.Add(getGameLogic().getMyHero().targetEnemy);
            getGameLogic().getMyHero().targetFriendly = 0;
            
        }
        else if (abi.targetType == "SINGLE_FRIENDLY" || abi.targetType == "HOT")
        {
            friends.Add(getGameLogic().getMyHero().targetFriendly);
            getGameLogic().getMyHero().targetEnemy = 0;
        }
        else if (abi.targetType == "AOE")
        {
            List<int> enemiesInRange = fieldOfViewAbility.FindVisibleTargets(360f, abi.range, false);
            if (enemiesInRange != null && enemiesInRange.Count > 0)
            {
                enemies = enemiesInRange;
            } else {
                return false;
            }
        }
        else if (abi.targetType == "CONE")
        {
            List<int> enemiesInRange = fieldOfViewAbility.FindVisibleTargets(90f, abi.range, false);
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

    private void handleInput()
    {

        //if aoe/targeting spell we wait for mouse to send spell 
        //if (Input.GetMouseButtonUp(0))
        //{
        //    try
        //    {
        //        CircleAoe aoeSpell = activeSpellPrefab.GetComponent(typeof(CircleAoe)) as CircleAoe;
        //        List<int> friendlies = new List<int>();
        //        List<int> enemies = aoeSpell.GetAoeTargets();

        //        //send active spell along with list of friendlies and enemies
        //        NewSendSpell(activeSpell, enemies, friendlies);

        //        //destroy spell after use
        //        Destroy(activeSpellPrefab);
        //    }
        //    catch (Exception e)
        //    {

        //        throw new Exception("Could not cast spell: " + activeSpell.name + "-Error: " + e);
        //    }
        //}

        //if (Input.GetMouseButtonUp(1))
        //{
        //    try
        //    {
        //        Destroy(activeSpellPrefab);
        //    }
        //    catch (Exception e)
        //    {

        //        throw new Exception("Could not abort casting spell: " + activeSpell.name + "-Error: " + e);
        //    }
        //}

        //cancel spell when rightclicking


    }

    // Temp for castBar


    //if (isCasting && castBarFiller.fillAmount< 1.0f)
    // {
    //castBarFiller.fillAmount += 1.0f / castTime* Time.deltaTime;
    //timeLeft = timeLeft - Time.deltaTime;
    //tmpTxt.text = (Math.Round(timeLeft,2)).ToString();
    //}
    //      else
    //      {
    //resetCastBar();
    //}


    //private void resetCastBar()
    //{
    //    tmpTxt.text = "";
    //    isCasting = false;
    //    castBarFiller.fillAmount = 0;
    //    timeLeft = castTime;
    //}


    private void rotateToPosition(Vector3 vector3)
    {
        Debug.Log("Rotate towards : " + vector3);
        CharacterAnimations anim = (CharacterAnimations)getGameLogic().getMyHero().trans.GetComponent(typeof(CharacterAnimations));
        anim.rotateToTarget(vector3);
    }

    GameLogic getGameLogic()
    {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }
}
