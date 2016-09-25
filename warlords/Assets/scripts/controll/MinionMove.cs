using UnityEngine;
using System.Collections;
using Assets.scripts.vo;

public class MinionMove : MonoBehaviour
{
    public int minionId = 0;
    public Vector3 desiredPosition;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if ((minionId > 0 && ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMinion(minionId) != null))
        {
            //populate desiredPosition with a vector3
            desiredPosition = ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMinion(minionId).getDesiredPosition();
            
            if(desiredPosition != null)
            { 
                //if minion is not too close we can move
                if (Vector3.Distance(transform.position, desiredPosition) > 5)
                {
                    Debug.Log("Distance: " + Vector3.Distance(transform.position, desiredPosition));
                    getAnimation().setDesiredLocation(desiredPosition);
                }
            }
        }
    }

    MinionAnimations getAnimation()
    {
        return (MinionAnimations)transform.GetComponent(typeof(MinionAnimations));
    }
}
