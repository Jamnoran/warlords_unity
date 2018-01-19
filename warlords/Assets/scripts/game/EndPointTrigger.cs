using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.scripts.vo;

public class EndPointTrigger : MonoBehaviour {

    List<int> heroesClickedPortal = new List<int>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider target) {
        if (target.tag == "Hero") {
            Hero heroEntered = getGameLogic().getClosestHeroByPosition(target.transform.position);
            bool alreadyClicked = false;
            foreach (int i in heroesClickedPortal)
            {
                if (i == heroEntered.id)
                {
                    alreadyClicked = true;
                }
            }
            if (!alreadyClicked)
            {
                heroesClickedPortal.Add(heroEntered.id);
                Hero hero = getGameLogic().getMyHero();
                if (heroEntered.id == hero.id)
                {
                    getCommunication().heroHasClickedPortal(hero.id);
                }
            }
        }
    }


    GameLogic getGameLogic() {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }

    ServerCommunication getCommunication() {
        return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
    }
}
