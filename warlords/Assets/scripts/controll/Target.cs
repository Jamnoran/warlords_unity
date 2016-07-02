using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void minionWithinClickDistance(Vector3 click)
    {
        print("Checking if minion is within clicking distance");
        foreach (var minion in ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMinions())
        {
            Vector3 minionPosition = new Vector3(minion.positionX,0.1f,minion.positionZ);
            float dist = Vector3.Distance(minionPosition, click);

            if (dist <= 3.0f)
            {
                print("Clicked on a minion!!: " + dist);
                ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).setHeroTargetEnemy(minion.id);
            }
        }
    }
}
