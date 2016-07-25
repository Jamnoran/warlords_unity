using UnityEngine;
using System.Collections;
using Assets.scripts.vo;

public class MinionMove : MonoBehaviour
{
    public int minionId = 0;
    public Vector3 desiredPosition;
    public CharacterController controller;
    public bool overrideWalk = false;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (overrideWalk || (minionId > 0 && ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMinion(minionId) != null))
        {
            if (!overrideWalk)
            {
                desiredPosition = ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMinion(minionId).getDesiredPosition();
            }
            
            if(desiredPosition != null)
            { 
                if (Vector3.Distance(transform.position, desiredPosition) > 5)
                {
                    Debug.Log("Distance: " + Vector3.Distance(transform.position, desiredPosition));
                    float newX = transform.position.x;
                    if (transform.position.x < desiredPosition.x) {
                        newX  = transform.position.x + 0.5f;
                    } else if (transform.position.x > desiredPosition.x)
                    {
                        newX = transform.position.x - 0.5f;
                    }
                    float newZ = transform.position.z;
                    if (transform.position.z < desiredPosition.z)
                    {
                        newZ = transform.position.z + 0.5f;
                    }
                    else if (transform.position.z > desiredPosition.z)
                    {
                        newZ = transform.position.z - 0.5f;
                    }
                    Debug.Log("New x " +newX + " new z " + newZ);
                    transform.position.Set(newX, transform.position.y, newZ);
                    
                    //rotate our hero towards the desired position so we allways run forward
                    //Quaternion newRotation = Quaternion.LookRotation(desiredPosition - controller.transform.position);
                    //newRotation.x = 0f;
                    //newRotation.z = 0f;
                    //controller.transform.rotation = Quaternion.Slerp(controller.transform.rotation, newRotation, Time.deltaTime * 10);

                    //call our charactercontroller to move forward
                    //controller.SimpleMove(controller.transform.forward);
                }
            }
        }
    }
}
