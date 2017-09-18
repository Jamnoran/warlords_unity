using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.scripts.vo;

public class EndPointTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider target) {
        if (target.tag == "Hero") {
            Hero heroEntered = getGameLogic().getClosestHeroByPosition(target.transform.position);
            Hero hero = getGameLogic().getMyHero();
            if (heroEntered.id == hero.id) {
                FieldOfView field = ((FieldOfView)hero.trans.Find(hero.getModelName()).GetComponent(typeof(FieldOfView)));
                if (field.isPortalInRange()) {
                    Debug.Log("Stair was in range");
                }
                getCommunication().heroHasClickedPortal(hero.id);
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
