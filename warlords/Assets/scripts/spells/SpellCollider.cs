using Assets.scripts.vo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCollider : MonoBehaviour {

    public List<Minion> MinionList = new List<Minion>();


    private void OnTriggerEnter(Collider minionTransform)
    {

        if (minionTransform.transform.tag == "Enemy")
        {
            getGameLogic().AddAoeMinion(minionTransform.transform);
        
        }
    }

    private void OnTriggerExit(Collider minionTransform)
    {
        if (minionTransform.transform.tag == "Enemy")
        {
            getGameLogic().RemoveAoeMinion(minionTransform.transform);
        }
     
    }


    


    GameLogic getGameLogic()
    {
        if (GameObject.Find("GameLogicObject") != null)
        {
            return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
        }
        else
        {
            return null;
        }
    }


}
