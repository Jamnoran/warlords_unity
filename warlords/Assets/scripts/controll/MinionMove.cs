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
	void Update ()
    {
        //if ((minionId > 0 && ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMinion(minionId) != null))
        //{
        //populate desiredPosition with a vector3
        //desiredPosition = ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMinion(minionId).getDesiredPosition();

        //       if (desiredPosition != null)
        //{
        //Debug.Log("Distance: " + Vector3.Distance(transform.position, desiredPosition));
        //if minion is not too close we can move
        //if (Vector3.Distance(transform.position, desiredPosition) > 3)
        //{
        //getAnimation().setDesiredLocation(desiredPosition);
        //}
        //          else
        //{
        //Debug.Log("Hero is now in range, we should stop moving and send attack command to server from minion");
        //getAnimation().stopMovement();
        //getAnimation().sendAttackInRange();
        //}
        //}
        //}
    }

    MinionAnimations getAnimation()
    {
        return (MinionAnimations)transform.GetComponent(typeof(MinionAnimations));
    }
}
