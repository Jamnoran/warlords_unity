using UnityEngine;
using System.Collections;

public class CharacterAnimations : MonoBehaviour {
    public GameObject character;
    private Animator anim;
    public Vector3 targetPosition;
    bool isMoving;
    private Rigidbody rbody;
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 3.0F;
    public bool sentStopAnimation = false;

	// Use this for initialization
	void Start () {
        anim = character.GetComponent<Animator>();
        rbody = character.GetComponent<Rigidbody>();
	}

    // Update is called once per frame
    void Update()
    {
        //if (getGameLogic().isMyHeroAlive())
        //{
            //start moving the player towards the desired position
            Vector3 targetPostition = new Vector3(targetPosition.x, character.transform.position.y, targetPosition.z);
            character.transform.LookAt(targetPostition);


            // find the target position relative to the player:
            Vector3 dir = targetPosition - transform.position;
            // ignore any height difference:
            dir.y = 0;
            // calculate velocity limited to the desired speed:
            Vector3 velocity = Vector3.ClampMagnitude(dir * moveSpeed, moveSpeed);
            // move the character including gravity:
            CharacterController controller = (CharacterController)GetComponent(typeof(CharacterController));
            controller.SimpleMove(velocity);


            //if we are at the desired position we must stop moving
            if (Vector3.Distance(character.transform.position, targetPosition) < 0.2f)
            {
                isMoving = false;
            }

            if (!isMoving && !sentStopAnimation && getGameLogic().getClosestHeroByPosition(character.transform.position).id == getGameLogic().getMyHero().id)
            {
                sentStopAnimation = true;
                getCommunication().sendStopHero(getGameLogic().getMyHero().id);
            }
        //}else { 
            // Play dead animation
        //}
    }

    public void rotateToTarget(Vector3 postition)
    {
        Debug.Log("Rotating towards target");
        Vector3 rotatingPostition = new Vector3(postition.x, character.transform.position.y, postition.z);
        character.transform.LookAt(rotatingPostition);

    }

    public void attackAnimation()
    {
        anim.Play("auto", -1, 0f);
    }

    public void runAnimation()
    {
        anim.Play("walk", -1, 0f);
    }

    public void idleAnimation()
    {
        Debug.Log("Starting idle animation again");
        anim.Play("idle", -1, 0f);
    }

    public void setDesiredLocation(Vector3 position)
    {
        targetPosition = position;
        isMoving = true;
        sentStopAnimation = false;
    }

    public void stopMove()
    {
        targetPosition = character.transform.position;
    }





    ServerCommunication getCommunication()
    {
        if (GameObject.Find("Communication") != null)
        {
            return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
        }
        else
        {
            return null;
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
