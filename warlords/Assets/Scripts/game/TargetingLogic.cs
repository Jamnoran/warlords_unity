using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.scripts.spells;

public class TargetingLogic : MonoBehaviour {

    public GameObject activeSpellPrefab;
    private Ability abilityToWaitForMoreInput;
    private float gizmoRadius;
    private Transform gizmoTransform;
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(gizmoTransform.position, gizmoRadius);
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
            int enemy = hero.targetEnemy;
            if (enemy > 0)
            {
                Minion min = getGameLogic().getMinion(enemy);
                if (Vector3.Distance(hero.trans.position, min.minionTransform.position) < abi.range)
                {
                    enemies.Add(enemy);
                    hero.targetFriendly = 0;
                }
                else
                {
                    getNotificationHandler().showNotification(2, "Target out of range");
                    return false;
                }
            }
            else
            {
                getNotificationHandler().showNotification(2, "Didnt find any targets");
                return false;
            }
            
        }
        else if (abi.targetType == "SINGLE_FRIENDLY" || abi.targetType == "HOT")
        {
            friends.Add(hero.targetFriendly);
            hero.targetEnemy = 0;
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
            gizmoRadius = abi.range;
            gizmoTransform = hero.trans;

            Collider[] hitColliders = Physics.OverlapSphere(GameObject.Find("Warrior").transform.position, abi.range);

            foreach (var collider in hitColliders)
            {
                if (collider.tag == "Enemy")
                {
                    var heroTransform = hero.trans;
                    Vector3 enemyRelativePosition = heroTransform.InverseTransformPoint(collider.transform.position);

                    if (enemyRelativePosition.z > 0)
                    {
                        Minion min = getGameLogic().getClosestMinionByPosition(collider.transform.position);
                        enemies.Add(min.id);
                    }
                }

            }

            enemies.ForEach(x => Debug.LogError("enemy id's: " + x));
        }
        else
        {
            enemies.Add(hero.targetEnemy);
            friends.Add(hero.targetFriendly);
        }
        getGameLogic().sendSpell(abi.id, enemies, friends);
        return true;
    }



    private void handleInput()
    {

        //if aoe/targeting spell we wait for mouse to send spell 
        if (Input.GetMouseButtonUp(0))
        {
            try
            {
                //CircleAoe aoeSpell = activeSpellPrefab.GetComponent(typeof(CircleAoe)) as CircleAoe;
                //List<int> enemies = aoeSpell.GetAoeTargets();
                List<int> enemies = new List<int>();
                List<int> friends = new List<int>();

                //destroy spell after use
                Destroy(activeSpellPrefab);

                Hero hero = getGameLogic().getMyHero();
                FieldOfViewAbility fieldOfViewAbility = hero.trans.GetComponent<FieldOfViewAbility>();
                List<int> enemiesInRange = fieldOfViewAbility.FindVisibleTargets(90f, abilityToWaitForMoreInput.range, false);
                if (enemiesInRange != null && enemiesInRange.Count > 0)
                {
                    enemies = enemiesInRange;
                    Debug.Log(enemiesInRange.Count + " targets in range");
                }
                else
                {
                    //getNotificationHandler().showNotification(2, "Didnt find any targets");
                    //Debug.Log("Didnt find any targets");
                }
                getGameLogic().sendSpell(abilityToWaitForMoreInput.id, enemies, friends);
            }
            catch (Exception e)
            {

                throw new Exception("Could not cast spell: " + abilityToWaitForMoreInput.name + "-Error: " + e);
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            try
            {
                Destroy(activeSpellPrefab);
            }
            catch (Exception e)
            {

                throw new Exception("Could not abort casting spell: " + abilityToWaitForMoreInput.name + "-Error: " + e);
            }
        }
        abilityToWaitForMoreInput = null;
    }

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

    NotificationHandler getNotificationHandler()
    {
        return ((NotificationHandler)GameObject.Find("GameLogicObject").GetComponent(typeof(NotificationHandler)));
    }
}
