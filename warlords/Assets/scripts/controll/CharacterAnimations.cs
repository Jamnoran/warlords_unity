using UnityEngine;
using System.Collections;
using Assets.scripts.vo;

public class CharacterAnimations : MonoBehaviour {
    public GameObject character;
    private Animator anim;
    public Vector3 targetPosition;
    bool isMoving;
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 3.0F;
    public bool sentStopAnimation = false;

    int grounded = 0;
    float slopefix = 8.0f;

    public bool isGrounded = false;

	// Use this for initialization
	void Start () {
        anim = character.GetComponent<Animator>();
	}

    // Update is called once per frame
    void Update() {
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
        if (Vector3.Distance(character.transform.position, targetPosition) < 0.2f) {
                isMoving = false;
            }

            if (getGameLogic() != null) {
                Hero thisHero = getGameLogic().getClosestHeroByPosition(character.transform.position);
                if (!isMoving && !sentStopAnimation && thisHero.id == getGameLogic().getMyHero().id && !thisHero.getAutoAttacking()) {
                    sentStopAnimation = true;

                    getCommunication().sendMoveRequest(transform.position.x, transform.position.z, targetPosition.x, targetPosition.z);
                    getCommunication().sendStopHero(getGameLogic().getMyHero().id);
                }
            }
        //}else { 
            // Play dead animation
        //}
    }

    public void rotateToTarget(Vector3 postition) {
        Debug.Log("Rotating towards target");
        Vector3 rotatingPostition = new Vector3(postition.x, character.transform.position.y, postition.z);
        character.transform.LookAt(rotatingPostition);

    }

    public void attackAnimation() {
        anim.Play("auto", -1, 0f);
    }

    public void runAnimation() {
        anim.Play("walk", -1, 0f);
    }

    public void idleAnimation() {
        //Debug.Log("Starting idle animation again");
        anim.Play("idle", -1, 0f);
    }

    public void setDesiredLocation(Vector3 position) {
        targetPosition = position;
        isMoving = true;
        sentStopAnimation = false;
    }

    public void stopMove() {
        //Debug.Log("Setting target position to current position : " + character.transform.position.x + " x " + character.transform.position.z);
        targetPosition = character.transform.position;
    }





    ServerCommunication getCommunication() {
        return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
    }

    LobbyCommunication getLobbyCommunication() {
        return ((LobbyCommunication)GameObject.Find("Communication").GetComponent(typeof(LobbyCommunication)));
    }


    GameLogic getGameLogic() {
        if (GameObject.Find("GameLogicObject") != null) {
            return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
        } else {
            return null;
        }
    }
}
