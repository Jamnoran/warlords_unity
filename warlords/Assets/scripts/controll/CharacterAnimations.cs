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
    public bool idleAnimationRunning = true;

	// Use this for initialization
	void Start () {
        anim = character.GetComponent<Animator>();
        rbody = character.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        //if (Input.GetKeyDown("s"))
        //{
        //    print("Slash!");
        //    attackAnimation();
        //}

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

        if (!isMoving && !idleAnimationRunning)
        {
            idleAnimationRunning = true;
            Debug.Log("Starting idle animation again");
            anim.Play("idle", -1, 0f);
        }

    }


    public void attackAnimation()
    {
        anim.Play("auto", -1, 0f);
    }

    public void setDesiredLocation(Vector3 position)
    {
        targetPosition = position;
        isMoving = true;
        idleAnimationRunning = false;
        anim.Play("walk", -1, 0f);
    }
}
