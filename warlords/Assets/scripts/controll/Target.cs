using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour {

    public float MinTargetDistance = 3.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // We got a click on a point on map, here we need to handle if its friendly or enemy target or just ground
    // Return true if we found a target
    public bool click(Vector3 click, bool leftClick)
    {
        float closestDistanse = 300.0f;
        foreach (var minion in ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMinions())
        {
            Vector3 minionPosition = new Vector3(minion.positionX,0.1f,minion.positionZ);
            float dist = Vector3.Distance(minionPosition, click);

            if ((dist < closestDistanse) && dist <= MinTargetDistance)
            {
                print("Minion is closes at a distance at: " + dist);
                ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).setHeroTargetEnemy(minion.id);
                closestDistanse = dist;
            }
        }
        foreach (var hero in ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getHeroes())
        {
            Vector3 heroPosition = new Vector3(hero.positionX, 0.1f, hero.positionZ);
            float dist = Vector3.Distance(heroPosition, click);
            Debug.Log("Class: " + hero.class_type + " Distance from click [" + click.x + "x"  + click.z + "] is: " + dist);
            if ((dist < closestDistanse) && dist <= MinTargetDistance)
            {
                print("Hero is closes at a distance at: " + dist);
                ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).setHeroTargetFriendly(hero.id);
                closestDistanse = dist;
            }
        }
        if(closestDistanse < 300.0f) {
            return true;
        }

        if (leftClick)
        {
            ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).setHeroTargetEnemy(0);
            ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).setHeroTargetFriendly(0);
        }
        return false;
    }
}
